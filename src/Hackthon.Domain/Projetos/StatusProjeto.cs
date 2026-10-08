namespace Hackthon.Domain.Projetos;

/// <summary>
/// Ciclo de vida de uma proposta na esteira de elegibilidade da Lei do Bem.
/// </summary>
public enum StatusProjeto
{
    /// <summary>PDF recebido; o agente de IA ainda não concluiu a avaliação.</summary>
    ProcessandoIa = 1,

    /// <summary>Avaliação da IA concluída; aguardando a deliberação do analista.</summary>
    AguardandoAnalista = 2,

    /// <summary>Analista considerou o projeto elegível à Lei do Bem.</summary>
    Aprovado = 3,

    /// <summary>Analista considerou o projeto inelegível (ex.: trabalho de rotina).</summary>
    Reprovado = 4,

    /// <summary>Analista pediu complementação; o proponente pode reenviar o PDF.</summary>
    Reanalise = 5
}
