using Hackthon.Application.Projetos;

namespace Hackthon.Infrastructure.Arquivos;

public sealed class ExtratorTextoComposto : IExtratorTextoArquivo
{
    private readonly IExtratorTextoPdf _extratorPdf;

    public ExtratorTextoComposto(IExtratorTextoPdf extratorPdf)
    {
        _extratorPdf = extratorPdf;
    }

    public bool Suporta(string extensao)
    {
        var ext = extensao?.ToLowerInvariant().Trim();
        return ext is ".pdf" or ".csv";
    }

    public async Task<string> ExtrairAsync(Stream stream, string extensao, CancellationToken ct)
    {
        var ext = extensao?.ToLowerInvariant().Trim();
        if (ext == ".pdf")
        {
            return await _extratorPdf.ExtrairAsync(stream, ct);
        }

        if (ext == ".csv")
        {
            return await ExtratorTextoCsv.ExtrairAsync(stream, ct);
        }

        throw new DocumentoInvalidoException($"Extensão '{extensao}' não é suportada para extração de texto.");
    }
}
