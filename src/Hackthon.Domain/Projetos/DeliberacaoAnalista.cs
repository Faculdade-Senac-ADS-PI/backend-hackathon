namespace Hackthon.Domain.Projetos;

/// <summary>
/// Registro de rastreabilidade da decisão humana do analista, conforme o Guia do Participante:
/// classificação + fundamentação + evidências por ID + (quando aplicável) recorte/limitação ou
/// elo ausente. Serve de base para o parecer defendível diante da fiscalização.
/// </summary>
public sealed record DeliberacaoAnalista(
    string Justificativa,
    string? RecorteSustentado,
    string? LimitacaoTecnica,
    string? EvidenciaNecessaria,
    string? EloAusente,
    string? EvidenciasAdicionais,
    string? EvidenciasUtilizadas,
    string? EvidenciasContrarias,
    string? Lacunas,
    string? RegraAplicada);
