using Hackthon.Application.Comum;

namespace Hackthon.Application.Agente;

/// <summary>
/// Porta do agente que apoia o analista na leitura das normas da Lei do Bem.
/// A implementação futura consulta embeddings no PostgreSQL (pgvector) e devolve trechos citados.
/// Ela não fecha o enquadramento: a decisão continua com o analista.
/// </summary>
public interface IAgenteElegibilidade
{
    ContratoAgente DescreverContrato();

    Task<Resultado<ConsultaAgenteResposta>> ConsultarAsync(ConsultaAgenteComando comando, CancellationToken ct);
}

public sealed record ConsultaAgenteComando(Guid? ProjetoId, string? Pergunta);

public sealed record TrechoNorma(string Documento, string Trecho, string? Referencia);

public sealed record ConsultaAgenteResposta(
    string RespostaApoio,
    IReadOnlyList<TrechoNorma> Citacoes,
    bool DecisaoAutomatica);

public sealed record ContratoAgente(
    string Estado,
    string Responsabilidade,
    string ComoPlugar,
    IReadOnlyList<string> EntradasPrevistas,
    string Saida);
