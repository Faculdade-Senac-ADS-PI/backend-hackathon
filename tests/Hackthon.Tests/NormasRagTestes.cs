using Hackthon.Application.Normas;
using Hackthon.Domain.Projetos;
using Hackthon.Infrastructure.Normas;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Hackthon.Tests;

public class NormasRagTestes
{
    // ---------- Divisão de documentos ----------

    [Fact]
    public void DivisorDocumentos_DividePorArtigos()
    {
        const string texto =
            "Art. 1º Esta Lei estabelece incentivos fiscais para a inovação tecnológica.\n" +
            "Art. 2º Para os fins desta Lei considera-se pesquisa o trabalho experimental.";

        var trechos = DivisorDocumentos.Dividir(texto);

        Assert.True(trechos.Count >= 2);
        Assert.Contains(trechos, t => t.Referencia == "Art. 1");
        Assert.Contains(trechos, t => t.Referencia == "Art. 2");
    }

    [Fact]
    public void DivisorDocumentos_DividePorBlocosQuandoNaoHaArtigos()
    {
        var paragrafo = new string('a', 700);
        var texto = string.Join("\n\n", Enumerable.Repeat(paragrafo, 6));

        var trechos = DivisorDocumentos.Dividir(texto);

        Assert.True(trechos.Count >= 2);
        Assert.All(trechos, t => Assert.StartsWith("Seção", t.Referencia));
    }

    [Fact]
    public void DivisorDocumentos_IgnoraTextoVazio()
    {
        Assert.Empty(DivisorDocumentos.Dividir("   \n  "));
    }

    [Fact]
    public void DivisorDocumentos_RegistraPaginaQuandoHaMarcadorDePdf()
    {
        var paragrafo = new string('b', 1300);
        var texto = paragrafo + "\f\n\n" + paragrafo;

        var trechos = DivisorDocumentos.Dividir(texto);

        Assert.True(trechos.Count >= 2);
        Assert.Contains(trechos, t => t.Pagina == 1);
        Assert.Contains(trechos, t => t.Pagina == 2);
    }

    // ---------- Classe recomendada ----------

    [Theory]
    [InlineData("Elegível", "Elegível")]
    [InlineData("elegivel", "Elegível")]
    [InlineData("com ressalvas", "Com ressalvas")]
    [InlineData("ComRessalvas", "Com ressalvas")]
    [InlineData("NÃO ELEGÍVEL", "Não elegível")]
    [InlineData("Evidência insuficiente", "Evidência insuficiente")]
    public void NormalizarClasse_AceitaVariacoesDeRotulo(string entrada, string esperado)
    {
        Assert.Equal(esperado, AvaliacaoFrascati.NormalizarClasse(entrada));
    }

    [Fact]
    public void NormalizarClasse_DevolveNuloQuandoNaoReconhece()
    {
        Assert.Null(AvaliacaoFrascati.NormalizarClasse("talvez"));
        Assert.Null(AvaliacaoFrascati.NormalizarClasse(null));
    }

    [Fact]
    public void AvaliacaoFrascati_UsaClasseDoAgenteIndependenteDaNota()
    {
        var avaliacao = new AvaliacaoFrascati(
            notaNovidade: 20, notaCriatividade: 20, notaSistematico: 20,
            notaTransferibilidade: 20, notaIncerteza: 20,
            resumo: "Resumo", parecer: "Parecer",
            recomendacao: "Não elegível", justificativasJson: "{}", origem: "llm:teste",
            motivoClasse: "Aplicação de técnica conhecida, sem incerteza tecnológica.",
            normasCitadasJson: "{\"Novidade\":\"[1] Art. 2\"}",
            usouRag: true, trechosConsultados: 4);

        Assert.Equal(100, avaliacao.NotaTotal);
        Assert.Equal("Não elegível", avaliacao.ClasseRecomendada);
        Assert.Equal("Não elegível", avaliacao.Recomendacao);
        Assert.True(avaliacao.UsouRag);
        Assert.Equal(4, avaliacao.TrechosConsultados);
        Assert.NotNull(avaliacao.NormasCitadasJson);
    }

    [Fact]
    public void AvaliacaoFrascati_CaiNaFaixaPorNotaQuandoSemClasse()
    {
        var avaliacao = new AvaliacaoFrascati(
            notaNovidade: 16, notaCriatividade: 16, notaSistematico: 16,
            notaTransferibilidade: 16, notaIncerteza: 16,
            resumo: "Resumo", parecer: "Parecer",
            recomendacao: string.Empty, justificativasJson: "{}", origem: "teste");

        Assert.Equal("Elegível", avaliacao.ClasseRecomendada);
        Assert.False(avaliacao.UsouRag);
        Assert.Null(avaliacao.NormasCitadasJson);
    }

    // ---------- Degradação segura (RAG não pode quebrar o fluxo) ----------

