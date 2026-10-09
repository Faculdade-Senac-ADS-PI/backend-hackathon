using Hackthon.Domain.Projetos;

namespace Hackthon.Application.Projetos;

public static class RegrasDocumento
{
    public const long TamanhoMaximoBytesPorArquivo = 15 * 1024 * 1024; // 15 MB por arquivo
    public const long TamanhoMaximoBytesTotal = 30 * 1024 * 1024; // 30 MB total
    public const int MinimoCaracteresTexto = 100;
}

public sealed record ArquivoEntradaComando(
    string NomeArquivo,
    long TamanhoBytes,
    Stream Conteudo);

public sealed record ArquivoItemResposta(
    Guid Id,
    string Nome,
    long TamanhoBytes,
    string Extensao,
    DateTimeOffset CriadoEm);

public sealed record EnviarPropostaComando(
    Guid ProponenteId,
    string? Titulo,
    string? Setor,
    IReadOnlyList<ArquivoEntradaComando> Arquivos);

public sealed record ReenviarPropostaComando(
    Guid ProjetoId,
    Guid ProponenteId,
    IReadOnlyList<ArquivoEntradaComando> Arquivos);

/// <summary>Deliberação do analista: uma das 4 classes do desafio + campos de rastreabilidade.</summary>
public sealed record DeliberarProjetoComando(
    Guid ProjetoId,
    Guid AnalistaId,
    string? Decisao,
    string? Justificativa,
    string? RecorteSustentado,
    string? LimitacaoTecnica,
    string? EvidenciaNecessaria,
    string? EloAusente,
    string? EvidenciasAdicionais,
    string? EvidenciasUtilizadas,
    string? EvidenciasContrarias,
    string? Lacunas,
    string? RegraAplicada);

/// <summary>Visão do proponente: confirmação de envio, status e arquivos.</summary>
public sealed record ProjetoProponenteResposta(
    Guid Id,
    string Protocolo,
    string Titulo,
    string Setor,
    string NomeArquivo,
    IReadOnlyList<ArquivoItemResposta> Arquivos,
    int VersaoEnvio,
    StatusProjeto Status,
    string? JustificativaAnalista,
    DateTimeOffset CriadoEm,
    DateTimeOffset AtualizadoEm,
    DateTimeOffset? DecididoEm);

public sealed record CriterioAvaliadoResposta(
    string Chave,
    string Nome,
    string Descricao,
    int? Nota,
    int NotaMaxima,
    string? Justificativa,
    string? NormasCitadas = null);

/// <summary>Visão do analista: dossiê completo com resumo, nota, critérios avaliados pela IA e lista de arquivos.</summary>
public sealed record ProjetoAnaliseResposta(
    Guid Id,
    string Protocolo,
    string Titulo,
    string Setor,
    StatusProjeto Status,
    int VersaoEnvio,
    Guid ProponenteId,
    string ProponenteNome,
    string ProponenteEmail,
    string NomeArquivo,
    long TamanhoBytes,
    IReadOnlyList<ArquivoItemResposta> Arquivos,
    int? NotaIa,
    string? RecomendacaoIa,
    string? ResumoIa,
    string? ParecerIa,
    string? OrigemAnaliseIa,
    string? ClasseRecomendadaIa,
    string? MotivoClasseIa,
    string? EvidenciasDossieIa,
    string? ContradicoesIa,
    string? LacunasIa,
    bool UsouRag,
    int TrechosConsultadosIa,
    DateTimeOffset? AnalisadoEm,
    IReadOnlyList<CriterioAvaliadoResposta> Criterios,
    string? JustificativaAnalista,
    string? AnalistaNome,
    DateTimeOffset? DecididoEm,
    string? RecorteSustentado,
    string? LimitacaoTecnica,
    string? EvidenciaNecessaria,
    string? EloAusente,
    string? EvidenciasAdicionais,
    string? EvidenciasUtilizadas,
    string? EvidenciasContrarias,
    string? Lacunas,
    string? RegraAplicada,
    DateTimeOffset CriadoEm,
    DateTimeOffset AtualizadoEm);

/// <summary>Parecer rastreável gerado pelo sistema para a classificação do projeto (documento defendível).</summary>
public sealed record ParecerResposta(
    Guid ProjetoId,
    string Protocolo,
    string Titulo,
    string Classificacao,
    string Markdown,
    DateTimeOffset GeradoEm);

public sealed record ArquivoProjeto(Stream Conteudo, string NomeArquivo, string TipoMime);
