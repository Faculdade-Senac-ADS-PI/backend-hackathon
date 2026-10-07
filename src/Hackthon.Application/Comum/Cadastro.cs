using System.Net.Mail;
using Hackthon.Domain.Usuarios;

namespace Hackthon.Application.Comum;

public static class Cadastro
{
    public static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();

    public static string NormalizarCpfCnpj(string? documento) =>
        string.IsNullOrWhiteSpace(documento)
            ? string.Empty
            : new string(documento.Where(char.IsDigit).ToArray());

    public static string? Validar(string? nome, string? email, string? cpfCnpj, string? senha)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length < 3 || nome.Trim().Length > 160)
            return "Informe o nome com pelo menos 3 caracteres.";

        if (string.IsNullOrWhiteSpace(email) || !EmailValido(email))
            return "Informe um e-mail válido.";

        if (string.IsNullOrWhiteSpace(cpfCnpj) || !ValidarCpfCnpj(cpfCnpj))
            return "Informe um CPF ou CNPJ válido.";

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

    public static bool ValidarCpfCnpj(string? documento)
    {
        var normalizado = NormalizarCpfCnpj(documento);
        if (normalizado.Length == 11)
            return ValidarCpf(normalizado);
        if (normalizado.Length == 14)
            return ValidarCnpj(normalizado);
        return false;
    }

    public static bool ValidarCpf(string cpf)
    {
        if (cpf.Length != 11)
            return false;
        if (cpf.All(c => c == cpf[0]))
            return false;

        var soma = 0;
        for (var i = 0; i < 9; i++)
            soma += (cpf[i] - '0') * (10 - i);
        var resto = soma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;
        if ((cpf[9] - '0') != digito1)
            return false;

        soma = 0;
        for (var i = 0; i < 10; i++)
            soma += (cpf[i] - '0') * (11 - i);
        resto = soma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;
        return (cpf[10] - '0') == digito2;
    }

    public static bool ValidarCnpj(string cnpj)
    {
        if (cnpj.Length != 14)
            return false;
        if (cnpj.All(c => c == cnpj[0]))
            return false;

        int[] multiplicadores1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] multiplicadores2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        var soma = 0;
        for (var i = 0; i < 12; i++)
            soma += (cnpj[i] - '0') * multiplicadores1[i];
        var resto = soma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;
        if ((cnpj[12] - '0') != digito1)
            return false;

        soma = 0;
        for (var i = 0; i < 13; i++)
            soma += (cnpj[i] - '0') * multiplicadores2[i];
        resto = soma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;
        return (cnpj[13] - '0') == digito2;
    }

    public static bool TentarPapel(string? texto, out Papel papel)
    {
        papel = default;
        if (string.IsNullOrWhiteSpace(texto))
            return false;

        var limpo = texto.Trim();
        if (limpo.All(char.IsDigit))
            return false;

        if (limpo.Equals("Preponente", StringComparison.OrdinalIgnoreCase))
        {
            papel = Papel.Proponente;
            return true;
        }

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
