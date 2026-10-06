using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hackthon.Application.Acesso;
using Hackthon.Domain.Usuarios;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Hackthon.Infrastructure.Auth;

public sealed class EmissorToken : IEmissorToken
{
    private readonly IOptions<JwtOpcoes> _opcoes;

    public EmissorToken(IOptions<JwtOpcoes> opcoes)
    {
        _opcoes = opcoes;
    }

    public TokenEmitido Emitir(Usuario usuario, DateTimeOffset agora)
    {
        var opcoes = _opcoes.Value;
        if (string.IsNullOrWhiteSpace(opcoes.Secret) || opcoes.Secret.Length < 32)
        {
            throw new InvalidOperationException(
                "Jwt:Secret precisa ter pelo menos 32 caracteres. Em desenvolvimento use appsettings.Development.json.");
        }

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opcoes.Secret));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
        var expira = agora.AddHours(opcoes.ExpiraHoras <= 0 ? 8 : opcoes.ExpiraHoras);
        var token = new JwtSecurityToken(
            issuer: opcoes.Issuer,
            audience: opcoes.Audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim("nome", usuario.Nome),
                new Claim("papel", usuario.Papel.ToString())
            ],
            notBefore: agora.UtcDateTime,
            expires: expira.UtcDateTime,
            signingCredentials: credenciais);

        return new TokenEmitido(new JwtSecurityTokenHandler().WriteToken(token), expira);
    }
}
