using Hackthon.Application.Comum;
using Hackthon.Domain.Acesso;

namespace Hackthon.Application.Acesso;

public interface IEntrar
{
    Task<Resultado<SessaoResposta>> ExecutarAsync(EntrarComando comando, CancellationToken ct);
}

public sealed class EntrarServico : IEntrar
{
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IRegistroAcessoRepositorio _registros;
    private readonly IUnidadeDeTrabalho _unidade;
    private readonly ISenhaHasher _senhas;
    private readonly IEmissorToken _tokens;
    private readonly IRelogio _relogio;

    public EntrarServico(
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

    public async Task<Resultado<SessaoResposta>> ExecutarAsync(EntrarComando comando, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(comando.Email) || string.IsNullOrWhiteSpace(comando.Senha))
            return Resultado<SessaoResposta>.Falha(400, "Entrada inválida", "Informe e-mail e senha.");

        var email = Cadastro.NormalizarEmail(comando.Email);
        var usuario = await _usuarios.ObterPorEmailAsync(email, ct);
        if (usuario is null || !_senhas.Conferir(usuario.SenhaHash, comando.Senha))
            return Resultado<SessaoResposta>.Falha(401, "Não foi possível entrar", "E-mail ou senha inválidos.");

        if (!usuario.Ativo)
            return Resultado<SessaoResposta>.Falha(403, "Conta desativada", "Esta conta está desativada. Procure a administração.");

        var agora = _relogio.Agora();
        await _registros.AdicionarAsync(
            RegistroAcesso.Criar(usuario.Id, usuario.Id, AcaoAcesso.Login, "Entrada aceita", agora),
            ct);
        await _unidade.SalvarAsync(ct);

        return Resultado<SessaoResposta>.Ok(Sessoes.Emitir(_tokens, usuario, agora));
    }
}
