using System.Net;
using Microsoft.Extensions.Logging;

namespace Hackthon.Infrastructure.Agente;

/// <summary>
/// Reexecuta chamadas HTTP em falhas transitórias do provedor de IA (429 e 5xx) com espera
/// crescente. Evita que uma sobrecarga momentânea derrube a avaliação para o modo heurístico
/// ou a consulta para a lista de trechos. A requisição é recriada a cada tentativa, pois um
/// <see cref="HttpRequestMessage"/> não pode ser reenviado.
/// </summary>
internal static class RetentativaHttp
{
    private static readonly TimeSpan[] Esperas =
    {
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10)
    };

    public static async Task<HttpResponseMessage> EnviarAsync(
        HttpClient http,
        Func<HttpRequestMessage> criarRequisicao,
        int tentativas,
        ILogger logger,
        string contexto,
        CancellationToken ct)
    {
        var maximo = Math.Max(1, tentativas);
        for (var tentativa = 1; ; tentativa++)
        {
            using var requisicao = criarRequisicao();
            var resposta = await http.SendAsync(requisicao, ct);

            if (!Transitorio(resposta.StatusCode) || tentativa >= maximo)
                return resposta;

            logger.LogWarning(
                "{Contexto}: provedor respondeu {Status} (tentativa {Tentativa}/{Maximo}); aguardando para tentar de novo.",
                contexto, (int)resposta.StatusCode, tentativa, maximo);
            resposta.Dispose();
            await Task.Delay(Esperas[Math.Min(tentativa - 1, Esperas.Length - 1)], ct);
        }
    }

    private static bool Transitorio(HttpStatusCode status) =>
        status is HttpStatusCode.TooManyRequests
            or HttpStatusCode.InternalServerError
            or HttpStatusCode.BadGateway
            or HttpStatusCode.ServiceUnavailable
            or HttpStatusCode.GatewayTimeout;
}
