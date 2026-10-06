using Hackthon.Api.Auth;
using Hackthon.Api.Contratos;
using Hackthon.Api.Http;
using Hackthon.Application.Acesso;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hackthon.Api.Controllers;

[ApiController]
[Authorize(Policy = Politicas.SomenteAdm)]
[Route("api/usuarios")]
public sealed class UsuariosController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar([FromServices] IListarUsuarios caso, CancellationToken ct)
    {
        var usuarios = await caso.ExecutarAsync(ct);
        return Ok(usuarios);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromBody] CriarUsuarioRequest? pedido,
        [FromServices] ICriarUsuario caso,
        CancellationToken ct)
    {
        if (pedido is null)
            return PedidoVazio();
        if (!UsuarioAtual.TentarId(User, out var atorId))
            return Unauthorized();

        var resultado = await caso.ExecutarAsync(
            new CriarUsuarioComando(pedido.Nome, pedido.Email, pedido.Senha, pedido.Papel, atorId),
            ct);
        if (!resultado.Sucesso || resultado.Valor is null)
            return ResultadoHttp.Problema(resultado);

        return Created($"/api/usuarios/{resultado.Valor.Id}", resultado.Valor);
    }

    [HttpPatch("{id:guid}/papel")]
    public async Task<IActionResult> AlterarPapel(
        Guid id,
        [FromBody] AlterarPapelRequest? pedido,
        [FromServices] IAlterarPapel caso,
        CancellationToken ct)
    {
        if (pedido is null)
            return PedidoVazio();
        if (!UsuarioAtual.TentarId(User, out var atorId))
            return Unauthorized();

        var resultado = await caso.ExecutarAsync(new AlterarPapelComando(id, pedido.Papel, atorId), ct);
        if (!resultado.Sucesso || resultado.Valor is null)
            return ResultadoHttp.Problema(resultado);

        return Ok(resultado.Valor);
    }

    [HttpPost("{id:guid}/desativar")]
    public Task<IActionResult> Desativar(Guid id, [FromServices] IAlterarAtivacao caso, CancellationToken ct) =>
        AlterarAtivacao(id, false, caso, ct);

    [HttpPost("{id:guid}/reativar")]
    public Task<IActionResult> Reativar(Guid id, [FromServices] IAlterarAtivacao caso, CancellationToken ct) =>
        AlterarAtivacao(id, true, caso, ct);

    private async Task<IActionResult> AlterarAtivacao(Guid id, bool ativo, IAlterarAtivacao caso, CancellationToken ct)
    {
        if (!UsuarioAtual.TentarId(User, out var atorId))
            return Unauthorized();

        var resultado = await caso.ExecutarAsync(new AlterarAtivacaoComando(id, ativo, atorId), ct);
        if (!resultado.Sucesso || resultado.Valor is null)
            return ResultadoHttp.Problema(resultado);

        return Ok(resultado.Valor);
    }

    private ObjectResult PedidoVazio() =>
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
