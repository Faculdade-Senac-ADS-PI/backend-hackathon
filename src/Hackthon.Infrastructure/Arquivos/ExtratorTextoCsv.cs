using System.Text;

namespace Hackthon.Infrastructure.Arquivos;

public sealed class ExtratorTextoCsv
{
    private const int LimiteLinhas = 500;
    private const int LimiteCaracteres = 150000;

    public static async Task<string> ExtrairAsync(Stream stream, CancellationToken ct)
    {
        // Lê com UTF-8 detectando BOM automaticamente
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: true);
        var sb = new StringBuilder();
        int linhaNum = 0;

        while (!reader.EndOfStream)
        {
            ct.ThrowIfCancellationRequested();
            var linha = await reader.ReadLineAsync(ct);
            if (linha is null) break;

            sb.AppendLine(linha);
            linhaNum++;

            if (sb.Length >= LimiteCaracteres || linhaNum >= LimiteLinhas)
            {
                sb.AppendLine($"\n... [Arquivo CSV truncado para análise: total exibido de {linhaNum} linhas].");
                break;
            }
        }

        return sb.ToString();
    }
}
