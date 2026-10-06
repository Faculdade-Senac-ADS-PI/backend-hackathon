namespace Hackthon.Application.Acesso;

public interface IListarUsuarios
{
    Task<IReadOnlyList<UsuarioResumo>> ExecutarAsync(CancellationToken ct);
}

public sealed class ListarUsuariosServico : IListarUsuarios
{
    private readonly IUsuarioRepositorio _usuarios;

    public ListarUsuariosServico(IUsuarioRepositorio usuarios)
    {
        _usuarios = usuarios;
    }

    public async Task<IReadOnlyList<UsuarioResumo>> ExecutarAsync(CancellationToken ct)
    {
        var usuarios = await _usuarios.ListarAsync(ct);
        return usuarios.Select(UsuarioMapeador.Resumir).ToList();
    }
}
