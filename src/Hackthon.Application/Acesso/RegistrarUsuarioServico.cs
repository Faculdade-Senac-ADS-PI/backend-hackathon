using Hackthon.Application.Comum;
using Hackthon.Domain.Acesso;
using Hackthon.Domain.Usuarios;

namespace Hackthon.Application.Acesso;

public interface IRegistrarUsuario
{
    Task<Resultado<SessaoResposta>> ExecutarAsync(RegistrarUsuarioComando comando, CancellationToken ct);
}

public interface IRegistrarProponente
{
    Task<Resultado<SessaoResposta>> ExecutarAsync(RegistrarProponenteComando comando, CancellationToken ct);
}

public sealed class RegistrarUsuarioServico : IRegistrarUsuario, IRegistrarProponente
{
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IRegistroAcessoRepositorio _registros;
    private readonly IUnidadeDeTrabalho _unidade;
    private readonly ISenhaHasher _senhas;
    private readonly IEmissorToken _tokens;
    private readonly IRelogio _relogio;

    public RegistrarUsuarioServico(
        IUsuarioRepositorio usuarios,
        IRegistroAcessoRepositorio registros,
        IUnidadeDeTrabalho unidade,
        ISenhaHasher senhas,
        IEmissorToken tokens,
        IRelogio relogio)
    {
        _usuarios = usuarios;
        _registros = registros;
        _unidade = unidade;
        _senhas = senhas;
        _tokens = tokens;
        _relogio = relogio;
    }

    public Task<Resultado<SessaoResposta>> ExecutarAsync(RegistrarProponenteComando comando, CancellationToken ct) =>
        ExecutarAsync(new RegistrarUsuarioComando(comando.Nome, comando.Email, comando.CpfCnpj, comando.Senha, "Proponente"), ct);

    public async Task<Resultado<SessaoResposta>> ExecutarAsync(RegistrarUsuarioComando comando, CancellationToken ct)
    {
        var erro = Cadastro.Validar(comando.Nome, comando.Email, comando.CpfCnpj, comando.Senha);
        if (erro is not null)
            return Resultado<SessaoResposta>.Falha(400, "Cadastro inválido", erro);

        var papel = Papel.Proponente;
        if (!string.IsNullOrWhiteSpace(comando.Papel))
        {
            if (!Cadastro.TentarPapel(comando.Papel, out papel))
                return Resultado<SessaoResposta>.Falha(400, "Papel inválido", "Escolha Proponente ou Assistente.");

            if (papel == Papel.Adm)
            {
                return Resultado<SessaoResposta>.Falha(
                    403,
                    "Cadastro recusado",
                    "A conta de ADM não pode ser criada no autocadastro. Ela nasce com a administração.");
            }
        }

        var email = Cadastro.NormalizarEmail(comando.Email!);
        if (await _usuarios.ObterPorEmailAsync(email, ct) is not null)
            return Resultado<SessaoResposta>.Falha(409, "E-mail já cadastrado", "Já existe uma conta com este e-mail.");

        var cpfCnpj = Cadastro.NormalizarCpfCnpj(comando.CpfCnpj!);
        if (await _usuarios.ObterPorCpfCnpjAsync(cpfCnpj, ct) is not null)
            return Resultado<SessaoResposta>.Falha(409, "CPF/CNPJ já cadastrado", "Já existe uma conta com este CPF/CNPJ.");

        var agora = _relogio.Agora();
        var usuario = Usuario.Criar(comando.Nome!, email, cpfCnpj, _senhas.GerarHash(comando.Senha!), papel, agora);
        var registro = RegistroAcesso.Criar(usuario.Id, usuario.Id, AcaoAcesso.Registro, $"Autocadastro de {papel}", agora);

        try
        {
            await _unidade.ExecutarEmTransacaoAsync(async token =>
            {
                await _usuarios.AdicionarAsync(usuario, token);
                await _unidade.SalvarAsync(token);
                await _registros.AdicionarAsync(registro, token);
            }, ct);
        }
        catch (Exception ex) when (ex is ConflitoDeEmailException or ConflitoDeCpfCnpjException or RegraDeNegocioException)
        {
            return Falhas.Traduzir<SessaoResposta>(ex);
        }

        return Resultado<SessaoResposta>.Ok(Sessoes.Emitir(_tokens, usuario, agora));
    }
}
