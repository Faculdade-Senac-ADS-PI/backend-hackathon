using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Hackthon.Application.Normas;
using Hackthon.Infrastructure.Agente;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hackthon.Infrastructure.Normas;

/// <summary>
/// Gera embeddings via endpoint compatível com OpenAI (POST {UrlBase}embeddings).
/// Usa o mesmo provedor/chave do agente de IA; o modelo e a dimensão ficam em RagOpcoes.
/// </summary>
public sealed class GeradorEmbeddingGemini : IGeradorEmbedding
{
    private readonly HttpClient _http;
    private readonly AgenteIaOpcoes _ia;
    private readonly RagOpcoes _rag;
    private readonly ILogger<GeradorEmbeddingGemini> _logger;

    public GeradorEmbeddingGemini(
        HttpClient http,
        IOptions<AgenteIaOpcoes> ia,
        IOptions<RagOpcoes> rag,
        ILogger<GeradorEmbeddingGemini> logger)
    {
        _http = http;
        _ia = ia.Value;
        _rag = rag.Value;
        _logger = logger;
    }

    public async Task<float[]> GerarAsync(string texto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(texto))
            throw new ArgumentException("Texto vazio não pode ser vetorizado.", nameof(texto));

        var vetores = await GerarLoteAsync(new[] { texto }, ct);
        return vetores[0];
    }

    public async Task<IReadOnlyList<float[]>> GerarLoteAsync(IReadOnlyList<string> textos, CancellationToken ct)
    {
        if (textos.Count == 0)
            return Array.Empty<float[]>();

        if (!_ia.UsaLlm || string.IsNullOrWhiteSpace(_ia.ApiKey))
            throw new InvalidOperationException(
                "O provedor de IA/embeddings não está configurado (AgenteIa). A base de normas não pode ser indexada.");

        var tamanhoLote = Math.Max(1, _rag.TamanhoLote);
        var resultado = new List<float[]>(textos.Count);
        for (var i = 0; i < textos.Count; i += tamanhoLote)
        {
            ct.ThrowIfCancellationRequested();
            var lote = textos.Skip(i).Take(tamanhoLote).ToList();
            resultado.AddRange(await EmbedarLoteAsync(lote, ct));
        }

        return resultado;
    }

    private async Task<IReadOnlyList<float[]>> EmbedarLoteAsync(IReadOnlyList<string> textos, CancellationToken ct)
    {
        var corpo = new Dictionary<string, object>
        {
            ["model"] = _rag.ModeloEmbedding,
            ["input"] = textos,
            ["dimensions"] = _rag.Dimensao
        };

        var url = new Uri(new Uri(_ia.UrlBase), "embeddings");
        using var requisicao = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(corpo)
        };
        if (!string.IsNullOrWhiteSpace(_ia.ApiKey))
            requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _ia.ApiKey);

        using var resposta = await _http.SendAsync(requisicao, ct);
        var bruto = await resposta.Content.ReadAsStringAsync(ct);
        if (!resposta.IsSuccessStatusCode)
            throw new HttpRequestException($"Embeddings responderam {(int)resposta.StatusCode}: {Recortar(bruto, 300)}");

        using var documento = JsonDocument.Parse(bruto);
        var dados = documento.RootElement.GetProperty("data");
        var vetores = new List<float[]>(dados.GetArrayLength());
        foreach (var item in dados.EnumerateArray())
        {
            var embedding = item.GetProperty("embedding");
            var vetor = new float[embedding.GetArrayLength()];
            var j = 0;
            foreach (var valor in embedding.EnumerateArray())
                vetor[j++] = valor.GetSingle();
            vetores.Add(vetor);
        }

        if (vetores.Count != textos.Count)
            _logger.LogWarning("Embeddings devolveram {Recebidos} vetores para {Enviados} textos.", vetores.Count, textos.Count);

        return vetores;
    }

    private static string Recortar(string texto, int limite) =>
        texto.Length <= limite ? texto : texto[..limite] + "...";
}
