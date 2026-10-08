namespace Hackthon.Application.Projetos;

public interface IExtratorTextoArquivo
{
    bool Suporta(string extensao);
    Task<string> ExtrairAsync(Stream stream, string extensao, CancellationToken ct);
}
