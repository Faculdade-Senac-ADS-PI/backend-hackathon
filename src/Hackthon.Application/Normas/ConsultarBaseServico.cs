namespace Hackthon.Application.Normas;

/// <summary>Informa o estado da base de normas (documentos e trechos indexados).</summary>
public sealed class ConsultarBaseServico : IConsultarBase
{
    private readonly IFonteConhecimento _fonte;
    private readonly INormaRepositorio _repositorio;

    public ConsultarBaseServico(IFonteConhecimento fonte, INormaRepositorio repositorio)
    {
        _fonte = fonte;
        _repositorio = repositorio;
    }

    public async Task<EstadoBaseResposta> ObterAsync(CancellationToken ct)
    {
        var documentos = await _repositorio.ContarPorDocumentoAsync(ct);
        var total = documentos.Sum(d => d.Trechos);
        var mensagem = total > 0
            ? $"{documentos.Count} documento(s) indexado(s), {total} trecho(s)."
            : "A base ainda não foi indexada. Coloque os documentos na pasta e execute a preparação.";

        return new EstadoBaseResposta(_fonte.Pasta, total > 0, total, documentos, mensagem);
    }
}
