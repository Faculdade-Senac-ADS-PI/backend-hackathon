namespace Hackthon.Domain.Projetos;

/// <summary>
/// Ciclo de vida de um projeto na esteira de elegibilidade da Lei do Bem.
/// Após a deliberação do analista, o status É a classificação final, usando as
/// quatro classes oficiais do desafio (Guia do Participante):
/// Elegível · Com ressalvas · Não elegível · Evidência insuficiente.
/// "Reanalise" é estado de fluxo (o proponente ainda vai complementar), não classificação.
/// </summary>
public enum StatusProjeto
{
    /// <summary>Documentos recebidos; o agente de IA ainda não concluiu a avaliação.</summary>
    ProcessandoIa = 1,

    /// <summary>Avaliação da IA concluída; aguardando a deliberação do analista.</summary>
    AguardandoAnalista = 2,

    /// <summary>As evidências caracterizam P&D no escopo definido (novidade, criatividade,
    /// incerteza investigada, trabalho sistemático e conhecimento transferível).</summary>
    Elegivel = 3,

    /// <summary>As evidências caracterizam aplicação de técnica conhecida, configuração,
    /// integração, migração ou verificação rotineira, sem investigação tecnológica demonstrada.</summary>
    NaoElegivel = 4,

    /// <summary>Fluxo: o analista pediu complementação e o proponente vai reenviar os documentos.</summary>
    Reanalise = 5,

    /// <summary>Há prova suficiente de P&D, mas uma limitação técnica ou de validação concreta
    /// restringe parte da conclusão pretendida dentro do escopo.</summary>
    ComRessalvas = 6,

    /// <summary>Falta informação essencial para caracterizar ou verificar o núcleo alegado;
    /// não é possível concluir fundamentadamente entre P&amp;D e rotina.</summary>
    EvidenciaInsuficiente = 7
}
