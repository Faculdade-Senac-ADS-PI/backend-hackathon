using System.Globalization;
using System.Text;
using Hackthon.Application.Acesso;
using Hackthon.Application.Comum;
using Hackthon.Domain.Projetos;

namespace Hackthon.Application.Projetos;

/// <summary>
/// Deliberação do analista: classificação final com uma das quatro classes do desafio
/// (Elegível · Com ressalvas · Não elegível · Evidência insuficiente) ou pedido de Reanálise.
/// Exige fundamentação e evidências por ID — base do parecer rastreável.
/// </summary>
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
            return Falha(400, "Decisão inválida",
                "Escolha Elegível, Com ressalvas, Não elegível, Evidência insuficiente ou Reanalise.");

        var justificativa = comando.Justificativa?.Trim();
        if (string.IsNullOrWhiteSpace(justificativa))
            return Falha(400, "Justificativa obrigatória",
                "Registre o motivo da conclusão com as fontes que a sustentam: ela é a base do parecer rastreável.");

        if (justificativa.Length > 4000)
            return Falha(400, "Justificativa muito longa", "Use no máximo 4000 caracteres.");

        if (!ValidarCamposDaClasse(decisao, comando, out var falhaClasse))
            return falhaClasse!;

        var analista = await _usuarios.ObterPorIdAsync(comando.AnalistaId, ct);
        if (analista is null || !analista.Ativo)
            return Falha(403, "Acesso negado", "Sua conta não está ativa para deliberar.");

        var projeto = await _projetos.ObterPorIdAsync(comando.ProjetoId, ct);
        if (projeto is null)
            return Falha(404, "Projeto não encontrado", "O projeto solicitado não existe.");

        if (projeto.Status != StatusProjeto.AguardandoAnalista)
            return Falha(409, "Deliberação indisponível", "Este projeto não está aguardando a decisão do analista.");

        var dados = new DeliberacaoAnalista(
            Justificativa: justificativa,
            RecorteSustentado: comando.RecorteSustentado?.Trim(),
            LimitacaoTecnica: comando.LimitacaoTecnica?.Trim(),
            EvidenciaNecessaria: comando.EvidenciaNecessaria?.Trim(),
            EloAusente: comando.EloAusente?.Trim(),
            EvidenciasAdicionais: comando.EvidenciasAdicionais?.Trim(),
            EvidenciasUtilizadas: comando.EvidenciasUtilizadas?.Trim(),
            EvidenciasContrarias: comando.EvidenciasContrarias?.Trim(),
            Lacunas: comando.Lacunas?.Trim(),
            RegraAplicada: comando.RegraAplicada?.Trim());

        try
        {
            projeto.Deliberar(decisao, dados, analista.Id, _relogio.Agora());
        }
        catch (ArgumentException ex)
        {
            return Falha(400, "Deliberação incompleta", ex.Message);
        }

        await _unidade.SalvarAsync(ct);

        var proponente = await _usuarios.ObterPorIdAsync(projeto.ProponenteId, ct);
        return Resultado<ProjetoAnaliseResposta>.Ok(MapeadorProjetos.ParaAnalise(projeto, proponente, analista));
    }

    /// <summary>Valida os campos obrigatórios de cada classe conforme o Guia do Participante.</summary>
    private static bool ValidarCamposDaClasse(StatusProjeto decisao, DeliberarProjetoComando comando, out Resultado<ProjetoAnaliseResposta>? falha)
    {
        falha = null;

        if (decisao == StatusProjeto.Reanalise)
            return true;

        // Toda classificação conclusiva precisa citar as evidências utilizadas, por ID.
        if (string.IsNullOrWhiteSpace(comando.EvidenciasUtilizadas))
        {
            falha = Falha(400, "Evidências obrigatórias",
                "Informe as evidências utilizadas na conclusão, identificadas por ID (ex.: PRJ21-EV05, metodo.md#2).");
            return false;
        }

        if (decisao == StatusProjeto.ComRessalvas)
        {
            if (string.IsNullOrWhiteSpace(comando.RecorteSustentado) ||
                string.IsNullOrWhiteSpace(comando.LimitacaoTecnica) ||
                string.IsNullOrWhiteSpace(comando.EvidenciaNecessaria))
            {
                falha = Falha(400, "Deliberação incompleta",
                    "\"Com ressalvas\" exige: o recorte sustentado pelas evidências, a limitação técnica específica e a evidência necessária para resolver a ressalva.");
                return false;
            }
        }

        if (decisao == StatusProjeto.EvidenciaInsuficiente &&
            (string.IsNullOrWhiteSpace(comando.EloAusente) ||
             string.IsNullOrWhiteSpace(comando.EvidenciasAdicionais)))
        {
            falha = Falha(400, "Deliberação incompleta",
                "\"Evidência insuficiente\" exige especificar o elo ausente e as evidências adicionais a solicitar.");
            return false;
        }

        return true;
    }

    /// <summary>Aceita as entradas com ou sem acento, em qualquer caixa (ex.: "Nao elegivel", "COM RESSALVAS").</summary>
    private static bool TentarDecisao(string? valor, out StatusProjeto decisao)
    {
        decisao = default;
        var normalizado = Normalizar(valor);
        return normalizado switch
        {
            "elegivel" => Atribuir(StatusProjeto.Elegivel, out decisao),
            "comressalvas" => Atribuir(StatusProjeto.ComRessalvas, out decisao),
            "naoelegivel" => Atribuir(StatusProjeto.NaoElegivel, out decisao),
            "evidenciainsuficiente" => Atribuir(StatusProjeto.EvidenciaInsuficiente, out decisao),
            "reanalise" => Atribuir(StatusProjeto.Reanalise, out decisao),
            _ => false
        };

        static bool Atribuir(StatusProjeto valor, out StatusProjeto destino)
        {
            destino = valor;
            return true;
        }
    }

    private static string Normalizar(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return string.Empty;

        var minusculo = valor.Trim().ToLowerInvariant();
        var decomposto = minusculo.Normalize(NormalizationForm.FormD);
        var limpo = new StringBuilder(decomposto.Length);
        foreach (var c in decomposto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                continue;
            // Remove espaços e hífens para aceitar tanto "Com ressalvas" quanto "ComRessalvas".
            if (char.IsWhiteSpace(c) || c == '-' || c == '_')
                continue;
            limpo.Append(c);
        }

        return limpo.ToString();
    }

    private static Resultado<ProjetoAnaliseResposta> Falha(int status, string titulo, string detalhe) =>
        Resultado<ProjetoAnaliseResposta>.Falha(status, titulo, detalhe);
}
