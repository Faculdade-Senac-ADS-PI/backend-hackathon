namespace Hackthon.Application.Comum;

public sealed class Resultado<T>
{
    private Resultado(bool sucesso, T? valor, string? titulo, string? detalhe, int status)
    {
        Sucesso = sucesso;
        Valor = valor;
        Titulo = titulo;
        Detalhe = detalhe;
        Status = status;
    }

    public bool Sucesso { get; }
    public T? Valor { get; }
    public string? Titulo { get; }
    public string? Detalhe { get; }
    public int Status { get; }

    public static Resultado<T> Ok(T valor) => new(true, valor, null, null, 200);

    public static Resultado<T> Falha(int status, string titulo, string detalhe) =>
        new(false, default, titulo, detalhe, status);
}
