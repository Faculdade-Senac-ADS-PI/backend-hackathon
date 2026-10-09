using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Hackthon.Application.Projetos;
using Hackthon.Domain.Projetos;

namespace Hackthon.Infrastructure.Agente;

/// <summary>
/// Motor determinístico de apoio: procura evidências textuais de cada um dos 5 critérios de Frascati.
/// Usado quando não há LLM configurado ou quando o provedor de IA falha. Não substitui o analista.
/// </summary>
public sealed partial class AnalisadorElegibilidadeHeuristico : IAnalisadorElegibilidade
{
    private static readonly Dictionary<string, string[]> Indicadores = new()
    {
        [CriteriosFrascati.Novidade] =
        [
            "inedit", "inovac", "inovador", "novo processo", "novo produto", "nova abordagem", "nova tecnologia",
            "nova solucao", "pioneir", "estado da arte", "patente", "avanco", "nao existia", "nao havia",
            "prototipo", "original", "diferencial", "desenvolvimento de"
        ],
        [CriteriosFrascati.Criatividade] =
        [
            "criativ", "ganho", "reducao de custo", "economia", "produtividade", "eficiencia", "melhoria",
            "retorno", "competitiv", "otimiz", "reduziu", "aumentou", "agrega valor", "abordagem diferente",
            "solucao alternativa", "impacto"
        ],
        [CriteriosFrascati.Sistematico] =
        [
            "metodolog", "planejamento", "cronograma", "etapa", "fase", "teste", "ensaio", "experiment",
            "hipotese", "validac", "registro", "relatorio", "documenta", "indicador", "metrica", "coleta de dados",
            "piloto", "prova de conceito", "iterac", "protocolo"
        ],
        [CriteriosFrascati.Transferibilidade] =
        [
            "replic", "reproduz", "manual", "procedimento", "padroniz", "escalav", "outras areas", "outras unidades",
            "outras empresas", "outras organizacoes", "transfer", "difus", "publicac", "artigo", "capacitac",
            "conhecimento gerado", "aplicavel", "generaliz"
        ],
        [CriteriosFrascati.Incerteza] =
        [
            "incerteza", "risco tecnologico", "risco tecnico", "nao se sabia", "desconhecid", "desafio tecnico",
            "barreira", "limitac", "nao era possivel", "dificuldade", "tentativa", "nao trivial", "complex",
            "investigac", "pesquisa", "viabilidade", "obstaculo", "falha"
        ]
    };

    private static readonly string[] IndiciosRotina =
    [
        "manutencao corretiva", "manutencao preventiva", "atividade rotineira", "rotina operacional", "suporte tecnico",
        "customizacao", "parametrizacao", "configuracao padrao", "atualizacao de versao", "compra de equipamento",
        "aquisicao de software", "migracao de sistema"
    ];

    public Task<AvaliacaoFrascati> AvaliarAsync(PropostaParaAvaliacao proposta, CancellationToken ct) =>
        Task.FromResult(Avaliar(proposta, "heuristico"));

