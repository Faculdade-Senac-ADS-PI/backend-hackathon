using Hackthon.Application.Comum;

namespace Hackthon.Application.Acesso;

public interface IObterPerfil
{
    Task<Resultado<UsuarioResumo>> ExecutarAsync(Guid usuarioId, CancellationToken ct);
}

public sealed class ObterPerfilServico : IObterPerfil
{
    private readonly IUsuarioRepositorio _usuarios;

    public ObterPerfilServico(IUsuarioRepositorio usuarios)
    {
        _usuarios = usuarios;
    }

    public async Task<Resultado<UsuarioResumo>> ExecutarAsync(Guid usuarioId, CancellationToken ct)
    {
        var usuario = await _usuarios.ObterPorIdAsync(usuarioId, ct);
        if (usuario is null)
            return Resultado<UsuarioResumo>.Falha(404, "Conta não encontrada", "Não há uma conta para esta sessão.");

        if (!usuario.Ativo)
            return Resultado<UsuarioResumo>.Falha(403, "Conta desativada", "Esta conta está desativada. Procure a administração.");

        return Resultado<UsuarioResumo>.Ok(UsuarioMapeador.Resumir(usuario));
    }
}
