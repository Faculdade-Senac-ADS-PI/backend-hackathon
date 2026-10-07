namespace Hackthon.Api.Contratos;

public sealed record RegistrarRequest(string? Nome, string? Email, string? CpfCnpj, string? Senha, string? Papel);

public sealed record EntrarRequest(string? Email, string? Senha);

public sealed record CriarUsuarioRequest(string? Nome, string? Email, string? CpfCnpj, string? Senha, string? Papel);

public sealed record AlterarPapelRequest(string? Papel);

public sealed record ConsultaAgenteRequest(Guid? ProjetoId, string? Pergunta);
