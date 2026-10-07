using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Hackthon.Api.Auth;

public static class SessaoCookie
{
    public const string Nome = "hackthon_token";
}

public static class UsuarioAtual
{
    public static bool TentarId(ClaimsPrincipal principal, out Guid id)
    {
        var valor = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(valor, out id);
    }
}
