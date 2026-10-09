using Hackthon.Application.Comum;
using Hackthon.Application.Normas;
using Hackthon.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace Hackthon.Infrastructure.Normas;

/// <summary>Persistência e busca vetorial dos trechos de norma em PostgreSQL + pgvector.</summary>
public sealed class NormaRepositorio : INormaRepositorio
{
    private readonly HackthonDbContext _db;
    private readonly IRelogio _relogio;

    public NormaRepositorio(HackthonDbContext db, IRelogio relogio)
    {
        _db = db;
        _relogio = relogio;
    }

    public Task<bool> HaTrechosAsync(CancellationToken ct) =>
        _db.TrechosNormativos.AsNoTracking().AnyAsync(ct);

    public async Task SubstituirDocumentoAsync(
        string documento,
        string? arquivo,
        IReadOnlyList<TrechoParaIndexar> trechos,
        CancellationToken ct)
    {
        var agora = _relogio.Agora();
        var existente = await _db.DocumentosNormativos
            .Include(d => d.Itens)
            .FirstOrDefaultAsync(d => d.Nome == documento, ct);

        if (existente is null)
        {
            existente = new DocumentoNormativo
            {
                Id = Guid.NewGuid(),
                Nome = documento,
                CriadoEm = agora,
                AtualizadoEm = agora
            };
            _db.DocumentosNormativos.Add(existente);
        }
        else
        {
            _db.TrechosNormativos.RemoveRange(existente.Itens);
            existente.Itens.Clear();
        }

        existente.Arquivo = arquivo;
        existente.Hash = trechos.Count > 0 ? trechos[0].Hash : null;
        existente.Trechos = trechos.Count;
        existente.AtualizadoEm = agora;

        foreach (var trecho in trechos)
        {
            existente.Itens.Add(new TrechoNormativo
            {
                Id = Guid.NewGuid(),
                DocumentoId = existente.Id,
                DocumentoNome = documento,
                Referencia = trecho.Referencia,
                Pagina = trecho.Pagina,
                Ordem = trecho.Ordem,
                Texto = trecho.Texto,
                Hash = trecho.Hash,
                Embedding = new Vector(trecho.Vetor),
                CriadoEm = agora
            });
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ContagemDocumento>> ContarPorDocumentoAsync(CancellationToken ct)
    {
        return await _db.DocumentosNormativos
            .AsNoTracking()
            .OrderBy(d => d.Nome)
            .Select(d => new ContagemDocumento(d.Nome, d.Trechos, d.AtualizadoEm))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TrechoRecuperado>> BuscarAsync(
        float[] vetorConsulta,
        int quantidade,
        CancellationToken ct)
    {
        var vetor = new Vector(vetorConsulta);
        var itens = await _db.TrechosNormativos
            .AsNoTracking()
            .OrderBy(t => t.Embedding.CosineDistance(vetor))
            .Take(Math.Max(1, quantidade))
            .Select(t => new
            {
                t.DocumentoNome,
                t.Referencia,
                t.Pagina,
                t.Texto,
                Distancia = t.Embedding.CosineDistance(vetor)
            })
            .ToListAsync(ct);

        return itens
            .Select(item => new TrechoRecuperado(item.DocumentoNome, item.Referencia, item.Pagina, item.Texto, item.Distancia))
            .ToList();
    }
}
