using Hackthon.Domain.Projetos;

namespace Hackthon.Application.Projetos;

public static class RegrasDocumento
{
    public const long TamanhoMaximoBytes = 10 * 1024 * 1024;
    public const int MinimoCaracteresTexto = 200;
}

public sealed record EnviarPropostaComando(
    Guid ProponenteId,
    string? Titulo,
    string? Setor,
    string? NomeArquivo,
    long TamanhoBytes,
    Stream? Conteudo);

public sealed record ReenviarPropostaComando(
    Guid ProjetoId,
    Guid ProponenteId,
    string? NomeArquivo,
    long TamanhoBytes,
    Stream? Conteudo);

public sealed record DeliberarProjetoComando(
    Guid ProjetoId,
    Guid AnalistaId,
    string? Decisao,
    string? Justificativa);

/// <summary>Visão do proponente: confirmação de envio, status e resultado. Não expõe notas internas da IA.</summary>
public sealed record ProjetoProponenteResposta(
    Guid Id,
    string Protocolo,
    string Titulo,
    string Setor,
    string NomeArquivo,
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
    string? Justificativa);

/// <summary>Visão do analista: dossiê completo com resumo, nota e critérios avaliados pela IA.</summary>
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
    int? NotaIa,
    string? RecomendacaoIa,
    string? ResumoIa,
    string? ParecerIa,
    string? OrigemAnaliseIa,
    DateTimeOffset? AnalisadoEm,
    IReadOnlyList<CriterioAvaliadoResposta> Criterios,
    string? JustificativaAnalista,
    string? AnalistaNome,
    DateTimeOffset? DecididoEm,
    DateTimeOffset CriadoEm,
    DateTimeOffset AtualizadoEm);

public sealed record ArquivoProjeto(Stream Conteudo, string NomeArquivo);
