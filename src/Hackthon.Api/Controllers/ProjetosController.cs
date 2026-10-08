using Hackthon.Api.Auth;
using Hackthon.Api.Contratos;
using Hackthon.Api.Http;
using Hackthon.Application.Projetos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hackthon.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/projetos")]
public sealed class ProjetosController : ControllerBase
{
    /// <summary>Submissão inicial de proposta em PDF pelo proponente.</summary>
    [HttpPost("upload")]
    [HttpPost]
    [Authorize(Policy = Politicas.SomenteProponente)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Enviar(
        [FromForm] EnviarPropostaUploadRequest? pedido,
        [FromServices] IEnviarProposta caso,
        CancellationToken ct)
    {
        if (pedido is null || pedido.Arquivo is null || pedido.Arquivo.Length == 0)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Arquivo ausente",
                Detail = "Envie o arquivo PDF com a proposta de inovação."
            });
        }

        if (string.IsNullOrWhiteSpace(pedido.Titulo))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Título ausente",
                Detail = "Informe o título da proposta."
            });
        }

        if (string.IsNullOrWhiteSpace(pedido.Setor))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Setor ausente",
                Detail = "Informe o setor/área da proposta."
            });
        }

        if (!UsuarioAtual.TentarId(User, out var proponenteId))
            return Unauthorized();

        await using var stream = pedido.Arquivo.OpenReadStream();
        var comando = new EnviarPropostaComando(
            ProponenteId: proponenteId,
            Titulo: pedido.Titulo,
            Setor: pedido.Setor,
            NomeArquivo: pedido.Arquivo.FileName,
            TamanhoBytes: pedido.Arquivo.Length,
            Conteudo: stream);

        var resultado = await caso.EnviarAsync(comando, ct);
        if (!resultado.Sucesso || resultado.Valor is null)
            return ResultadoHttp.Problema(resultado);

        return Created($"/api/projetos/{resultado.Valor.Id}", resultado.Valor);
    }

    /// <summary>Reenvio de proposta após o analista solicitar reanálise.</summary>
    [HttpPost("{id:guid}/reenviar")]
    [Authorize(Policy = Politicas.SomenteProponente)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Reenviar(
        Guid id,
        [FromForm] ReenviarPropostaUploadRequest? pedido,
        [FromServices] IEnviarProposta caso,
        CancellationToken ct)
    {
        if (pedido is null || pedido.Arquivo is null || pedido.Arquivo.Length == 0)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Arquivo ausente",
                Detail = "Envie o arquivo PDF revisado da proposta."
            });
        }

        if (!UsuarioAtual.TentarId(User, out var proponenteId))
            return Unauthorized();

        await using var stream = pedido.Arquivo.OpenReadStream();
        var comando = new ReenviarPropostaComando(
            ProjetoId: id,
            ProponenteId: proponenteId,
            NomeArquivo: pedido.Arquivo.FileName,
            TamanhoBytes: pedido.Arquivo.Length,
            Conteudo: stream);

        var resultado = await caso.ReenviarAsync(comando, ct);
        if (!resultado.Sucesso || resultado.Valor is null)
            return ResultadoHttp.Problema(resultado);

        return Ok(resultado.Valor);
    }

    /// <summary>Lista de propostas do proponente autenticado (visão limpa).</summary>
    [HttpGet("meus")]
    [Authorize(Policy = Politicas.SomenteProponente)]
    public async Task<IActionResult> MeusProjetos(
        [FromServices] IConsultarProjetos caso,
        CancellationToken ct)
    {
        if (!UsuarioAtual.TentarId(User, out var proponenteId))
            return Unauthorized();

        var projetos = await caso.ListarDoProponenteAsync(proponenteId, ct);
        return Ok(projetos);
    }

    /// <summary>Fila de análise técnica para analistas e administradores (visão com pontuação e critérios Frascati).</summary>
    [HttpGet("fila")]
    [Authorize(Policy = Politicas.AnalistaOuAdm)]
    public async Task<IActionResult> FilaAnalise(
        [FromQuery] string? status,
        [FromServices] IConsultarProjetos caso,
        CancellationToken ct)
    {
        var resultado = await caso.ListarFilaAsync(status, ct);
        if (!resultado.Sucesso || resultado.Valor is null)
            return ResultadoHttp.Problema(resultado);

        return Ok(resultado.Valor);
    }

    /// <summary>Consulta de proposta por identificador. Retorna visão do analista ou do proponente conforme o perfil.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(
        Guid id,
        [FromServices] IConsultarProjetos caso,
        CancellationToken ct)
    {
        if (!UsuarioAtual.TentarId(User, out var solicitanteId))
            return Unauthorized();

        var ehAnalistaOuAdm = User.IsInRole("Analista") || User.IsInRole("Adm") || User.IsInRole("Assistente");
        if (ehAnalistaOuAdm)
        {
            var resultado = await caso.ObterParaAnalistaAsync(id, ct);
            if (!resultado.Sucesso || resultado.Valor is null)
                return ResultadoHttp.Problema(resultado);

            return Ok(resultado.Valor);
        }
        else
        {
            var resultado = await caso.ObterParaProponenteAsync(id, solicitanteId, ct);
            if (!resultado.Sucesso || resultado.Valor is null)
                return ResultadoHttp.Problema(resultado);

            return Ok(resultado.Valor);
        }
    }

    /// <summary>Download do PDF original armazenado da proposta.</summary>
    [HttpGet("{id:guid}/arquivo")]
    public async Task<IActionResult> BaixarArquivo(
        Guid id,
        [FromServices] IConsultarProjetos caso,
        CancellationToken ct)
    {
        if (!UsuarioAtual.TentarId(User, out var solicitanteId))
            return Unauthorized();

        var ehAnalistaOuAdm = User.IsInRole("Analista") || User.IsInRole("Adm") || User.IsInRole("Assistente");
        var resultado = await caso.AbrirArquivoAsync(id, solicitanteId, ehAnalistaOuAdm, ct);
        if (!resultado.Sucesso || resultado.Valor is null)
            return ResultadoHttp.Problema(resultado);

        return File(resultado.Valor.Conteudo, "application/pdf", resultado.Valor.NomeArquivo);
    }

    /// <summary>Deliberação do analista humano sobre a proposta (Aprovar, Reprovar ou Reanálise).</summary>
    [HttpPost("{id:guid}/deliberar")]
    [Authorize(Policy = Politicas.AnalistaOuAdm)]
    public async Task<IActionResult> Deliberar(
        Guid id,
        [FromBody] DeliberarProjetoRequest? pedido,
        [FromServices] IDeliberarProjeto caso,
        CancellationToken ct)
    {
        if (pedido is null || string.IsNullOrWhiteSpace(pedido.Decisao))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Decisão ausente",
                Detail = "Informe a decisão: Aprovado, Reprovado ou Reanalise."
            });
        }

        if (!UsuarioAtual.TentarId(User, out var analistaId))
            return Unauthorized();

        var comando = new DeliberarProjetoComando(
            ProjetoId: id,
            AnalistaId: analistaId,
            Decisao: pedido.Decisao,
            Justificativa: pedido.Justificativa);

        var resultado = await caso.ExecutarAsync(comando, ct);
        if (!resultado.Sucesso || resultado.Valor is null)
            return ResultadoHttp.Problema(resultado);

        return Ok(resultado.Valor);
    }
}
