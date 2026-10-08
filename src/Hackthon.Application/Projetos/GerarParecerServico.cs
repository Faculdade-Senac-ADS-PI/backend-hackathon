using System.Text;
using Hackthon.Application.Acesso;
using Hackthon.Application.Comum;
using Hackthon.Domain.Projetos;
using Hackthon.Domain.Usuarios;

namespace Hackthon.Application.Projetos;

/// <summary>
/// Monta o parecer preliminar de enquadramento em Markdown a partir da classificação registrada
/// pelo analista, respondendo às três perguntas de rastreabilidade do desafio:
/// qual regra foi usada · qual informação do projeto sustenta a decisão · quem decidiu e quando.
/// </summary>
public sealed class GerarParecerServico : IGerarParecer
{
    private const string RegraPadrao =
        "Manual de Frascati (5 critérios) · Lei nº 11.196/2005 · Decreto nº 5.798/2006 · IN RFB nº 1.187/2011";

    private readonly IProjetoRepositorio _projetos;
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IRelogio _relogio;

    public GerarParecerServico(IProjetoRepositorio projetos, IUsuarioRepositorio usuarios, IRelogio relogio)
    {
        _projetos = projetos;
        _usuarios = usuarios;
        _relogio = relogio;
    }

    public async Task<Resultado<ParecerResposta>> ExecutarAsync(Guid projetoId, CancellationToken ct)
    {
        var projeto = await _projetos.ObterPorIdAsync(projetoId, ct);
        if (projeto is null)
            return Falha(404, "Projeto não encontrado", "O projeto solicitado não existe.");

        if (! EhClassificacaoFinal(projeto.Status))
            return Falha(409, "Parecer indisponível",
                "O parecer é gerado após a deliberação do analista (Elegível, Com ressalvas, Não elegível ou Evidência insuficiente).");

        var proponente = await _usuarios.ObterPorIdAsync(projeto.ProponenteId, ct);
        var analista = projeto.AnalistaId is { } analistaId
            ? await _usuarios.ObterPorIdAsync(analistaId, ct)
            : null;

        var markdown = Montar(projeto, proponente, analista);

        return Resultado<ParecerResposta>.Ok(new ParecerResposta(
            projeto.Id,
            projeto.Protocolo,
            projeto.Titulo,
            Rotulo(projeto.Status),
            markdown,
            _relogio.Agora()));
    }

    private static bool EhClassificacaoFinal(StatusProjeto status) =>
        status is StatusProjeto.Elegivel or StatusProjeto.ComRessalvas
            or StatusProjeto.NaoElegivel or StatusProjeto.EvidenciaInsuficiente;

    internal static string Rotulo(StatusProjeto status) => status switch
    {
        StatusProjeto.Elegivel => "Elegível",
        StatusProjeto.ComRessalvas => "Com ressalvas",
        StatusProjeto.NaoElegivel => "Não elegível",
        StatusProjeto.EvidenciaInsuficiente => "Evidência insuficiente",
        _ => status.ToString()
    };

