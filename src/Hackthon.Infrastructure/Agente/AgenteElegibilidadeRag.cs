using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Hackthon.Application.Agente;
using Hackthon.Application.Comum;
using Hackthon.Application.Normas;
using Hackthon.Application.Projetos;
using Hackthon.Infrastructure.Normas;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hackthon.Infrastructure.Agente;

/// <summary>
/// Agente de apoio ao analista com base no RAG: recupera trechos das normas da Lei do Bem
/// a partir de uma pergunta em linguagem natural e responde citando as fontes.
/// Nunca decide sozinho nem fecha o enquadramento — devolve subsídios com as referências.
/// </summary>
public sealed class AgenteElegibilidadeRag : IAgenteElegibilidade
{
    private readonly IRecuperadorTrechos _recuperador;
    private readonly IProjetoRepositorio _projetos;
    private readonly AgenteIaOpcoes _ia;
    private readonly RagOpcoes _rag;
    private readonly HttpClient _http;
    private readonly ILogger<AgenteElegibilidadeRag> _logger;

    public AgenteElegibilidadeRag(
        IRecuperadorTrechos recuperador,
        IProjetoRepositorio projetos,
        IOptions<AgenteIaOpcoes> ia,
        IOptions<RagOpcoes> rag,
        HttpClient http,
        ILogger<AgenteElegibilidadeRag> logger)
    {
        _recuperador = recuperador;
        _projetos = projetos;
        _ia = ia.Value;
        _rag = rag.Value;
        _http = http;
        _logger = logger;
    }

    public ContratoAgente DescreverContrato() => new(
        Estado: "operacional",
        Responsabilidade: "Apoiar o analista com trechos das normas da Lei do Bem. Não decidir se o projeto é P&D.",
        ComoPlugar: "Coloque os documentos oficiais em 'base-conhecimento', indexe pela rota /api/normas/reindexar " +
                    "e consulte com uma pergunta. As respostas citam as fontes recuperadas.",
        EntradasPrevistas: ["projetoId", "pergunta do analista", "trechos de norma recuperados"],
        Saida: "Resposta em texto livre citando as fontes como [n], sem decidir o enquadramento.");

    public async Task<Resultado<ConsultaAgenteResposta>> ConsultarAsync(ConsultaAgenteComando comando, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(comando.Pergunta))
        {
            return Resultado<ConsultaAgenteResposta>.Falha(
                400,
                "Pergunta obrigatória",
                "Informe a pergunta do analista sobre as normas da Lei do Bem.");
        }

        IReadOnlyList<TrechoRecuperado> trechos;
        try
        {
            trechos = await _recuperador.BuscarAsync(comando.Pergunta!, _rag.TrechosPorConsulta, ct);
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Falha ao recuperar trechos de norma para a consulta do analista.");
            trechos = Array.Empty<TrechoRecuperado>();
        }

        string? textoProjeto = null;
        if (comando.ProjetoId is Guid projetoId)
        {
            var projeto = await _projetos.ObterPorIdAsync(projetoId, ct);
            if (projeto is not null)
                textoProjeto = Recortar(projeto.TextoExtraido, 8000);
        }

        var citacoes = trechos.Select(ParaCitacao).ToList();

        if (trechos.Count == 0 && string.IsNullOrWhiteSpace(textoProjeto))
        {
            return Resultado<ConsultaAgenteResposta>.Ok(new ConsultaAgenteResposta(
                "Não há trechos de norma indexados nem conteúdo de projeto para responder. " +
                "Coloque os documentos oficiais na pasta de conhecimento e execute a preparação da base (/api/normas/reindexar).",
                Array.Empty<TrechoNorma>(),
                false));
        }

        if (!_ia.UsaLlm || string.IsNullOrWhiteSpace(_ia.ApiKey))
        {
            var resposta = citacoes.Count == 0
                ? "A base de normas não devolveu trechos relevantes para a pergunta."
                : "Sem modelo de linguagem configurado para redigir a resposta. Trechos relevantes encontrados:";
            return Resultado<ConsultaAgenteResposta>.Ok(new ConsultaAgenteResposta(resposta, citacoes, false));
        }

