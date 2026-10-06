using System.Net.Mail;
using Hackthon.Domain.Usuarios;

namespace Hackthon.Application.Comum;

public static class Cadastro
{
    public static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();

    public static string? Validar(string? nome, string? email, string? senha)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length < 3 || nome.Trim().Length > 160)
            return "Informe o nome com pelo menos 3 caracteres.";

        if (string.IsNullOrWhiteSpace(email) || !EmailValido(email))
            return "Informe um e-mail válido.";

        if (string.IsNullOrWhiteSpace(senha)
            || senha.Length < 8
            || senha.Length > 72
            || !senha.Any(char.IsLetter)
            || !senha.Any(char.IsDigit))
        {
            return "A senha precisa ter de 8 a 72 caracteres, com pelo menos uma letra e um número.";
        }

        return null;
    }

    public static bool TentarPapel(string? texto, out Papel papel)
    {
        papel = default;
        if (string.IsNullOrWhiteSpace(texto))
            return false;

        var limpo = texto.Trim();
        if (limpo.All(char.IsDigit))
            return false;

        return Enum.TryParse(limpo, ignoreCase: true, out papel) && Enum.IsDefined(papel);
    }

    private static bool EmailValido(string email)
    {
        var normalizado = NormalizarEmail(email);
        if (normalizado.Length > 180 || normalizado.Contains(' '))
            return false;

        try
        {
            var endereco = new MailAddress(normalizado);
            return endereco.Address == normalizado && normalizado.Contains('.');
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
