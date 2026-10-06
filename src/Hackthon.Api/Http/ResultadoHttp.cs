using Hackthon.Application.Comum;
using Microsoft.AspNetCore.Mvc;

namespace Hackthon.Api.Http;

public static class ResultadoHttp
{
    public static IActionResult Problema<T>(Resultado<T> resultado) =>
        new ObjectResult(new ProblemDetails
        {
            Status = resultado.Status,
            Title = resultado.Titulo,
            Detail = resultado.Detalhe
        })
        {
            StatusCode = resultado.Status
        };
}
