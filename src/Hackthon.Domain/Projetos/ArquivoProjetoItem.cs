namespace Hackthon.Domain.Projetos;

/// <summary>
/// Representa um arquivo (PDF ou CSV) anexado a uma proposta de projeto.
/// </summary>
public sealed class ArquivoProjetoItem
{
    public Guid Id { get; private set; }
    public Guid ProjetoId { get; private set; }
    public string NomeOriginal { get; private set; } = null!;
    public string Caminho { get; private set; } = null!;
    public long TamanhoBytes { get; private set; }
    public string Extensao { get; private set; } = null!;
    public DateTimeOffset CriadoEm { get; private set; }

    private ArquivoProjetoItem()
    {
    }

    public ArquivoProjetoItem(
        Guid id,
        Guid projetoId,
        string nomeOriginal,
        string caminho,
        long tamanhoBytes,
        string extensao,
        DateTimeOffset criadoEm)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id inválido.", nameof(id));
        if (projetoId == Guid.Empty)
            throw new ArgumentException("ProjetoId inválido.", nameof(projetoId));
        if (string.IsNullOrWhiteSpace(nomeOriginal))
            throw new ArgumentException("O nome original do arquivo é obrigatório.", nameof(nomeOriginal));
        if (string.IsNullOrWhiteSpace(caminho))
            throw new ArgumentException("O caminho do arquivo é obrigatório.", nameof(caminho));
        if (tamanhoBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(tamanhoBytes), "O arquivo não pode ser vazio.");

        Id = id;
        ProjetoId = projetoId;
        NomeOriginal = nomeOriginal.Trim();
        Caminho = caminho.Trim();
        TamanhoBytes = tamanhoBytes;
        Extensao = extensao.ToLowerInvariant().Trim();
        CriadoEm = criadoEm;
    }
}
