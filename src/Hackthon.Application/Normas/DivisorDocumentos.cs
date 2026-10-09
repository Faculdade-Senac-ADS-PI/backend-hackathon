using System.Text;
using System.Text.RegularExpressions;

namespace Hackthon.Application.Normas;

/// <summary>
/// Divide o texto de um documento de norma em trechos indexáveis.
/// Leis/regulamentos são divididos por artigo; manuais e guias, por seção,
/// com uma pequena sobreposição para não cortar o sentido na emenda.
///
/// PDFs são extraídos com marcadores de página (\f = form feed); quando presentes,
/// cada trecho registra a página de origem.
/// </summary>
public static partial class DivisorDocumentos
{
    /// <summary>Tamanho-alvo de cada trecho (em caracteres).</summary>
    public const int TamanhoAlvo = 1200;

    /// <summary>Sobreposição entre trechos consecutivos (em caracteres).</summary>
    public const int Sobreposicao = 200;

    private const char MarcadorPagina = '\f';

    /// <summary>Divide o texto em trechos numerados, detectando artigos quando existirem.</summary>
    public static IReadOnlyList<TrechoBruto> Dividir(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return Array.Empty<TrechoBruto>();

        var normalizado = texto.Replace("\r\n", "\n").Replace('\r', '\n').Trim();
        var comPagina = normalizado.Contains(MarcadorPagina);
        var artigos = Artigo().Matches(normalizado);

        return artigos.Count >= 2
            ? DividirPorArtigos(normalizado, artigos, comPagina)
            : DividirPorBlocos(normalizado, comPagina);
    }

    private static IReadOnlyList<TrechoBruto> DividirPorArtigos(string texto, MatchCollection artigos, bool comPagina)
    {
        var trechos = new List<TrechoBruto>();
        for (var i = 0; i < artigos.Count; i++)
        {
            var inicio = artigos[i].Index;
            var fim = i + 1 < artigos.Count ? artigos[i + 1].Index : texto.Length;
            var corpo = texto[inicio..fim].Trim();
            if (corpo.Length < 20)
                continue;

            var pagina = comPagina ? PaginaEm(texto, inicio) : (int?)null;
            var referencia = $"Art. {artigos[i].Groups[1].Value}";
            foreach (var parte in Fatiar(corpo).Where(p => !string.IsNullOrWhiteSpace(p)))
                trechos.Add(new TrechoBruto(referencia, trechos.Count, Limpar(parte), pagina));
        }

        return trechos;
    }

    private static IReadOnlyList<TrechoBruto> DividirPorBlocos(string texto, bool comPagina)
    {
        var paragrafos = texto
            .Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
            .SelectMany(p => p.Length <= TamanhoAlvo * 2 ? new[] { p } : Fatiar(p).ToArray())
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .ToList();

        var trechos = new List<TrechoBruto>();
        var buffer = new StringBuilder();
        var paginaAtual = 1;
        var paginaBuffer = 1;

        foreach (var paragrafo in paragrafos)
        {
            if (buffer.Length == 0)
                paginaBuffer = paginaAtual;

            if (buffer.Length > 0 && buffer.Length + paragrafo.Length + 2 > TamanhoAlvo)
            {
                var conteudo = Limpar(buffer.ToString());
                trechos.Add(new TrechoBruto(
                    $"Seção {trechos.Count + 1}",
                    trechos.Count,
                    conteudo,
                    comPagina ? paginaBuffer : null));

                // Sobreposição: repete o final do bloco anterior no início do próximo.
                var cauda = conteudo.Length > Sobreposicao ? conteudo[^Sobreposicao..] : conteudo;
                buffer.Clear();
                buffer.Append(cauda).Append("\n\n");
                paginaBuffer = paginaAtual;
            }
            else if (buffer.Length > 0)
            {
                buffer.Append("\n\n");
            }

            buffer.Append(paragrafo);
            paginaAtual += paragrafo.Count(c => c == MarcadorPagina);
        }

        if (buffer.Length > 0)
            trechos.Add(new TrechoBruto(
                $"Seção {trechos.Count + 1}",
                trechos.Count,
                Limpar(buffer.ToString()),
                comPagina ? paginaBuffer : null));

        return trechos;
    }

    private static int PaginaEm(string texto, int posicao)
    {
        var pagina = 1;
        for (var i = 0; i < posicao && i < texto.Length; i++)
        {
            if (texto[i] == MarcadorPagina)
                pagina++;
        }

        return pagina;
    }

    private static string Limpar(string texto) =>
        texto.Replace(MarcadorPagina, ' ').Trim();

    private static IEnumerable<string> Fatiar(string texto)
    {
        if (texto.Length <= TamanhoAlvo)
        {
            yield return texto;
            yield break;
        }

        var passo = TamanhoAlvo - Sobreposicao;
        for (var i = 0; i < texto.Length; i += passo)
        {
            var fim = Math.Min(i + TamanhoAlvo, texto.Length);
            yield return texto[i..fim];
            if (fim >= texto.Length)
                break;
        }
    }

    [GeneratedRegex(@"(?im)^\s*art(?:igo)?\.?\s*(\d+)")]
    private static partial Regex Artigo();
}

/// <summary>Trecho produzido pelo divisor, antes de virar vetor.</summary>
public sealed record TrechoBruto(string Referencia, int Ordem, string Texto, int? Pagina);
