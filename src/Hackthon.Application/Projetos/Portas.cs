using Hackthon.Application.Comum;
using Hackthon.Domain.Projetos;

namespace Hackthon.Application.Projetos;

public interface IProjetoRepositorio
{
    Task<Projeto?> ObterPorIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Projeto>> ListarPorProponenteAsync(Guid proponenteId, CancellationToken ct);
    Task<IReadOnlyList<Projeto>> ListarAsync(StatusProjeto? status, CancellationToken ct);
    Task AdicionarAsync(Projeto projeto, CancellationToken ct);
    Task<bool> ExisteProtocoloAsync(string protocolo, CancellationToken ct);
}

/// <summary>Guarda os PDFs enviados. A implementação padrão grava em disco local.</summary>
public interface IArmazenamentoArquivos
{
    /// <summary>Salva o conteúdo e devolve o caminho relativo usado para recuperá-lo depois.</summary>
    Task<string> SalvarAsync(Stream conteudo, string extensao, CancellationToken ct);
    Stream? AbrirLeitura(string caminho);
    void Remover(string caminho);
}

public interface IExtratorTextoPdf
{
    /// <summary>Extrai o texto do PDF. Lança <see cref="DocumentoInvalidoException"/> se o arquivo não puder ser lido.</summary>
    Task<string> ExtrairAsync(Stream pdf, CancellationToken ct);
}

/// <summary>
/// Agente de IA que avalia a proposta nos 5 critérios de Frascati.
/// Implementações não devem propagar falhas do provedor de IA: devem cair no motor heurístico.
/// </summary>
public interface IAnalisadorElegibilidade
{
    Task<AvaliacaoFrascati> AvaliarAsync(PropostaParaAvaliacao proposta, CancellationToken ct);
}

public sealed record PropostaParaAvaliacao(string Titulo, string Setor, string Texto);

public sealed class DocumentoInvalidoException : Exception
{
    public DocumentoInvalidoException(string mensagem, Exception? interna = null)
        : base(mensagem, interna)
    {
    }
}

public interface IEnviarProposta
{
    Task<Resultado<ProjetoProponenteResposta>> EnviarAsync(EnviarPropostaComando comando, CancellationToken ct);
    Task<Resultado<ProjetoProponenteResposta>> ReenviarAsync(ReenviarPropostaComando comando, CancellationToken ct);
}

public interface IConsultarProjetos
{
    Task<IReadOnlyList<ProjetoProponenteResposta>> ListarDoProponenteAsync(Guid proponenteId, CancellationToken ct);
    Task<Resultado<IReadOnlyList<ProjetoAnaliseResposta>>> ListarFilaAsync(string? status, CancellationToken ct);
    Task<Resultado<ProjetoProponenteResposta>> ObterParaProponenteAsync(Guid id, Guid proponenteId, CancellationToken ct);
    Task<Resultado<ProjetoAnaliseResposta>> ObterParaAnalistaAsync(Guid id, CancellationToken ct);
    Task<Resultado<ArquivoProjeto>> AbrirArquivoAsync(Guid id, Guid solicitanteId, bool ehAnalista, CancellationToken ct);
    Task<Resultado<ArquivoProjeto>> AbrirArquivoItemAsync(Guid projetoId, Guid arquivoId, Guid solicitanteId, bool ehAnalista, CancellationToken ct);
}

public interface IDeliberarProjeto
{
    Task<Resultado<ProjetoAnaliseResposta>> ExecutarAsync(DeliberarProjetoComando comando, CancellationToken ct);
}

/// <summary>Gera o parecer rastreável (documento defendível) a partir da classificação registrada.</summary>
public interface IGerarParecer
{
    Task<Resultado<ParecerResposta>> ExecutarAsync(Guid projetoId, CancellationToken ct);
}