    [Fact]
    public async Task RecuperadorTrechos_BaseVazia_NaoRetornaTrechos()
    {
        var recuperador = new RecuperadorTrechos(
            new EmbeddingFalso(),
            new RepositorioVazio(),
            Options.Create(new RagOpcoes()),
            NullLogger<RecuperadorTrechos>.Instance);

        var trechos = await recuperador.BuscarAsync("alguma pergunta", 3, CancellationToken.None);

        Assert.Empty(trechos);
        Assert.False(await recuperador.BaseDisponivelAsync(CancellationToken.None));
    }

    [Fact]
    public async Task IndexarBaseServico_PastaVazia_RetornaFalhaSemLancar()
    {
        var servico = new IndexarBaseServico(
            new FonteVazia(),
            new EmbeddingFalso(),
            new RepositorioVazio(),
            NullLogger<IndexarBaseServico>.Instance);

        var resultado = await servico.ExecutarAsync(CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Equal(409, resultado.Status);
        Assert.Contains("vazia", resultado.Titulo, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task IndexarBaseServico_ComDocumento_IndexaOsTrechos()
    {
        var repositorio = new RepositorioCaptura();
        var servico = new IndexarBaseServico(
            new FonteFixa(),
            new EmbeddingFalso(),
            repositorio,
            NullLogger<IndexarBaseServico>.Instance);

        var resultado = await servico.ExecutarAsync(CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.True(repositorio.TrechosGravados >= 2);
        Assert.NotNull(resultado.Valor);
        Assert.True(resultado.Valor!.Configurada);
    }

    [Fact]
    public async Task IndexarBaseServico_FalhaDeEmbedding_Retorna503SemLancar()
    {
        var servico = new IndexarBaseServico(
            new FonteFixa(),
            new EmbeddingQuebrado(),
            new RepositorioCaptura(),
            NullLogger<IndexarBaseServico>.Instance);

        var resultado = await servico.ExecutarAsync(CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Equal(503, resultado.Status);
    }

    // ---------- Dublês ----------

    private sealed class EmbeddingFalso : IGeradorEmbedding
    {
        public Task<float[]> GerarAsync(string texto, CancellationToken ct) =>
            Task.FromResult(new float[3]);

        public Task<IReadOnlyList<float[]>> GerarLoteAsync(IReadOnlyList<string> textos, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<float[]>>(textos.Select(_ => new float[3]).ToList());
    }

    private sealed class EmbeddingQuebrado : IGeradorEmbedding
    {
        public Task<float[]> GerarAsync(string texto, CancellationToken ct) =>
            Task.FromException<float[]>(new InvalidOperationException("provedor não configurado"));

        public Task<IReadOnlyList<float[]>> GerarLoteAsync(IReadOnlyList<string> textos, CancellationToken ct) =>
            Task.FromException<IReadOnlyList<float[]>>(new InvalidOperationException("provedor não configurado"));
    }

    private sealed class FonteVazia : IFonteConhecimento
    {
        public string Pasta => "base-test";
        public Task<IReadOnlyList<DocumentoFonte>> ListarAsync(CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<DocumentoFonte>>(Array.Empty<DocumentoFonte>());
    }

    private sealed class FonteFixa : IFonteConhecimento
    {
        public string Pasta => "base-test";
        public Task<IReadOnlyList<DocumentoFonte>> ListarAsync(CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<DocumentoFonte>>(new[]
            {
                new DocumentoFonte(
                    "lei.md",
                    null,
                    "Art. 1º Esta Lei estabelece incentivos fiscais para a inovação tecnológica.\n" +
                    "Art. 2º Para os fins desta Lei considera-se pesquisa o trabalho experimental.")
            });
    }

    private sealed class RepositorioVazio : INormaRepositorio
    {
        public Task<bool> HaTrechosAsync(CancellationToken ct) => Task.FromResult(false);

        public Task SubstituirDocumentoAsync(string documento, string? arquivo, IReadOnlyList<TrechoParaIndexar> trechos, CancellationToken ct) =>
            Task.CompletedTask;

        public Task<IReadOnlyList<ContagemDocumento>> ContarPorDocumentoAsync(CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<ContagemDocumento>>(Array.Empty<ContagemDocumento>());

        public Task<IReadOnlyList<TrechoRecuperado>> BuscarAsync(float[] vetorConsulta, int quantidade, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<TrechoRecuperado>>(Array.Empty<TrechoRecuperado>());
    }

    private sealed class RepositorioCaptura : INormaRepositorio
    {
        public int TrechosGravados { get; private set; }

        public Task<bool> HaTrechosAsync(CancellationToken ct) => Task.FromResult(TrechosGravados > 0);

        public Task SubstituirDocumentoAsync(string documento, string? arquivo, IReadOnlyList<TrechoParaIndexar> trechos, CancellationToken ct)
        {
            TrechosGravados = trechos.Count;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<ContagemDocumento>> ContarPorDocumentoAsync(CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<ContagemDocumento>>(
                TrechosGravados > 0
                    ? new[] { new ContagemDocumento("lei.md", TrechosGravados, DateTimeOffset.UtcNow) }
                    : Array.Empty<ContagemDocumento>());

        public Task<IReadOnlyList<TrechoRecuperado>> BuscarAsync(float[] vetorConsulta, int quantidade, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<TrechoRecuperado>>(Array.Empty<TrechoRecuperado>());
    }
}