    public AvaliacaoFrascati Avaliar(PropostaParaAvaliacao proposta, string origem = "heuristico")
    {
        var texto = Normalizar(proposta.Texto);
        var rotina = IndiciosRotina.Where(texto.Contains).ToList();
        var notas = new Dictionary<string, int>();
        var justificativas = new Dictionary<string, string>();

        foreach (var criterio in CriteriosFrascati.Todos)
        {
            var encontrados = Indicadores[criterio.Chave]
                .Select(termo => (termo, ocorrencias: Contar(texto, termo)))
                .Where(item => item.ocorrencias > 0)
                .ToList();

            var distintos = encontrados.Count;
            var ocorrencias = encontrados.Sum(item => item.ocorrencias);
            var nota = distintos * 2.5 + Math.Min(ocorrencias, 20) * 0.25;

            var penaliza = criterio.Chave is CriteriosFrascati.Novidade or CriteriosFrascati.Incerteza;
            if (penaliza)
                nota -= rotina.Count * 2;

            notas[criterio.Chave] = (int)Math.Clamp(Math.Round(nota), 0, CriteriosFrascati.NotaMaximaPorCriterio);

            var explicacao = distintos == 0
                ? "Não foram encontradas evidências textuais deste critério no documento."
                : $"Evidências encontradas ({ocorrencias} ocorrência(s)): {string.Join(", ", encontrados.Take(6).Select(e => e.termo))}.";
            if (penaliza && rotina.Count > 0)
                explicacao += $" Indícios de atividade de rotina reduziram a nota: {string.Join(", ", rotina)}.";
            justificativas[criterio.Chave] = explicacao;
        }

        var total = notas.Values.Sum();
        var fortes = CriteriosFrascati.Todos.Where(c => notas[c.Chave] >= 14).Select(c => c.Nome).ToList();
        var fracos = CriteriosFrascati.Todos.Where(c => notas[c.Chave] < 10).Select(c => c.Nome).ToList();

        var parecer = new StringBuilder("Avaliação automática por palavras-chave (sem modelo de linguagem). ");
        parecer.Append(fortes.Count > 0 ? $"Critérios com boa evidência: {string.Join(", ", fortes)}. " : "Nenhum critério com evidência forte. ");
        if (fracos.Count > 0)
            parecer.Append($"Critérios que merecem atenção do analista: {string.Join(", ", fracos)}. ");
        parecer.Append("A decisão final sobre a elegibilidade na Lei do Bem cabe ao analista.");

        // Sem material para concluir: o motor heurístico recomenda "Evidência insuficiente"
        // (elo ausente), em vez de classificar por nota com base em documento quase vazio.
        var recomendacao = total == 0 || proposta.Texto.Trim().Length < 400
            ? "Evidência insuficiente"
            : AvaliacaoFrascati.ClassificarPorNota(total);

        return new AvaliacaoFrascati(
            notas[CriteriosFrascati.Novidade],
            notas[CriteriosFrascati.Criatividade],
            notas[CriteriosFrascati.Sistematico],
            notas[CriteriosFrascati.Transferibilidade],
            notas[CriteriosFrascati.Incerteza],
            GerarResumo(proposta),
            parecer.ToString(),
            recomendacao,
            JsonSerializer.Serialize(justificativas),
            origem);
    }

    /// <summary>Resumo extrativo: as frases do documento com mais evidências dos critérios, na ordem original.</summary>
    private static string GerarResumo(PropostaParaAvaliacao proposta)
    {
        var frases = Sentencas().Split(Espacos().Replace(proposta.Texto, " "))
            .Select((frase, indice) => (frase: frase.Trim(), indice))
            .Where(item => item.frase.Length is >= 60 and <= 400)
            .Select(item => (item.frase, item.indice, peso: Indicadores.Values.SelectMany(t => t).Count(Normalizar(item.frase).Contains)))
            .OrderByDescending(item => item.peso)
            .ThenBy(item => item.indice)
            .Take(4)
            .OrderBy(item => item.indice)
            .Select(item => item.frase.EndsWith('.') ? item.frase : item.frase + ".")
            .ToList();

        var cabecalho = $"Proposta \"{proposta.Titulo}\" do setor {proposta.Setor}.";
        return frases.Count == 0 ? cabecalho : $"{cabecalho} {string.Join(" ", frases)}";
    }

    private static int Contar(string texto, string termo)
    {
        var total = 0;
        var indice = texto.IndexOf(termo, StringComparison.Ordinal);
        while (indice >= 0)
        {
            total++;
            indice = texto.IndexOf(termo, indice + termo.Length, StringComparison.Ordinal);
        }

        return total;
    }

    private static string Normalizar(string texto)
    {
        var decomposto = texto.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var limpo = new StringBuilder(decomposto.Length);
        foreach (var c in decomposto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                limpo.Append(c);
        }

        return Espacos().Replace(limpo.ToString(), " ");
    }

    [GeneratedRegex(@"(?<=[\.\!\?])\s+")]
    private static partial Regex Sentencas();

    [GeneratedRegex(@"\s+")]
    private static partial Regex Espacos();
}
