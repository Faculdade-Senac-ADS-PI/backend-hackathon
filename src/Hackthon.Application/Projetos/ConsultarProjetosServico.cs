using Hackthon.Application.Acesso;
using Hackthon.Application.Comum;
using Hackthon.Domain.Projetos;
using Hackthon.Domain.Usuarios;

namespace Hackthon.Application.Projetos;

public sealed class ConsultarProjetosServico : IConsultarProjetos
{
    private readonly IProjetoRepositorio _projetos;
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IArmazenamentoArquivos _armazenamento;

    public ConsultarProjetosServico(
        IProjetoRepositorio projetos,
        IUsuarioRepositorio usuarios,
        IArmazenamentoArquivos armazenamento)
    {
        _projetos = projetos;
        _usuarios = usuarios;
        _armazenamento = armazenamento;
    }

    public async Task<IReadOnlyList<ProjetoProponenteResposta>> ListarDoProponenteAsync(Guid proponenteId, CancellationToken ct)
    {
        var projetos = await _projetos.ListarPorProponenteAsync(proponenteId, ct);
        return projetos.Select(MapeadorProjetos.ParaProponente).ToList();
    }

    public async Task<Resultado<IReadOnlyList<ProjetoAnaliseResposta>>> ListarFilaAsync(string? status, CancellationToken ct)
    {
        StatusProjeto? filtro = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<StatusProjeto>(status, ignoreCase: true, out var convertido) || !Enum.IsDefined(convertido))
            {
                return Resultado<IReadOnlyList<ProjetoAnaliseResposta>>.Falha(
                    400,
                    "Filtro inválido",
                    "Use ProcessandoIa, AguardandoAnalista, Aprovado, Reprovado ou Reanalise.");
            }

            filtro = convertido;
        }

        var projetos = await _projetos.ListarAsync(filtro, ct);
        var usuarios = (await _usuarios.ListarAsync(ct)).ToDictionary(u => u.Id);

        IReadOnlyList<ProjetoAnaliseResposta> lista = projetos
            .Select(p => MapeadorProjetos.ParaAnalise(
                p,
                usuarios.GetValueOrDefault(p.ProponenteId),
                p.AnalistaId is { } analistaId ? usuarios.GetValueOrDefault(analistaId) : null))
            .ToList();

        return Resultado<IReadOnlyList<ProjetoAnaliseResposta>>.Ok(lista);
    }

    public async Task<Resultado<ProjetoProponenteResposta>> ObterParaProponenteAsync(Guid id, Guid proponenteId, CancellationToken ct)
    {
        var projeto = await _projetos.ObterPorIdAsync(id, ct);
        if (projeto is null || projeto.ProponenteId != proponenteId)
        {
            return Resultado<ProjetoProponenteResposta>.Falha(
                404,
                "Projeto não encontrado",
                "Não localizamos esta proposta entre os seus envios.");
        }

        return Resultado<ProjetoProponenteResposta>.Ok(MapeadorProjetos.ParaProponente(projeto));
    }

    public async Task<Resultado<ProjetoAnaliseResposta>> ObterParaAnalistaAsync(Guid id, CancellationToken ct)
    {
        var projeto = await _projetos.ObterPorIdAsync(id, ct);
        if (projeto is null)
            return Resultado<ProjetoAnaliseResposta>.Falha(404, "Projeto não encontrado", "O projeto solicitado não existe.");

        var proponente = await _usuarios.ObterPorIdAsync(projeto.ProponenteId, ct);
        Usuario? analista = projeto.AnalistaId is { } analistaId ? await _usuarios.ObterPorIdAsync(analistaId, ct) : null;
        return Resultado<ProjetoAnaliseResposta>.Ok(MapeadorProjetos.ParaAnalise(projeto, proponente, analista));
    }

    public async Task<Resultado<ArquivoProjeto>> AbrirArquivoAsync(Guid id, Guid solicitanteId, bool ehAnalista, CancellationToken ct)
    {
        var projeto = await _projetos.ObterPorIdAsync(id, ct);
        if (projeto is null || (!ehAnalista && projeto.ProponenteId != solicitanteId))
            return Resultado<ArquivoProjeto>.Falha(404, "Projeto não encontrado", "O projeto solicitado não existe.");

        var conteudo = _armazenamento.AbrirLeitura(projeto.CaminhoArquivo);
        if (conteudo is null)
            return Resultado<ArquivoProjeto>.Falha(404, "Arquivo indisponível", "O PDF desta proposta não está mais no armazenamento.");

        return Resultado<ArquivoProjeto>.Ok(new ArquivoProjeto(conteudo, projeto.NomeArquivoOriginal));
    }
}
