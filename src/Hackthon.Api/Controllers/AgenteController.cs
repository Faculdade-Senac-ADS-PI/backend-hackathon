using Hackthon.Api.Auth;
using Hackthon.Api.Contratos;
using Hackthon.Api.Http;
using Hackthon.Application.Agente;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hackthon.Api.Controllers;

[ApiController]
[Authorize(Policy = Politicas.AnalistaOuAdm)]
[Route("api/agente")]
public sealed class AgenteController : ControllerBase
{
    [HttpGet("contrato")]
    public ActionResult<ContratoAgente> Contrato([FromServices] IAgenteElegibilidade agente) =>
        Ok(agente.DescreverContrato());

    [HttpPost("consulta")]
    public async Task<IActionResult> Consultar(
        [FromBody] ConsultaAgenteRequest? pedido,
        [FromServices] IAgenteElegibilidade agente,
        CancellationToken ct)
    {
        if (pedido is null)
        {
            return new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Pedido inválido",
                Detail = "Envie a pergunta do analista."
            })
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        var resultado = await agente.ConsultarAsync(new ConsultaAgenteComando(pedido.ProjetoId, pedido.Pergunta), ct);
        if (!resultado.Sucesso || resultado.Valor is null)
            return ResultadoHttp.Problema(resultado);

        return Ok(resultado.Valor);
    }
}
