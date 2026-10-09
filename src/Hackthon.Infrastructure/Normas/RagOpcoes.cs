namespace Hackthon.Infrastructure.Normas;

/// <summary>
/// Configuração do RAG (base de normas). Nada de chave aqui — a chave continua em AgenteIa:ApiKey.
/// </summary>
public sealed class RagOpcoes
{
    public const string Secao = "Rag";

    /// <summary>Dimensão padrão dos vetores (a coluna do banco é criada com esta dimensão).</summary>
    public const int DimensaoPadrao = 768;

    /// <summary>Liga/desliga o uso da base de normas na avaliação e na consulta do analista.</summary>
    public bool Habilitado { get; set; } = true;

    /// <summary>Pasta (relativa à raiz da API ou absoluta) onde ficam os documentos oficiais.</summary>
    public string PastaBase { get; set; } = "base-conhecimento";

    /// <summary>Modelo de embeddings do provedor. O endpoint aceita gemini-embedding-001.</summary>
    public string ModeloEmbedding { get; set; } = "gemini-embedding-001";

    /// <summary>Dimensão dos vetores. Fixada em 768 (pgvector indexa até 2000 colunas).</summary>
    public int Dimensao { get; set; } = DimensaoPadrao;

    /// <summary>Quantos trechos recuperar por critério na avaliação automática.</summary>
    public int TrechosPorCriterio { get; set; } = 3;

    /// <summary>Quantos trechos recuperar na consulta livre do analista.</summary>
    public int TrechosPorConsulta { get; set; } = 6;

    /// <summary>Abaixo desta similaridade de cosseno o trecho é descartado.</summary>
    public double LimiarSimilaridade { get; set; } = 0.10;

    /// <summary>Tamanho do lote enviado ao provedor de embeddings.</summary>
    public int TamanhoLote { get; set; } = 32;
}
