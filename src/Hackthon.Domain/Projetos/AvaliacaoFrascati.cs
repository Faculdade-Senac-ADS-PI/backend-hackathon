namespace Hackthon.Domain.Projetos;

/// <summary>
/// Resultado da avaliação do agente de IA sobre os 5 critérios de Frascati.
/// Garante que cada nota fique entre 0 e 20 e que o total seja a soma das notas.
/// </summary>
public sealed record AvaliacaoFrascati
{
    public int NotaNovidade { get; }
    public int NotaCriatividade { get; }
    public int NotaSistematico { get; }
    public int NotaTransferibilidade { get; }
    public int NotaIncerteza { get; }
    public int NotaTotal => NotaNovidade + NotaCriatividade + NotaSistematico + NotaTransferibilidade + NotaIncerteza;

    public string Resumo { get; }
    public string Parecer { get; }
    public string Recomendacao { get; }

    /// <summary>JSON com a justificativa textual de cada critério (chave = <see cref="CriteriosFrascati"/>).</summary>
    public string JustificativasJson { get; }

    /// <summary>Quem produziu a avaliação (ex.: "llm:gpt-4o-mini" ou "heuristico").</summary>
    public string Origem { get; }

    public AvaliacaoFrascati(
        int notaNovidade,
        int notaCriatividade,
        int notaSistematico,
        int notaTransferibilidade,
        int notaIncerteza,
        string resumo,
        string parecer,
        string recomendacao,
        string justificativasJson,
        string origem)
    {
        NotaNovidade = Validar(notaNovidade, nameof(notaNovidade));
        NotaCriatividade = Validar(notaCriatividade, nameof(notaCriatividade));
        NotaSistematico = Validar(notaSistematico, nameof(notaSistematico));
        NotaTransferibilidade = Validar(notaTransferibilidade, nameof(notaTransferibilidade));
        NotaIncerteza = Validar(notaIncerteza, nameof(notaIncerteza));

        if (string.IsNullOrWhiteSpace(resumo))
            throw new ArgumentException("O resumo da avaliação é obrigatório.", nameof(resumo));

        Resumo = resumo.Trim();
        Parecer = parecer?.Trim() ?? string.Empty;
        Recomendacao = string.IsNullOrWhiteSpace(recomendacao) ? ClassificarPorNota(NotaTotal) : recomendacao.Trim();
        JustificativasJson = string.IsNullOrWhiteSpace(justificativasJson) ? "{}" : justificativasJson;
        Origem = string.IsNullOrWhiteSpace(origem) ? "desconhecida" : origem.Trim();
    }

    /// <summary>
    /// Faixas de referência mapeadas para as quatro classes oficiais do desafio:
    /// ≥75 Elegível · 50–74 Com ressalvas · &lt;50 Não elegível.
    /// "Evidência insuficiente" é decidido pelo analisador quando faltar material para concluir.
    /// </summary>
    public static string ClassificarPorNota(int notaTotal) => notaTotal switch
    {
        >= 75 => "Elegível",
        >= 50 => "Com ressalvas",
        _ => "Não elegível"
    };

    private static int Validar(int nota, string campo)
    {
        if (nota < 0 || nota > CriteriosFrascati.NotaMaximaPorCriterio)
            throw new ArgumentOutOfRangeException(campo, $"Cada critério vale de 0 a {CriteriosFrascati.NotaMaximaPorCriterio}.");
        return nota;
    }
}
