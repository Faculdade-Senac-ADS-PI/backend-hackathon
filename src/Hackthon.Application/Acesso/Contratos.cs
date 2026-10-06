using Hackthon.Domain.Usuarios;

namespace Hackthon.Application.Acesso;

public sealed record UsuarioResumo(Guid Id, string Nome, string Email, string Papel, bool Ativo);

public sealed record SessaoResposta(string Token, DateTimeOffset ExpiraEm, UsuarioResumo Usuario);

public readonly record struct TokenEmitido(string Token, DateTimeOffset ExpiraEm);

public sealed record RegistrarProponenteComando(string? Nome, string? Email, string? Senha);

public sealed record EntrarComando(string? Email, string? Senha);

public sealed record CriarUsuarioComando(string? Nome, string? Email, string? Senha, string? Papel, Guid AtorId);

public sealed record AlterarPapelComando(Guid UsuarioId, string? Papel, Guid AtorId);

public sealed record AlterarAtivacaoComando(Guid UsuarioId, bool Ativo, Guid AtorId);

public static class UsuarioMapeador
{
    public static UsuarioResumo Resumir(Usuario usuario) =>
        new(usuario.Id, usuario.Nome, usuario.Email, usuario.Papel.ToString(), usuario.Ativo);
}
