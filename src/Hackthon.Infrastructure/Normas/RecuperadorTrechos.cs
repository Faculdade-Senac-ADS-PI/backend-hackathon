using Hackthon.Application.Normas;
using Microsoft.Extensions.Logging;

namespace Hackthon.Infrastructure.Normas;

/// <summary>
/// Recupera trechos de norma relevantes: gera o vetor da consulta, busca os mais próximos
/// (cosseno), descarta os fracos e remove repetições.
/// </summary>
public sealed class RecuperadorTrechos : IRecuperadorTrechos
{
    private readonly IGeradorEmbedding _embedding;
    private readonly INormaRepositorio _repositorio;
    private readonly RagOpcoes _rag;
    private readonly ILogger<RecuperadorTrechos> _logger;

    public RecuperadorTrechos(
        IGeradorEmbedding embedding,
        INormaRepositorio repositorio,
        Microsoft.Extensions.Options.IOptions<RagOpcoes> rag,
        ILogger<RecuperadorTrechos> logger)
    {
        _embedding = embedding;
        _repositorio = repositorio;
        _rag = rag.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<TrechoRecuperado>> BuscarAsync(string consulta, int? quantidade, CancellationToken ct)
    {
        if (!_rag.Habilitado || string.IsNullOrWhiteSpace(consulta))
            return Array.Empty<TrechoRecuperado>();

        if (!await _repositorio.HaTrechosAsync(ct))
            return Array.Empty<TrechoRecuperado>();

        var desejados = Math.Max(1, quantidade ?? _rag.TrechosPorConsulta);
        var vetor = await _embedding.GerarAsync(consulta, ct);

        // Busca um pouco além e depois filtra por similaridade e remove repetições.
        var candidatos = await _repositorio.BuscarAsync(vetor, desejados * 2, ct);

        var vistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var selecionados = new List<TrechoRecuperado>();
        foreach (var trecho in candidatos)
        {
            if (trecho.Similaridade < _rag.LimiarSimilaridade)
                continue;
            if (!vistos.Add(trecho.Texto.Trim()))
                continue;

            selecionados.Add(trecho);
            if (selecionados.Count >= desejados)
                break;
        }

        _logger.LogDebug("Recuperados {Total} trechos para uma consulta de {Desejados} desejados.", selecionados.Count, desejados);
        return selecionados;
    }

    public async Task<bool> BaseDisponivelAsync(CancellationToken ct)
    {
        if (!_rag.Habilitado)
            return false;

        try
        {
            return await _repositorio.HaTrechosAsync(ct);
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Não foi possível verificar a base de normas.");
            return false;
        }
    }
}
