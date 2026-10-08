namespace Hackthon.Domain.Projetos;

/// <summary>
/// Os 5 critérios de elegibilidade (Manual de Frascati / Lei 11.196/2005) avaliados pelo agente.
/// Fonte única de verdade: o prompt da IA, o motor heurístico e a documentação usam estas definições.
/// </summary>
public static class CriteriosFrascati
{
    public const int NotaMaximaPorCriterio = 20;
    public const int NotaMaximaTotal = NotaMaximaPorCriterio * 5;

    public const string Novidade = "Novidade";
    public const string Criatividade = "Criatividade";
    public const string Sistematico = "Sistematico";
    public const string Transferibilidade = "Transferibilidade";
    public const string Incerteza = "Incerteza";

    public static readonly IReadOnlyList<DefinicaoCriterio> Todos =
    [
        new(Novidade, "Novidade",
            "O projeto deve ser inovador no sentido de desenvolver ou descobrir o que não é dominado ou conhecido, significando um avanço impactante em relação ao conhecimento existente."),
        new(Criatividade, "Criatividade",
            "O projeto deve ser relevante de forma criativa e mostrar a capacidade de produzir ganhos para a empresa ou para o setor determinado."),
        new(Sistematico, "Sistemático",
            "O P&D precisa ser realizado de maneira planejada e organizada, com registros de processos e resultados. Deve haver métodos, planejamento, testes e registro."),
        new(Transferibilidade, "Transferibilidade",
            "O conhecimento ou resultado deve ter a capacidade de ser explicado e reproduzido em outras organizações."),
        new(Incerteza, "Incerteza",
            "Deve existir incerteza científica ou tecnológica quanto ao resultado ou ao caminho para alcançá-lo, que não possa ser resolvida com o conhecimento técnico usual ou de rotina.")
    ];
}

public sealed record DefinicaoCriterio(string Chave, string Nome, string Descricao);
