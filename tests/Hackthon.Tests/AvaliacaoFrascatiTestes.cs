using Hackthon.Application.Projetos;
using Hackthon.Domain.Projetos;
using Hackthon.Infrastructure.Agente;
using Xunit;

namespace Hackthon.Tests;

public class AvaliacaoFrascatiTestes
{
    [Fact]
    public void AvaliacaoFrascati_CalculaNotaTotalCorretamente()
    {
        var avaliacao = new AvaliacaoFrascati(
            notaNovidade: 18,
            notaCriatividade: 15,
            notaSistematico: 16,
            notaTransferibilidade: 14,
            notaIncerteza: 17,
            resumo: "Resumo do teste",
            parecer: "Parecer do teste",
            recomendacao: "Elegível",
            justificativasJson: "{}",
            origem: "teste");

        Assert.Equal(80, avaliacao.NotaTotal);
        Assert.Equal("Elegível", avaliacao.Recomendacao);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(21)]
    public void AvaliacaoFrascati_RejeitaNotasForaDoIntervalo(int notaInvalida)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new AvaliacaoFrascati(
                notaNovidade: notaInvalida,
                notaCriatividade: 15,
                notaSistematico: 15,
                notaTransferibilidade: 15,
                notaIncerteza: 15,
                resumo: "Resumo",
                parecer: "Parecer",
                recomendacao: "Elegível",
                justificativasJson: "{}",
                origem: "teste"));
    }

    [Fact]
    public void Projeto_TransitaEstadosCorretamente_AoRegistrarIaEDeliberar()
    {
        var agora = DateTimeOffset.UtcNow;
        var proponenteId = Guid.NewGuid();
        var analistaId = Guid.NewGuid();

        var documento = new DocumentoProposta(
            nomeOriginal: "projeto_teste.pdf",
            caminho: "propostas/teste.pdf",
            tamanhoBytes: 50000,
            textoExtraido: "Texto com inovação técnica e experimentos sistemáticos.");

        var projeto = Projeto.Criar(
            protocolo: "PROP-2026-TEST",
            titulo: "Projeto Teste P&D",
            setor: "Engenharia de Software",
            proponenteId: proponenteId,
            documentos: new[] { documento },
            agora: agora);

        Assert.Equal(StatusProjeto.ProcessandoIa, projeto.Status);

        var avaliacao = new AvaliacaoFrascati(
            notaNovidade: 16,
            notaCriatividade: 16,
            notaSistematico: 18,
            notaTransferibilidade: 15,
            notaIncerteza: 17,
            resumo: "Avaliação positiva da IA",
            parecer: "Apto",
            recomendacao: "Elegível",
            justificativasJson: "{}",
            origem: "heuristico");

        projeto.RegistrarAnaliseIa(avaliacao, agora);

        Assert.Equal(StatusProjeto.AguardandoAnalista, projeto.Status);
        Assert.Equal(82, projeto.NotaIa);

        // Analista classifica como Elegível, citando as evidências por ID (rastreabilidade)
        var deliberacao = new DeliberacaoAnalista(
            Justificativa: "Projeto muito bem detalhado",
            RecorteSustentado: null,
            LimitacaoTecnica: null,
            EvidenciaNecessaria: null,
            EloAusente: null,
            EvidenciasAdicionais: null,
            EvidenciasUtilizadas: "PROP-2026-TEST-EV01, PROP-2026-TEST-EV02",
            EvidenciasContrarias: null,
            Lacunas: null,
            RegraAplicada: null);

        projeto.Deliberar(StatusProjeto.Elegivel, deliberacao, analistaId, agora.AddMinutes(5));

        Assert.Equal(StatusProjeto.Elegivel, projeto.Status);
        Assert.Equal(analistaId, projeto.AnalistaId);
        Assert.NotNull(projeto.DecididoEm);
    }

    [Fact]
    public void ClassificarPorNota_MapeiaParaAsQuatroClasses()
    {
        Assert.Equal("Elegível", AvaliacaoFrascati.ClassificarPorNota(85));
        Assert.Equal("Com ressalvas", AvaliacaoFrascati.ClassificarPorNota(60));
        Assert.Equal("Não elegível", AvaliacaoFrascati.ClassificarPorNota(20));
    }

    [Fact]
    public void Projeto_RejeitaClassificacaoConclusivaSemEvidencias()
    {
        var projeto = CriarProjetoAguardandoAnalista(out var analistaId, out var agora);

        var deliberacao = new DeliberacaoAnalista(
            Justificativa: "Conclusão sem fontes",
            RecorteSustentado: null,
            LimitacaoTecnica: null,
            EvidenciaNecessaria: null,
            EloAusente: null,
            EvidenciasAdicionais: null,
            EvidenciasUtilizadas: null,
            EvidenciasContrarias: null,
            Lacunas: null,
            RegraAplicada: null);

        var excecao = Assert.Throws<ArgumentException>(() =>
            projeto.Deliberar(StatusProjeto.Elegivel, deliberacao, analistaId, agora));

        Assert.Contains("evidências utilizadas", excecao.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Projeto_ExigeCamposEspecificosDeComRessalvas()
    {
        var projeto = CriarProjetoAguardandoAnalista(out var analistaId, out var agora);

        var deliberacao = new DeliberacaoAnalista(
            Justificativa: "Há P&D, mas com limite",
            RecorteSustentado: "Sustenta a fase 1",
            LimitacaoTecnica: null, // ausente de propósito
            EvidenciaNecessaria: "Relatório em ambiente real",
            EloAusente: null,
            EvidenciasAdicionais: null,
            EvidenciasUtilizadas: "PRJ21-EV05",
            EvidenciasContrarias: null,
            Lacunas: null,
            RegraAplicada: null);

        Assert.Throws<ArgumentException>(() =>
            projeto.Deliberar(StatusProjeto.ComRessalvas, deliberacao, analistaId, agora));
    }

    private static Projeto CriarProjetoAguardandoAnalista(out Guid analistaId, out DateTimeOffset agora)
    {
        agora = DateTimeOffset.UtcNow;
        analistaId = Guid.NewGuid();

        var documento = new DocumentoProposta(
            nomeOriginal: "projeto_teste.pdf",
            caminho: "propostas/teste.pdf",
            tamanhoBytes: 50000,
            textoExtraido: "Texto com inovação técnica e experimentos sistemáticos.");

        var projeto = Projeto.Criar(
            protocolo: $"PROP-{Guid.NewGuid().ToString("N")[..8]}",
            titulo: "Projeto Teste P&D",
            setor: "Engenharia de Software",
            proponenteId: Guid.NewGuid(),
            documentos: new[] { documento },
            agora: agora);

        var avaliacao = new AvaliacaoFrascati(
            notaNovidade: 16,
            notaCriatividade: 16,
            notaSistematico: 18,
            notaTransferibilidade: 15,
            notaIncerteza: 17,
            resumo: "Avaliação positiva da IA",
            parecer: "Apto",
            recomendacao: "Elegível",
            justificativasJson: "{}",
            origem: "heuristico");

        projeto.RegistrarAnaliseIa(avaliacao, agora);
        return projeto;
    }

    [Fact]
    public void AnalisadorElegibilidadeHeuristico_AvaliaOs5CriteriosComSucesso()
    {
        var analisador = new AnalisadorElegibilidadeHeuristico();
        var proposta = new PropostaParaAvaliacao(
            Titulo: "Novo Sistema de IA Distribuída com Otimização Termodinâmica",
            Setor: "Pesquisa Aplicada",
            Texto: """
            O presente projeto visa a pesquisa e desenvolvimento de um novo método algorítmico inédito para a empresa.
            Apresenta abordagem criativa gerando ganhos substanciais de produtividade e eficiência operacional para o setor.
            Apresenta incerteza técnica considerável devido à falta de dados empíricos no setor.
            A metodologia científica contempla delineamento experimental, hipóteses refutáveis, testes controlados e registro sistemático.
            Os resultados são generalizáveis e passíveis de replicação e transferência de conhecimento técnico para outras unidades.
            """);

        var resultado = analisador.Avaliar(proposta);

        Assert.InRange(resultado.NotaNovidade, 1, 20);
        Assert.InRange(resultado.NotaCriatividade, 1, 20);
        Assert.InRange(resultado.NotaSistematico, 1, 20);
        Assert.InRange(resultado.NotaTransferibilidade, 1, 20);
        Assert.InRange(resultado.NotaIncerteza, 1, 20);
        Assert.True(resultado.NotaTotal > 0);
        Assert.NotEmpty(resultado.Resumo);
        Assert.NotEmpty(resultado.JustificativasJson);
    }
}
