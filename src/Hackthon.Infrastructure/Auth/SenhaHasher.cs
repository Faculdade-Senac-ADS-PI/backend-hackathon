using Hackthon.Application.Acesso;
using Microsoft.AspNetCore.Identity;

namespace Hackthon.Infrastructure.Auth;

public sealed class SenhaHasher : ISenhaHasher
{
    private readonly PasswordHasher<UsuarioMarcador> _hasher = new();

    public string GerarHash(string senha) => _hasher.HashPassword(UsuarioMarcador.Instancia, senha);

    public bool Conferir(string senhaHash, string senha)
    {
        var resultado = _hasher.VerifyHashedPassword(UsuarioMarcador.Instancia, senhaHash, senha);
        return resultado is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }

    private sealed class UsuarioMarcador
    {
        public static readonly UsuarioMarcador Instancia = new();
    }
}
