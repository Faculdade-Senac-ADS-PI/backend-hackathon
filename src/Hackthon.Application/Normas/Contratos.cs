using Hackthon.Application.Comum;

namespace Hackthon.Application.Normas;

// ============================================================================
// Portas do RAG (recuperação de normas da Lei do Bem)
// A base de conhecimento contém SOMENTE os documentos que o usuário colocar
// na pasta configurada. Nada de texto de norma inventado no código.
// ============================================================================

/// <summary>Gera embeddings (vetores) para textos usando o provedor de IA configurado.</summary>
public interface IGeradorEmbedding
{
    /// <summary>Gera o vetor de um único texto. Lança exceção se o provedor falhar.</summary>
    Task<float[]> GerarAsync(string texto, CancellationToken ct);

    /// <summary>Gera vetores para um lote, preservando a ordem de entrada.</summary>
    Task<IReadOnlyList<float[]>> GerarLoteAsync(IReadOnlyList<string> textos, CancellationToken ct);
}

/// <summary>Lê os documentos de norma da pasta de conhecimento (PDF/MD/TXT).</summary>
public interface IFonteConhecimento
{
    /// <summary>Caminho absoluto da pasta monitorada.</summary>
    string Pasta { get; }

    /// <summary>Lista os documentos disponíveis, já com o texto extraído. Vazio quando não há documentos.</summary>
    Task<IReadOnlyList<DocumentoFonte>> ListarAsync(CancellationToken ct);
}

/// <summary>Persistência e busca vetorial dos trechos de norma (pgvector).</summary>
public interface INormaRepositorio
{
    Task<bool> HaTrechosAsync(CancellationToken ct);

    /// <summary>Substitui o documento (remove trechos antigos) e grava os novos trechos.</summary>
    Task SubstituirDocumentoAsync(string documento, string? arquivo, IReadOnlyList<TrechoParaIndexar> trechos, CancellationToken ct);

    Task<IReadOnlyList<ContagemDocumento>> ContarPorDocumentoAsync(CancellationToken ct);

    /// <summary>Busca os <paramref name="quantidade"/> trechos mais próximos do vetor de consulta (cosseno).</summary>
    Task<IReadOnlyList<TrechoRecuperado>> BuscarAsync(float[] vetorConsulta, int quantidade, CancellationToken ct);
}

/// <summary>Recupera trechos de norma relevantes para uma consulta em linguagem natural.</summary>
public interface IRecuperadorTrechos
{
    Task<IReadOnlyList<TrechoRecuperado>> BuscarAsync(string consulta, int? quantidade, CancellationToken ct);

    Task<bool> BaseDisponivelAsync(CancellationToken ct);
}

/// <summary>Prepara/indexa a base de normas a partir da pasta de conhecimento. Restrito ao ADM.</summary>
public interface IIndexarBase
{
    Task<Resultado<EstadoBaseResposta>> ExecutarAsync(CancellationToken ct);
}

/// <summary>Consulta o estado atual da base de normas (transparência no admin/analista).</summary>
public interface IConsultarBase
{
    Task<EstadoBaseResposta> ObterAsync(CancellationToken ct);
}

// ============================================================================
// Modelos de apoio
// ============================================================================

/// <summary>Documento lido da pasta de conhecimento, com o texto já extraído.</summary>
public sealed record DocumentoFonte(string Nome, string? Caminho, string Texto);

/// <summary>Trecho pronto para ser gravado com o respectivo vetor.</summary>
public sealed record TrechoParaIndexar(string Referencia, int? Pagina, int Ordem, string Texto, string Hash, float[] Vetor);

/// <summary>Trecho devolvido pela busca vetorial.</summary>
public sealed record TrechoRecuperado(string Documento, string? Referencia, int? Pagina, string Texto, double Distancia)
{
    /// <summary>Similaridade de cosseno (1 - distância); quanto maior, mais relevante.</summary>
    public double Similaridade => 1 - Distancia;
}

/// <summary>Quantidade de trechos por documento (para a tela de status da base).</summary>
public sealed record ContagemDocumento(string Documento, int Trechos, DateTimeOffset? AtualizadoEm);

/// <summary>Estado da base de normas: quantos documentos e trechos estão indexados.</summary>
public sealed record EstadoBaseResposta(
    string Pasta,
    bool Configurada,
    int TotalTrechos,
    IReadOnlyList<ContagemDocumento> Documentos,
    string Mensagem);
