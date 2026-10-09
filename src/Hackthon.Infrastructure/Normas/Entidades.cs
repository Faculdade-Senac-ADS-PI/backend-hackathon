using Pgvector;

namespace Hackthon.Infrastructure.Normas;

/// <summary>
/// Documento de norma indexado (um por arquivo da pasta de conhecimento).
/// Reindexar o mesmo nome substitui os trechos, sem duplicar.
/// </summary>
public sealed class DocumentoNormativo
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? Arquivo { get; set; }
    public string? Hash { get; set; }
    public int Trechos { get; set; }
    public DateTimeOffset CriadoEm { get; set; }
    public DateTimeOffset AtualizadoEm { get; set; }

    public List<TrechoNormativo> Itens { get; set; } = new();
}

/// <summary>Trecho de norma com o respectivo vetor (pgvector).</summary>
public sealed class TrechoNormativo
{
    public Guid Id { get; set; }
    public Guid DocumentoId { get; set; }
    public string DocumentoNome { get; set; } = null!;
    public string? Referencia { get; set; }
    public int? Pagina { get; set; }
    public int Ordem { get; set; }
    public string Texto { get; set; } = null!;
    public string Hash { get; set; } = null!;
    public Vector Embedding { get; set; } = null!;
    public DateTimeOffset CriadoEm { get; set; }

    public DocumentoNormativo Documento { get; set; } = null!;
}
