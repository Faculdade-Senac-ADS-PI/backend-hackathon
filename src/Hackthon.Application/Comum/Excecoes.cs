namespace Hackthon.Application.Comum;

public sealed class ConflitoDeEmailException : Exception
{
    public ConflitoDeEmailException()
        : base("Já existe uma conta com este e-mail.")
    {
    }
}

public sealed class ConflitoDeCpfCnpjException : Exception
{
    public ConflitoDeCpfCnpjException()
        : base("Já existe uma conta com este CPF/CNPJ.")
    {
    }
}

public sealed class RegraDeNegocioException : Exception
{
    public RegraDeNegocioException(int status, string titulo, string detalhe)
        : base(detalhe)
    {
        Status = status;
        Titulo = titulo;
    }

    public int Status { get; }
    public string Titulo { get; }
}

public static class Falhas
{
    public static Resultado<T> Traduzir<T>(Exception ex) => ex switch
    {
        ConflitoDeEmailException => Resultado<T>.Falha(409, "E-mail já cadastrado", ex.Message),
        ConflitoDeCpfCnpjException => Resultado<T>.Falha(409, "CPF/CNPJ já cadastrado", ex.Message),
        RegraDeNegocioException regra => Resultado<T>.Falha(regra.Status, regra.Titulo, regra.Message),
        _ => throw new InvalidOperationException("Exceção fora do contrato de negócio.", ex)
    };
}
