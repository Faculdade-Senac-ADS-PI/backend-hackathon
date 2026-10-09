using Hackthon.Api.Auth;
using Hackthon.Api.Http;
using Hackthon.Application.Normas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hackthon.Api.Controllers;

/// <summary>
/// Base de normas (RAG): preparação e estado. A preparação exige papel de ADM;
/// a consulta de estado é liberada para analistas/adm (transparência das fontes).
/// </summary>
[ApiController]
[Authorize]
[Route("api/normas")]
public sealed class NormasController : ControllerBase
{
    /// <summary>Prepara/reindexa a base a partir da pasta de conhecimento (idempotente).</summary>
    [HttpPost("reindexar")]
    [Authorize(Policy = Politicas.SomenteAdm)]
    public async Task<IActionResult> Reindexar([FromServices] IIndexarBase servico, CancellationToken ct)
    {
        var resultado = await servico.ExecutarAsync(ct);
        if (!resultado.Sucesso || resultado.Valor is null)
            return ResultadoHttp.Problema(resultado);

        return Ok(resultado.Valor);
    }

    /// <summary>Estado da base (documentos e trechos indexados).</summary>
    [HttpGet("estado")]
    [Authorize(Policy = Politicas.AnalistaOuAdm)]
    public async Task<ActionResult<EstadoBaseResposta>> Estado([FromServices] IConsultarBase servico, CancellationToken ct) =>
        Ok(await servico.ObterAsync(ct));
}