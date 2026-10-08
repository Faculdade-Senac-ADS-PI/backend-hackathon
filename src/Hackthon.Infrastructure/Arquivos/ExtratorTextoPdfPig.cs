using System.Text;
using Hackthon.Application.Projetos;
using UglyToad.PdfPig;

namespace Hackthon.Infrastructure.Arquivos;

/// <summary>Extrai o texto do PDF com a biblioteca PdfPig (C# puro, sem dependências nativas).</summary>
public sealed class ExtratorTextoPdfPig : IExtratorTextoPdf
{
    public Task<string> ExtrairAsync(Stream pdf, CancellationToken ct)
    {
        try
        {
            using var documento = PdfDocument.Open(pdf);
            var texto = new StringBuilder();
            foreach (var pagina in documento.GetPages())
            {
                ct.ThrowIfCancellationRequested();
                texto.AppendLine(string.Join(' ', pagina.GetWords().Select(palavra => palavra.Text)));
            }

            return Task.FromResult(texto.ToString());
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DocumentoInvalidoException(
                "Não foi possível ler o PDF. Verifique se o arquivo não está corrompido ou protegido por senha.",
                ex);
        }
    }
}
