using Hackthon.Application.Acesso;
using Hackthon.Application.Comum;
using Hackthon.Domain.Projetos;

namespace Hackthon.Application.Projetos;

/// <summary>Decisão final do analista sobre a elegibilidade do projeto na Lei do Bem.</summary>
public sealed class DeliberarProjetoServico : IDeliberarProjeto
{
    private readonly IProjetoRepositorio _projetos;
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IUnidadeDeTrabalho _unidade;
    private readonly IRelogio _relogio;

    public DeliberarProjetoServico(
        IProjetoRepositorio projetos,
        IUsuarioRepositorio usuarios,
        IUnidadeDeTrabalho unidade,
        IRelogio relogio)
    {
        _projetos = projetos;
        _usuarios = usuarios;
        _unidade = unidade;
        _relogio = relogio;
    }

    public async Task<Resultado<ProjetoAnaliseResposta>> ExecutarAsync(DeliberarProjetoComando comando, CancellationToken ct)
    {
        if (!TentarDecisao(comando.Decisao, out var decisao))
            return Falha(400, "Decisão inválida", "Escolha Aprovado, Reprovado ou Reanalise.");

        var justificativa = comando.Justificativa?.Trim();
        if (decisao != StatusProjeto.Aprovado && string.IsNullOrWhiteSpace(justificativa))
        {
            return Falha(400, "Justificativa obrigatória",
                decisao == StatusProjeto.Reanalise
                    ? "Descreva o que o proponente precisa complementar no documento."
                    : "Explique por que o projeto não é elegível à Lei do Bem.");
        }

        if (justificativa?.Length > 4000)
            return Falha(400, "Justificativa muito longa", "Use no máximo 4000 caracteres.");

        var analista = await _usuarios.ObterPorIdAsync(comando.AnalistaId, ct);
        if (analista is null || !analista.Ativo)
            return Falha(403, "Acesso negado", "Sua conta não está ativa para deliberar.");

        var projeto = await _projetos.ObterPorIdAsync(comando.ProjetoId, ct);
        if (projeto is null)
            return Falha(404, "Projeto não encontrado", "O projeto solicitado não existe.");

        if (projeto.Status != StatusProjeto.AguardandoAnalista)
            return Falha(409, "Deliberação indisponível", "Este projeto não está aguardando a decisão do analista.");

        projeto.Deliberar(decisao, justificativa, analista.Id, _relogio.Agora());
        await _unidade.SalvarAsync(ct);

        var proponente = await _usuarios.ObterPorIdAsync(projeto.ProponenteId, ct);
        return Resultado<ProjetoAnaliseResposta>.Ok(MapeadorProjetos.ParaAnalise(projeto, proponente, analista));
    }

    private static bool TentarDecisao(string? valor, out StatusProjeto decisao)
    {
        decisao = default;
        var normalizado = valor?.Trim().Replace("á", "a", StringComparison.OrdinalIgnoreCase);
        if (!Enum.TryParse(normalizado, ignoreCase: true, out decisao))
            return false;
        return decisao is StatusProjeto.Aprovado or StatusProjeto.Reprovado or StatusProjeto.Reanalise;
    }

    private static Resultado<ProjetoAnaliseResposta> Falha(int status, string titulo, string detalhe) =>
        Resultado<ProjetoAnaliseResposta>.Falha(status, titulo, detalhe);
}
