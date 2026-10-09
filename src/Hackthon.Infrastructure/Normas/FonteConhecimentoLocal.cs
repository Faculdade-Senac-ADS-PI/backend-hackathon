using System.Text;
using Hackthon.Application.Normas;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UglyToad.PdfPig;

namespace Hackthon.Infrastructure.Normas;

/// <summary>
/// Lê os documentos de norma da pasta configurada (PDF, MD e TXT).
/// Arquivos ocultos e iniciados por "_" (como um LEIA-ME) são ignorados.
/// Não contém nenhum texto de norma: só lê o que o usuário colocou na pasta.
/// </summary>
public sealed class FonteConhecimentoLocal : IFonteConhecimento
{
    private static readonly string[] ExtensoesSuportadas = [".pdf", ".md", ".txt"];

    private readonly ILogger<FonteConhecimentoLocal> _logger;

    public FonteConhecimentoLocal(IOptions<RagOpcoes> opcoes, ILogger<FonteConhecimentoLocal> logger)
    {
        _logger = logger;
        Pasta = Path.GetFullPath(opcoes.Value.PastaBase);
    }

    public string Pasta { get; }

    public async Task<IReadOnlyList<DocumentoFonte>> ListarAsync(CancellationToken ct)
    {
        if (!Directory.Exists(Pasta))
            return Array.Empty<DocumentoFonte>();

        var arquivos = Directory.EnumerateFiles(Pasta, "*", SearchOption.AllDirectories)
            .Where(arquivo => ExtensoesSuportadas.Contains(Path.GetExtension(arquivo).ToLowerInvariant()))
            .Where(arquivo =>
            {
                var nome = Path.GetFileName(arquivo);
                return !nome.StartsWith('.') && !nome.StartsWith('_');
            })
            .OrderBy(arquivo => arquivo, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var documentos = new List<DocumentoFonte>(arquivos.Count);
        foreach (var arquivo in arquivos)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                var extensao = Path.GetExtension(arquivo).ToLowerInvariant();
                var texto = extensao == ".pdf"
                    ? ExtrairPdf(arquivo)
                    : await File.ReadAllTextAsync(arquivo, ct);

                if (string.IsNullOrWhiteSpace(texto))
                {
                    _logger.LogWarning("Documento {Arquivo} não gerou texto legível.", arquivo);
                    continue;
                }

                documentos.Add(new DocumentoFonte(Path.GetRelativePath(Pasta, arquivo), arquivo, texto));
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                _logger.LogWarning(ex, "Não foi possível ler o documento de norma {Arquivo}.", arquivo);
            }
        }

        return documentos;
    }

    /// <summary>Extrai o texto preservando as páginas com \f, para que os trechos registrem a página.</summary>
    private static string ExtrairPdf(string arquivo)
    {
        using var documento = PdfDocument.Open(arquivo);
        var texto = new StringBuilder();
        foreach (var pagina in documento.GetPages())
        {
            texto.Append(string.Join(' ', pagina.GetWords().Select(palavra => palavra.Text)));
            texto.Append('\f');
        }

        return texto.ToString();
    }
}
