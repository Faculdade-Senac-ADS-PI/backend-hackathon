using Hackthon.Application.Comum;
using Hackthon.Domain.Acesso;
using Hackthon.Domain.Usuarios;

namespace Hackthon.Application.Acesso;

public interface IRegistrarProponente
{
    Task<Resultado<SessaoResposta>> ExecutarAsync(RegistrarProponenteComando comando, CancellationToken ct);
}

public sealed class RegistrarProponenteServico : IRegistrarProponente
{
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IRegistroAcessoRepositorio _registros;
    private readonly IUnidadeDeTrabalho _unidade;
    private readonly ISenhaHasher _senhas;
    private readonly IEmissorToken _tokens;
    private readonly IRelogio _relogio;

    public RegistrarProponenteServico(
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

    public async Task<Resultado<SessaoResposta>> ExecutarAsync(RegistrarProponenteComando comando, CancellationToken ct)
    {
        var erro = Cadastro.Validar(comando.Nome, comando.Email, comando.Senha);
        if (erro is not null)
            return Resultado<SessaoResposta>.Falha(400, "Cadastro inválido", erro);

        var email = Cadastro.NormalizarEmail(comando.Email!);
        if (await _usuarios.ObterPorEmailAsync(email, ct) is not null)
            return Resultado<SessaoResposta>.Falha(409, "E-mail já cadastrado", "Já existe uma conta com este e-mail.");

        var agora = _relogio.Agora();
        var usuario = Usuario.Criar(comando.Nome!, email, _senhas.GerarHash(comando.Senha!), Papel.Proponente, agora);
        var registro = RegistroAcesso.Criar(usuario.Id, usuario.Id, AcaoAcesso.Registro, "Autocadastro de proponente", agora);

        try
        {
            await _unidade.ExecutarEmTransacaoAsync(async token =>
            {
                await _usuarios.AdicionarAsync(usuario, token);
                await _unidade.SalvarAsync(token);
                await _registros.AdicionarAsync(registro, token);
            }, ct);
        }
        catch (Exception ex) when (ex is ConflitoDeEmailException or RegraDeNegocioException)
        {
            return Falhas.Traduzir<SessaoResposta>(ex);
        }

        return Resultado<SessaoResposta>.Ok(Sessoes.Emitir(_tokens, usuario, agora));
    }
}
