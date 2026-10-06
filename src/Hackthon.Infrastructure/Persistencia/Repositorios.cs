using Hackthon.Application.Acesso;
using Hackthon.Domain.Acesso;
using Hackthon.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace Hackthon.Infrastructure.Persistencia;

public sealed class UsuarioRepositorio : IUsuarioRepositorio
{
    private readonly HackthonDbContext _db;

    public UsuarioRepositorio(HackthonDbContext db)
    {
        _db = db;
    }

    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct) =>
        _db.Usuarios.FirstOrDefaultAsync(usuario => usuario.Email == email, ct);

    public Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken ct) =>
        _db.Usuarios.FirstOrDefaultAsync(usuario => usuario.Id == id, ct);

    public async Task<IReadOnlyList<Usuario>> ListarAsync(CancellationToken ct) =>
        await _db.Usuarios.AsNoTracking().OrderBy(usuario => usuario.Nome).ToListAsync(ct);

    public Task AdicionarAsync(Usuario usuario, CancellationToken ct) =>
        _db.Usuarios.AddAsync(usuario, ct).AsTask();

    public Task<int> ContarAdmAtivosExcetoAsync(Guid usuarioId, CancellationToken ct) =>
        _db.Usuarios.CountAsync(
            usuario => usuario.Papel == Papel.Adm && usuario.Ativo && usuario.Id != usuarioId,
            ct);
}

public sealed class RegistroAcessoRepositorio : IRegistroAcessoRepositorio
{
    private readonly HackthonDbContext _db;

    public RegistroAcessoRepositorio(HackthonDbContext db)
    {
        _db = db;
    }

    public Task AdicionarAsync(RegistroAcesso registro, CancellationToken ct) =>
        _db.RegistrosAcesso.AddAsync(registro, ct).AsTask();
}
