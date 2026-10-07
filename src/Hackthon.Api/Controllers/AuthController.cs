using Hackthon.Api.Auth;
using Hackthon.Api.Contratos;
using Hackthon.Api.Http;
using Hackthon.Application.Acesso;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hackthon.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar(
        [FromBody] RegistrarRequest? pedido,
        [FromServices] IRegistrarUsuario caso,
        CancellationToken ct)
    {
        if (pedido is null)
            return ProblemaVazio();

        if (!string.IsNullOrWhiteSpace(pedido.Papel))
        {
            var papelLimpo = pedido.Papel.Trim();
            if (papelLimpo.Equals("Adm", StringComparison.OrdinalIgnoreCase)
                || papelLimpo.Equals("Administrador", StringComparison.OrdinalIgnoreCase)
                || papelLimpo.Equals("Analista", StringComparison.OrdinalIgnoreCase))
            {
                return new ObjectResult(new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Cadastro recusado",
                    Detail = "Apenas Proponente e Assistente podem se cadastrar publicamente. Analista e ADM nascem com a administração."
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }

        var resultado = await caso.ExecutarAsync(
            new RegistrarUsuarioComando(pedido.Nome, pedido.Email, pedido.CpfCnpj, pedido.Senha, pedido.Papel),
            ct);
        if (!resultado.Sucesso || resultado.Valor is null)
            return ResultadoHttp.Problema(resultado);

        return Created("/api/auth/eu", resultado.Valor);
    }

    [AllowAnonymous]
    [HttpPost("entrar")]
    public async Task<IActionResult> Entrar(
        [FromBody] EntrarRequest? pedido,
        [FromServices] IEntrar caso,
        CancellationToken ct)
    {
        if (pedido is null)
            return ProblemaVazio();

        var resultado = await caso.ExecutarAsync(new EntrarComando(pedido.Email, pedido.Senha), ct);
        if (!resultado.Sucesso || resultado.Valor is null)
            return ResultadoHttp.Problema(resultado);

        return Ok(resultado.Valor);
    }

    [Authorize]
    [HttpGet("eu")]
    public async Task<IActionResult> Eu([FromServices] IObterPerfil caso, CancellationToken ct)
    {
        if (!UsuarioAtual.TentarId(User, out var id))
            return Unauthorized();

        var resultado = await caso.ExecutarAsync(id, ct);
        if (!resultado.Sucesso || resultado.Valor is null)
            return ResultadoHttp.Problema(resultado);

        return Ok(resultado.Valor);
    }

    private ObjectResult ProblemaVazio() =>
        new(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Pedido inválido",
            Detail = "Envie o corpo da requisição."
        })
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
}
