using System.Globalization;
using System.Text;

namespace Hackthon.Domain.Projetos;

/// <summary>
/// Resultado da avaliação do agente de IA sobre os 5 critérios de Frascati.
/// Garante que cada nota fique entre 0 e 20 e que o total seja a soma das notas.
///
/// Além das notas, carrega a classe recomendada (uma das 4 classes oficiais) e os
/// elementos de rastreabilidade produzidos pelo agente com apoio da base de normas (RAG):
/// motivo, normas citadas por critério, evidências do dossiê, contradições e lacunas.
/// A classe recomendada é qualitativa; a nota é apenas um indício e um plano B.
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

    /// <summary>Classe recomendada (canônica): Elegível · Com ressalvas · Não elegível · Evidência insuficiente.</summary>
    public string Recomendacao { get; }

    /// <summary>JSON com a justificativa textual de cada critério (chave = <see cref="CriteriosFrascati"/>).</summary>
    public string JustificativasJson { get; }

    /// <summary>Quem produziu a avaliação (ex.: "llm:gemini-2.5-flash" ou "heuristico").</summary>
    public string Origem { get; }

    /// <summary>Mesmo valor de <see cref="Recomendacao"/>, exposto com o nome de domínio do desafio.</summary>
    public string ClasseRecomendada { get; }

    /// <summary>Por que a classe foi recomendada (não é a soma das notas).</summary>
    public string? MotivoClasse { get; }

    /// <summary>JSON { critério: trechos de norma citados } produzido com o apoio do RAG.</summary>
    public string? NormasCitadasJson { get; }

    /// <summary>Evidências do dossiê que sustentam a recomendação (arquivo + trecho curto).</summary>
    public string? EvidenciasDossie { get; }

    /// <summary>Contradições/divergências encontradas no dossiê (ex.: depoimento x documento).</summary>
    public string? Contradicoes { get; }

    /// <summary>Lacunas do dossiê que impedem afirmar a elegibilidade.</summary>
    public string? Lacunas { get; }

    /// <summary>Indica se a base de normas foi consultada com sucesso nesta avaliação.</summary>
    public bool UsouRag { get; }

    /// <summary>Quantidade de trechos de norma efetivamente usados no prompt.</summary>
    public int TrechosConsultados { get; }

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
        string origem,
        string? motivoClasse = null,
        string? normasCitadasJson = null,
        string? evidenciasDossie = null,
        string? contradicoes = null,
        string? lacunas = null,
        bool usouRag = false,
        int trechosConsultados = 0)
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

        // A classe vem do agente (qualitativa). Se ele não informar, cai na faixa por nota (plano B).
        var classe = NormalizarClasse(recomendacao)
            ?? (string.IsNullOrWhiteSpace(recomendacao) ? ClassificarPorNota(NotaTotal) : recomendacao.Trim());
        Recomendacao = classe;
        ClasseRecomendada = classe;

        JustificativasJson = string.IsNullOrWhiteSpace(justificativasJson) ? "{}" : justificativasJson;
        Origem = string.IsNullOrWhiteSpace(origem) ? "desconhecida" : origem.Trim();

        MotivoClasse = Limpar(motivoClasse);
        NormasCitadasJson = Limpar(normasCitadasJson);
        EvidenciasDossie = Limpar(evidenciasDossie);
        Contradicoes = Limpar(contradicoes);
        Lacunas = Limpar(lacunas);
        UsouRag = usouRag;
        TrechosConsultados = Math.Max(0, trechosConsultados);
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

    /// <summary>
    /// Converte rótulos livres (com acento, maiúsculas, hífen, nome do enum ou rótulo do desafio)
    /// em uma das quatro classes canônicas. Devolve <c>null</c> quando não reconhece.
    /// </summary>
    public static string? NormalizarClasse(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return null;

        var chave = Chave(valor);
        return chave switch
        {
            "elegivel" => "Elegível",
            "comressalvas" or "ressalvas" or "comressalva" => "Com ressalvas",
            "naoelegivel" => "Não elegível",
            "evidenciainsuficiente" or "insuficiente" => "Evidência insuficiente",
            _ => null
        };
    }

    /// <summary>Nome do enum de <see cref="StatusProjeto"/> correspondente à classe (para exibição).</summary>
    public static StatusProjeto? ClasseParaStatus(string? classe) => classe switch
    {
        "Elegível" => StatusProjeto.Elegivel,
        "Com ressalvas" => StatusProjeto.ComRessalvas,
        "Não elegível" => StatusProjeto.NaoElegivel,
        "Evidência insuficiente" => StatusProjeto.EvidenciaInsuficiente,
        _ => null
    };

    private static string Chave(string valor)
    {
        var decomposto = valor.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var limpo = new StringBuilder(decomposto.Length);
        foreach (var c in decomposto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                limpo.Append(c);
        }

        return new string(limpo.ToString().Where(char.IsLetterOrDigit).ToArray());
    }

    private static string? Limpar(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

    private static int Validar(int nota, string campo)
    {
        if (nota < 0 || nota > CriteriosFrascati.NotaMaximaPorCriterio)
            throw new ArgumentOutOfRangeException(campo, $"Cada critério vale de 0 a {CriteriosFrascati.NotaMaximaPorCriterio}.");
        return nota;
    }
}
