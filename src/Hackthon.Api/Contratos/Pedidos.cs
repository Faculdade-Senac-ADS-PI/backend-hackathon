using Microsoft.AspNetCore.Http;

namespace Hackthon.Api.Contratos;

public sealed record RegistrarRequest(string? Nome, string? Email, string? CpfCnpj, string? Senha, string? Papel);

public sealed record EntrarRequest(string? Email, string? Senha);

public sealed record CriarUsuarioRequest(string? Nome, string? Email, string? CpfCnpj, string? Senha, string? Papel);

public sealed record AlterarPapelRequest(string? Papel);

public sealed record ConsultaAgenteRequest(Guid? ProjetoId, string? Pergunta);

/// <summary>Upload de nova proposta com setor, título e múltiplos arquivos (PDF e CSV).</summary>
public sealed record EnviarPropostaUploadRequest(
    string? Titulo,
    string? Setor,
    IFormFile? Arquivo,
    List<IFormFile>? Arquivos);

/// <summary>Reenvio de proposta após solicitação de reanálise.</summary>
public sealed record ReenviarPropostaUploadRequest(
    IFormFile? Arquivo,
    List<IFormFile>? Arquivos);

/// <summary>Deliberação do analista (Aprovado, Reprovado ou Reanalise).</summary>
public sealed record DeliberarProjetoRequest(string? Decisao, string? Justificativa);
