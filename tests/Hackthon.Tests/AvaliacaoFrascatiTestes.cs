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
            recomendacao: "Alta elegibilidade",
            justificativasJson: "{}",
            origem: "teste");

        Assert.Equal(80, avaliacao.NotaTotal);
        Assert.Equal("Alta elegibilidade", avaliacao.Recomendacao);
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
                recomendacao: "Alta elegibilidade",
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
            recomendacao: "Alta elegibilidade",
            justificativasJson: "{}",
            origem: "heuristico");

        projeto.RegistrarAnaliseIa(avaliacao, agora);

        Assert.Equal(StatusProjeto.AguardandoAnalista, projeto.Status);
        Assert.Equal(82, projeto.NotaIa);

        // Analista delibera aprovando
        projeto.Deliberar(StatusProjeto.Aprovado, "Projeto muito bem detalhado", analistaId, agora.AddMinutes(5));

        Assert.Equal(StatusProjeto.Aprovado, projeto.Status);
        Assert.Equal(analistaId, projeto.AnalistaId);
        Assert.NotNull(projeto.DecididoEm);
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
