using Hackthon.Application.Comum;
using Hackthon.Domain.Acesso;
using Hackthon.Domain.Usuarios;

namespace Hackthon.Application.Acesso;

public interface ICriarUsuario
{
    Task<Resultado<UsuarioResumo>> ExecutarAsync(CriarUsuarioComando comando, CancellationToken ct);
}

public sealed class CriarUsuarioServico : ICriarUsuario
{
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IRegistroAcessoRepositorio _registros;
    private readonly IUnidadeDeTrabalho _unidade;
    private readonly ISenhaHasher _senhas;
    private readonly IRelogio _relogio;

    public CriarUsuarioServico(
        IUsuarioRepositorio usuarios,
        IRegistroAcessoRepositorio registros,
        IUnidadeDeTrabalho unidade,
        ISenhaHasher senhas,
        IRelogio relogio)
    {
        _usuarios = usuarios;
        _registros = registros;
        _unidade = unidade;
        _senhas = senhas;
        _relogio = relogio;
    }

    public async Task<Resultado<UsuarioResumo>> ExecutarAsync(CriarUsuarioComando comando, CancellationToken ct)
    {
        var erro = Cadastro.Validar(comando.Nome, comando.Email, comando.CpfCnpj, comando.Senha);
        if (erro is not null)
            return Resultado<UsuarioResumo>.Falha(400, "Cadastro inválido", erro);

        if (!Cadastro.TentarPapel(comando.Papel, out var papel))
            return Resultado<UsuarioResumo>.Falha(400, "Papel inválido", "Escolha Proponente, Assistente, Analista ou ADM.");

        var email = Cadastro.NormalizarEmail(comando.Email!);
        if (await _usuarios.ObterPorEmailAsync(email, ct) is not null)
            return Resultado<UsuarioResumo>.Falha(409, "E-mail já cadastrado", "Já existe uma conta com este e-mail.");

        var cpfCnpj = Cadastro.NormalizarCpfCnpj(comando.CpfCnpj!);
        if (await _usuarios.ObterPorCpfCnpjAsync(cpfCnpj, ct) is not null)
            return Resultado<UsuarioResumo>.Falha(409, "CPF/CNPJ já cadastrado", "Já existe uma conta com este CPF/CNPJ.");

        var agora = _relogio.Agora();
        var usuario = Usuario.Criar(comando.Nome!, email, cpfCnpj, _senhas.GerarHash(comando.Senha!), papel, agora);
        var registro = RegistroAcesso.Criar(
            usuario.Id,
            comando.AtorId,
            AcaoAcesso.Criacao,
            $"Conta criada com papel {papel}",
            agora);

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
            return Falhas.Traduzir<UsuarioResumo>(ex);
        }

        return Resultado<UsuarioResumo>.Ok(UsuarioMapeador.Resumir(usuario));
    }
}
