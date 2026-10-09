using System.Text.Json;
using Hackthon.Domain.Projetos;
using Hackthon.Domain.Usuarios;

namespace Hackthon.Application.Projetos;

internal static class MapeadorProjetos
{
    public static ProjetoProponenteResposta ParaProponente(Projeto projeto)
    {
        var arquivos = projeto.Arquivos
            .Select(a => new ArquivoItemResposta(a.Id, a.NomeOriginal, a.TamanhoBytes, a.Extensao, a.CriadoEm))
            .ToList();

        return new ProjetoProponenteResposta(
            projeto.Id,
            projeto.Protocolo,
            projeto.Titulo,
            projeto.Setor,
            projeto.NomeArquivoOriginal,
            arquivos,
            projeto.VersaoEnvio,
            projeto.Status,
            projeto.JustificativaAnalista,
            projeto.CriadoEm,
            projeto.AtualizadoEm,
            projeto.DecididoEm);
    }

    public static ProjetoAnaliseResposta ParaAnalise(Projeto projeto, Usuario? proponente, Usuario? analista)
    {
        var justificativas = LerJustificativas(projeto.JustificativasIaJson);
        var normas = LerJustificativas(projeto.NormasCitadasIaJson);
        var notas = new Dictionary<string, int?>
        {
            [CriteriosFrascati.Novidade] = projeto.NotaNovidade,
            [CriteriosFrascati.Criatividade] = projeto.NotaCriatividade,
            [CriteriosFrascati.Sistematico] = projeto.NotaSistematico,
            [CriteriosFrascati.Transferibilidade] = projeto.NotaTransferibilidade,
            [CriteriosFrascati.Incerteza] = projeto.NotaIncerteza
        };

        var criterios = CriteriosFrascati.Todos
            .Select(c => new CriterioAvaliadoResposta(
                c.Chave,
                c.Nome,
                c.Descricao,
                notas[c.Chave],
                CriteriosFrascati.NotaMaximaPorCriterio,
                justificativas.GetValueOrDefault(c.Chave),
                normas.GetValueOrDefault(c.Chave)))
            .ToList();

        var arquivos = projeto.Arquivos
            .Select(a => new ArquivoItemResposta(a.Id, a.NomeOriginal, a.TamanhoBytes, a.Extensao, a.CriadoEm))
            .ToList();

        return new ProjetoAnaliseResposta(
            projeto.Id,
            projeto.Protocolo,
            projeto.Titulo,
            projeto.Setor,
            projeto.Status,
            projeto.VersaoEnvio,
            projeto.ProponenteId,
            proponente?.Nome ?? "Proponente removido",
            proponente?.Email ?? string.Empty,
            projeto.NomeArquivoOriginal,
            projeto.TamanhoBytes,
            arquivos,
            projeto.NotaIa,
            projeto.RecomendacaoIa,
            projeto.ResumoIa,
            projeto.ParecerIa,
            projeto.OrigemAnaliseIa,
            projeto.ClasseRecomendadaIa ?? projeto.RecomendacaoIa,
            projeto.MotivoClasseIa,
            projeto.EvidenciasDossieIa,
            projeto.ContradicoesIa,
            projeto.LacunasIa,
            projeto.UsouRag,
            projeto.TrechosConsultadosIa,
            projeto.AnalisadoEm,
            criterios,
            projeto.JustificativaAnalista,
            analista?.Nome,
            projeto.DecididoEm,
            projeto.RecorteSustentado,
            projeto.LimitacaoTecnica,
            projeto.EvidenciaNecessaria,
            projeto.EloAusente,
            projeto.EvidenciasAdicionais,
            projeto.EvidenciasUtilizadas,
            projeto.EvidenciasContrarias,
            projeto.Lacunas,
            projeto.RegraAplicada,
            projeto.CriadoEm,
            projeto.AtualizadoEm);
    }

    private static Dictionary<string, string> LerJustificativas(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new Dictionary<string, string>();
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
        }
        catch (JsonException)
        {
            return new Dictionary<string, string>();
        }
    }
}
