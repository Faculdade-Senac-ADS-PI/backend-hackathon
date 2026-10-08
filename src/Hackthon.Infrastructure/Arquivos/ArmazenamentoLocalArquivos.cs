using Hackthon.Application.Projetos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hackthon.Infrastructure.Arquivos;

public sealed class ArmazenamentoOpcoes
{
    public const string Secao = "Armazenamento";

    /// <summary>Pasta onde os PDFs são gravados. Caminhos relativos partem da raiz da API.</summary>
    public string Diretorio { get; set; } = "armazenamento/propostas";
}

/// <summary>
/// Grava os PDFs em disco, organizados por ano/mês e com nome aleatório (o nome original fica no banco).
/// Para produção em várias instâncias, troque por uma implementação em object storage (S3, Azure Blob, MinIO).
/// </summary>
public sealed class ArmazenamentoLocalArquivos : IArmazenamentoArquivos
{
    private readonly string _raiz;
    private readonly ILogger<ArmazenamentoLocalArquivos> _logger;

    public ArmazenamentoLocalArquivos(IOptions<ArmazenamentoOpcoes> opcoes, ILogger<ArmazenamentoLocalArquivos> logger)
    {
        _raiz = Path.GetFullPath(opcoes.Value.Diretorio);
        _logger = logger;
    }

    public async Task<string> SalvarAsync(Stream conteudo, string extensao, CancellationToken ct)
    {
        var agora = DateTime.UtcNow;
        var relativo = $"{agora:yyyy}/{agora:MM}/{Guid.NewGuid():N}{extensao.ToLowerInvariant()}";
        var completo = Resolver(relativo)
            ?? throw new InvalidOperationException("Caminho de armazenamento inválido.");

        Directory.CreateDirectory(Path.GetDirectoryName(completo)!);
        await using var destino = new FileStream(completo, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
        await conteudo.CopyToAsync(destino, ct);
        return relativo;
    }

    public Stream? AbrirLeitura(string caminho)
    {
        var completo = Resolver(caminho);
        if (completo is null || !File.Exists(completo))
            return null;

        return new FileStream(completo, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);
    }

    public void Remover(string caminho)
    {
        var completo = Resolver(caminho);
        if (completo is null)
            return;

        try
        {
            if (File.Exists(completo))
                File.Delete(completo);
        }
        catch (IOException ex)
        {
            _logger.LogWarning(ex, "Não foi possível remover o arquivo {Caminho}.", caminho);
        }
    }

    /// <summary>Impede que um caminho gravado no banco aponte para fora da pasta de armazenamento.</summary>
    private string? Resolver(string caminho)
    {
        if (string.IsNullOrWhiteSpace(caminho))
            return null;

        var completo = Path.GetFullPath(Path.Combine(_raiz, caminho));
        return completo.StartsWith(_raiz + Path.DirectorySeparatorChar, StringComparison.Ordinal) ? completo : null;
    }
}
