using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hackthon.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/saude")]
public sealed class SaudeController : ControllerBase
{
    [HttpGet]
    public IActionResult Obter() => Ok(new { status = "ok", servico = "hackthon-api" });
}