    private static string Montar(Projeto projeto, Usuario? proponente, Usuario? analista)
    {
        var md = new StringBuilder();

        md.AppendLine("# Parecer Preliminar de Enquadramento — Lei do Bem");
        md.AppendLine();
        md.AppendLine($"**Instituição:** Banco do Nordeste — Protótipo STS 2026  ");
        md.AppendLine($"**Protocolo:** {projeto.Protocolo}  ");
        md.AppendLine($"**Projeto:** {projeto.Titulo}  ");
        md.AppendLine($"**Setor:** {projeto.Setor}  ");
        md.AppendLine($"**Proponente:** {proponente?.Nome ?? "Não identificado"} ({proponente?.Email ?? "—"})  ");
        md.AppendLine($"**Versão do dossiê analisada:** {projeto.VersaoEnvio}ª  ");
        md.AppendLine($"**Classificação:** **{Rotulo(projeto.Status)}**");
        md.AppendLine();

        md.AppendLine("## 1. Fundamentação da conclusão");
        md.AppendLine();
        md.AppendLine(projeto.JustificativaAnalista ?? "—");
        md.AppendLine();

        md.AppendLine("## 2. Avaliação dos cinco critérios (Manual de Frascati)");
        md.AppendLine();
        var justificativas = LerJustificativas(projeto.JustificativasIaJson);
        var notas = new Dictionary<string, int?>
        {
            [CriteriosFrascati.Novidade] = projeto.NotaNovidade,
            [CriteriosFrascati.Criatividade] = projeto.NotaCriatividade,
            [CriteriosFrascati.Sistematico] = projeto.NotaSistematico,
            [CriteriosFrascati.Transferibilidade] = projeto.NotaTransferibilidade,
            [CriteriosFrascati.Incerteza] = projeto.NotaIncerteza
        };
        var indice = 1;
        foreach (var criterio in CriteriosFrascati.Todos)
        {
            var nota = notas[criterio.Chave];
            var notaTexto = nota is null ? "—" : $"{nota}/{CriteriosFrascati.NotaMaximaPorCriterio}";
            md.AppendLine($"{indice}. **{criterio.Nome} — {notaTexto}**");
            var justificativa = justificativas.GetValueOrDefault(criterio.Chave);
            if (!string.IsNullOrWhiteSpace(justificativa))
                md.AppendLine($"   - {justificativa}");
            indice++;
        }

        md.AppendLine();
        if (projeto.Status == StatusProjeto.ComRessalvas)
        {
            md.AppendLine("## 3. Recorte sustentado, limitação e evidência necessária");
            md.AppendLine();
            md.AppendLine($"- **Recorte sustentado:** {Texto(projeto.RecorteSustentado)}");
            md.AppendLine($"- **Limitação específica:** {Texto(projeto.LimitacaoTecnica)}");
            md.AppendLine($"- **Evidência necessária para resolver a ressalva:** {Texto(projeto.EvidenciaNecessaria)}");
            md.AppendLine();
        }

        if (projeto.Status == StatusProjeto.EvidenciaInsuficiente)
        {
            md.AppendLine("## 3. Elo ausente e evidências adicionais");
            md.AppendLine();
            md.AppendLine($"- **Elo ausente:** {Texto(projeto.EloAusente)}");
            md.AppendLine($"- **Evidências adicionais a solicitar:** {Texto(projeto.EvidenciasAdicionais)}");
            md.AppendLine();
        }

        md.AppendLine("## 4. Evidências utilizadas (por ID)");
        md.AppendLine();
        md.AppendLine(Lista(projeto.EvidenciasUtilizadas));
        md.AppendLine();

        md.AppendLine("## 5. Evidências contrárias, contraditórias e divergências entre fontes");
        md.AppendLine();
        md.AppendLine(Lista(projeto.EvidenciasContrarias));
        md.AppendLine();

        md.AppendLine("## 6. Lacunas identificadas");
        md.AppendLine();
        md.AppendLine(Lista(projeto.Lacunas));
        md.AppendLine();

        if (!string.IsNullOrWhiteSpace(projeto.RecomendacaoIa) || !string.IsNullOrWhiteSpace(projeto.ResumoIa))
        {
            md.AppendLine("## 7. Apoio do agente de IA (não substitui o julgamento do analista)");
            md.AppendLine();
            if (!string.IsNullOrWhiteSpace(projeto.RecomendacaoIa))
                md.AppendLine($"- **Recomendação técnica da IA:** {projeto.RecomendacaoIa}" +
                              (projeto.NotaIa is null ? "" : $" (nota {projeto.NotaIa}/100)"));
            if (!string.IsNullOrWhiteSpace(projeto.OrigemAnaliseIa))
                md.AppendLine($"- **Origem:** {projeto.OrigemAnaliseIa}");
            if (!string.IsNullOrWhiteSpace(projeto.ParecerIa))
            {
                md.AppendLine();
                md.AppendLine($"> {projeto.ParecerIa}");
            }

            md.AppendLine();
        }

        md.AppendLine("## Rastreabilidade da decisão");
        md.AppendLine();
        var regraAplicada = string.IsNullOrWhiteSpace(projeto.RegraAplicada) ? RegraPadrao : projeto.RegraAplicada;
        md.AppendLine($"- **Regra aplicada:** {regraAplicada}");
        md.AppendLine("- **Informação do projeto que sustenta a decisão:** evidências por ID da seção 4 e avaliação dos critérios da seção 2, extraídas do dossiê (" +
                      $"{projeto.NomeArquivoOriginal}).");
        md.AppendLine($"- **Quem decidiu:** {analista?.Nome ?? "Analista não identificado"} ({RotuloPapel(analista)})");
        md.AppendLine($"- **Quando decidiu:** {FormatarData(projeto.DecididoEm)}");
        md.AppendLine($"- **Base normativa:** {RegraPadrao}");
        md.AppendLine();

        md.AppendLine("---");
        md.AppendLine();
        md.AppendLine("*Documento preliminar gerado pelo sistema de apoio à decisão. A conclusão final é do analista;* " +
                      "*o parecer deve ser lido junto do dossiê original antes de qualquer uso perante fiscalização.*");

        return md.ToString();
    }

    private static string Texto(string? valor) => string.IsNullOrWhiteSpace(valor) ? "—" : valor;

    private static string Lista(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return "_Nenhuma registrada._";

        var itens = valor
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(linha => linha.Trim().TrimEnd(','))
            .Where(linha => linha.Length > 0)
            .ToList();

        return itens.Count == 0
            ? "_Nenhuma registrada._"
            : string.Join(Environment.NewLine, itens.Select(item => $"- {item}"));
    }

    private static string RotuloPapel(Usuario? analista) => analista?.Papel.ToString() ?? "—";

    private static string FormatarData(DateTimeOffset? momento) =>
        momento?.ToOffset(momento.Value.Offset).ToString("dd/MM/yyyy 'às' HH:mm") ?? "—";

    private static Dictionary<string, string> LerJustificativas(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new Dictionary<string, string>();
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                ?? new Dictionary<string, string>();
        }
        catch (System.Text.Json.JsonException)
        {
            return new Dictionary<string, string>();
        }
    }

    private static Resultado<ParecerResposta> Falha(int status, string titulo, string detalhe) =>
        Resultado<ParecerResposta>.Falha(status, titulo, detalhe);
}
