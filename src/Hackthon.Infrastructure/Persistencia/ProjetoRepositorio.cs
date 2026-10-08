using Hackthon.Application.Projetos;
using Hackthon.Domain.Projetos;
using Microsoft.EntityFrameworkCore;

namespace Hackthon.Infrastructure.Persistencia;

public sealed class ProjetoRepositorio : IProjetoRepositorio
{
    private readonly HackthonDbContext _db;

    public ProjetoRepositorio(HackthonDbContext db)
    {
        _db = db;
    }

    public Task<Projeto?> ObterPorIdAsync(Guid id, CancellationToken ct) =>
        _db.Projetos.FirstOrDefaultAsync(projeto => projeto.Id == id, ct);

    public async Task<IReadOnlyList<Projeto>> ListarPorProponenteAsync(Guid proponenteId, CancellationToken ct) =>
        await _db.Projetos
            .AsNoTracking()
            .Where(projeto => projeto.ProponenteId == proponenteId)
            .OrderByDescending(projeto => projeto.CriadoEm)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Projeto>> ListarAsync(StatusProjeto? status, CancellationToken ct)
    {
        var consulta = _db.Projetos.AsNoTracking();
        if (status is not null)
            consulta = consulta.Where(projeto => projeto.Status == status);

        return await consulta.OrderByDescending(projeto => projeto.CriadoEm).ToListAsync(ct);
    }

    public Task AdicionarAsync(Projeto projeto, CancellationToken ct) =>
        _db.Projetos.AddAsync(projeto, ct).AsTask();

    public Task<bool> ExisteProtocoloAsync(string protocolo, CancellationToken ct) =>
        _db.Projetos.AnyAsync(projeto => projeto.Protocolo == protocolo, ct);
}
