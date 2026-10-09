using System.Security.Cryptography;
using System.Text;
using Hackthon.Application.Comum;
using Microsoft.Extensions.Logging;

namespace Hackthon.Application.Normas;

/// <summary>
/// Indexa a base de normas: lê os documentos da pasta de conhecimento, divide em trechos,
/// gera os vetores e grava (substituindo) no banco. Pode ser reexecutada sem duplicar trechos.
/// </summary>
public sealed class IndexarBaseServico : IIndexarBase
{
    private readonly IFonteConhecimento _fonte;
    private readonly IGeradorEmbedding _embedding;
    private readonly INormaRepositorio _repositorio;
    private readonly ILogger<IndexarBaseServico> _logger;

    public IndexarBaseServico(
        IFonteConhecimento fonte,
        IGeradorEmbedding embedding,
        INormaRepositorio repositorio,
        ILogger<IndexarBaseServico> logger)
    {
        _fonte = fonte;
        _embedding = embedding;
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<Resultado<EstadoBaseResposta>> ExecutarAsync(CancellationToken ct)
    {
        IReadOnlyList<DocumentoFonte> documentos;
        try
        {
            documentos = await _fonte.ListarAsync(ct);
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            _logger.LogError(ex, "Falha ao ler a pasta de conhecimento {Pasta}.", _fonte.Pasta);
            return Resultado<EstadoBaseResposta>.Falha(
                500,
                "Falha ao ler a base",
                $"Não foi possível ler a pasta '{_fonte.Pasta}': {ex.Message}");
        }

        if (documentos.Count == 0)
        {
            return Resultado<EstadoBaseResposta>.Falha(
                409,
                "Base de conhecimento vazia",
                $"Nenhum documento PDF, MD ou TXT encontrado em '{_fonte.Pasta}'. " +
                "Coloque os documentos oficiais (leis, guias, manual) nessa pasta e tente novamente.");
        }

        var totalTrechos = 0;
        var documentosIndexados = new List<string>();

        foreach (var documento in documentos)
        {
            ct.ThrowIfCancellationRequested();

            var brutos = DivisorDocumentos.Dividir(documento.Texto);
            if (brutos.Count == 0)
            {
                _logger.LogWarning("Documento {Documento} não gerou trechos (texto vazio ou não legível).", documento.Nome);
                continue;
            }

            IReadOnlyList<float[]> vetores;
            try
            {
                vetores = await _embedding.GerarLoteAsync(brutos.Select(t => t.Texto).ToList(), ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao gerar embeddings de {Documento}.", documento.Nome);
                return Resultado<EstadoBaseResposta>.Falha(
                    503,
                    "Provedor de IA indisponível",
                    $"Não foi possível gerar os vetores de '{documento.Nome}'. Confira a chave/provedor de IA " +
                    $"(AgenteIa__ApiKey) e tente novamente. Detalhe: {ex.Message}");
            }

            if (vetores.Count != brutos.Count)
            {
                return Resultado<EstadoBaseResposta>.Falha(
                    502,
                    "Resposta inválida do provedor de IA",
                    $"O provedor devolveu {vetores.Count} vetores para {brutos.Count} trechos de '{documento.Nome}'.");
            }

            var trechos = new List<TrechoParaIndexar>(brutos.Count);
            for (var i = 0; i < brutos.Count; i++)
            {
                var texto = brutos[i].Texto;
                trechos.Add(new TrechoParaIndexar(
                    brutos[i].Referencia,
                    brutos[i].Pagina,
                    brutos[i].Ordem,
                    texto,
                    Hash(texto),
                    vetores[i]));
            }

            await _repositorio.SubstituirDocumentoAsync(documento.Nome, documento.Caminho, trechos, ct);
            documentosIndexados.Add(documento.Nome);
            totalTrechos += trechos.Count;
            _logger.LogInformation("Indexados {Total} trechos de {Documento}.", trechos.Count, documento.Nome);
        }

        var estado = await MontarEstadoAsync(
            $"Base atualizada: {documentosIndexados.Count} documento(s) e {totalTrechos} trecho(s).", ct);
        return Resultado<EstadoBaseResposta>.Ok(estado);
    }

    private async Task<EstadoBaseResposta> MontarEstadoAsync(string mensagem, CancellationToken ct)
    {
        var contagens = await _repositorio.ContarPorDocumentoAsync(ct);
        var total = contagens.Sum(c => c.Trechos);
        return new EstadoBaseResposta(_fonte.Pasta, total > 0, total, contagens, mensagem);
    }

    internal static string Hash(string texto)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(texto));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