        try
        {
            var resposta = await PerguntarAsync(comando.Pergunta!, textoProjeto, trechos, ct);
            return Resultado<ConsultaAgenteResposta>.Ok(new ConsultaAgenteResposta(resposta, citacoes, false));
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Falha ao redigir a resposta com o modelo; devolvendo os trechos recuperados.");
            return Resultado<ConsultaAgenteResposta>.Ok(new ConsultaAgenteResposta(
                "Não foi possível redigir a resposta com o modelo. Segue a lista de trechos recuperados para conferência.",
                citacoes,
                false));
        }
    }

    private async Task<string> PerguntarAsync(
        string pergunta,
        string? textoProjeto,
        IReadOnlyList<TrechoRecuperado> trechos,
        CancellationToken ct)
    {
        var instrucoes = new StringBuilder();
        instrucoes.AppendLine("Você é um assistente técnico que apoia analistas da Lei do Bem (Lei 11.196/2005) e do Manual de Frascati.");
        instrucoes.AppendLine("Responda em português do Brasil, de forma objetiva, e SEMPRE cite as fontes usando os números entre colchetes [n].");
        instrucoes.AppendLine("Use exclusivamente os trechos de norma fornecidos. Se algo não estiver nos trechos, diga que não há base para afirmar.");
        instrucoes.AppendLine("Você NÃO decide o enquadramento; apenas apresenta o que as normas dizem e como se aplicam ao caso.");

        var usuario = new StringBuilder();
        usuario.AppendLine($"Pergunta do analista: {pergunta}");
        if (!string.IsNullOrWhiteSpace(textoProjeto))
            usuario.AppendLine($"\nTrechos do dossiê do projeto:\n<<<\n{textoProjeto}\n>>>");

        if (trechos.Count > 0)
        {
            usuario.AppendLine("\nTRECHOS DE NORMA RECUPERADOS:");
            for (var i = 0; i < trechos.Count; i++)
                usuario.AppendLine($"[{i + 1}] ({Rotulo(trechos[i])}): {Recortar(trechos[i].Texto, 900)}");
        }
        else
        {
            usuario.AppendLine("\nNenhum trecho de norma foi recuperado; não invente referências.");
        }

        var corpo = new Dictionary<string, object>
        {
            ["model"] = _ia.Modelo,
            ["temperature"] = _ia.Temperatura,
            ["messages"] = new object[]
            {
                new { role = "system", content = instrucoes.ToString() },
                new { role = "user", content = usuario.ToString() }
            }
        };

        var url = new Uri(new Uri(_ia.UrlBase), "chat/completions");
        using var resposta = await RetentativaHttp.EnviarAsync(
            _http,
            () =>
            {
                var requisicao = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = JsonContent.Create(corpo)
                };
                if (!string.IsNullOrWhiteSpace(_ia.ApiKey))
                    requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _ia.ApiKey);
                return requisicao;
            },
            _ia.Tentativas,
            _logger,
            "Consulta às normas",
            ct);
        var bruto = await resposta.Content.ReadAsStringAsync(ct);
        if (!resposta.IsSuccessStatusCode)
            throw new HttpRequestException($"Provedor de IA respondeu {(int)resposta.StatusCode}: {Recortar(bruto, 300)}");

        using var envelope = JsonDocument.Parse(bruto);
        return envelope.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()
            ?? "O modelo não devolveu conteúdo.";
    }

    private static TrechoNorma ParaCitacao(TrechoRecuperado trecho) =>
        new(trecho.Documento, Recortar(trecho.Texto, 800), Rotulo(trecho));

    private static string Rotulo(TrechoRecuperado trecho)
    {
        var referencia = string.IsNullOrWhiteSpace(trecho.Referencia) ? null : trecho.Referencia;
        var pagina = trecho.Pagina is int p ? $"p. {p}" : null;
        var complemento = string.Join(", ", new[] { referencia, pagina }.Where(x => x is not null));
        return complemento.Length == 0 ? trecho.Documento : $"{trecho.Documento} — {complemento}";
    }

    private static string Recortar(string texto, int limite) =>
        texto.Length <= limite ? texto : texto[..limite] + "...";
}
