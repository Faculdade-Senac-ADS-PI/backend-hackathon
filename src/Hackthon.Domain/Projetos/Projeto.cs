namespace Hackthon.Domain.Projetos;

/// <summary>
/// Proposta de projeto enviada em PDF por um colaborador (proponente) para apuração de
/// elegibilidade na Lei do Bem. A IA avalia os 5 critérios de Frascati e o analista delibera.
/// </summary>
public sealed class Projeto
{
    public Guid Id { get; private set; }
    public string Protocolo { get; private set; } = null!;
    public string Titulo { get; private set; } = null!;
    public string Setor { get; private set; } = null!;
    public StatusProjeto Status { get; private set; }
    public Guid ProponenteId { get; private set; }
    public int VersaoEnvio { get; private set; }

    // Documento enviado
    public string NomeArquivoOriginal { get; private set; } = null!;
    public string CaminhoArquivo { get; private set; } = null!;
    public long TamanhoBytes { get; private set; }
    public string TextoExtraido { get; private set; } = null!;

    // Avaliação do agente de IA (5 critérios de Frascati)
    public int? NotaIa { get; private set; }
    public int? NotaNovidade { get; private set; }
    public int? NotaCriatividade { get; private set; }
    public int? NotaSistematico { get; private set; }
    public int? NotaTransferibilidade { get; private set; }
    public int? NotaIncerteza { get; private set; }
    public string? ResumoIa { get; private set; }
    public string? ParecerIa { get; private set; }
    public string? RecomendacaoIa { get; private set; }
    public string? JustificativasIaJson { get; private set; }
    public string? OrigemAnaliseIa { get; private set; }
    public DateTimeOffset? AnalisadoEm { get; private set; }

    // Deliberação do analista
    public string? JustificativaAnalista { get; private set; }
    public Guid? AnalistaId { get; private set; }
    public DateTimeOffset? DecididoEm { get; private set; }

    public DateTimeOffset CriadoEm { get; private set; }
    public DateTimeOffset AtualizadoEm { get; private set; }

    private Projeto()
    {
    }

    public static Projeto Criar(
        string protocolo,
        string titulo,
        string setor,
        Guid proponenteId,
        DocumentoProposta documento,
        DateTimeOffset agora)
    {
        if (string.IsNullOrWhiteSpace(protocolo))
            throw new ArgumentException("O protocolo é obrigatório.", nameof(protocolo));
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("O título do projeto é obrigatório.", nameof(titulo));
        if (string.IsNullOrWhiteSpace(setor))
            throw new ArgumentException("O setor é obrigatório.", nameof(setor));
        if (proponenteId == Guid.Empty)
            throw new ArgumentException("O proponente responsável é obrigatório.", nameof(proponenteId));
        ArgumentNullException.ThrowIfNull(documento);

        var projeto = new Projeto
        {
            Id = Guid.NewGuid(),
            Protocolo = protocolo.Trim(),
            Titulo = titulo.Trim(),
            Setor = setor.Trim(),
            ProponenteId = proponenteId,
            VersaoEnvio = 1,
            Status = StatusProjeto.ProcessandoIa,
            CriadoEm = agora,
            AtualizadoEm = agora
        };
        projeto.AplicarDocumento(documento);
        return projeto;
    }

    /// <summary>Grava a avaliação da IA e coloca o projeto na fila do analista.</summary>
    public void RegistrarAnaliseIa(AvaliacaoFrascati avaliacao, DateTimeOffset agora)
    {
        ArgumentNullException.ThrowIfNull(avaliacao);
        if (Status != StatusProjeto.ProcessandoIa)
            throw new InvalidOperationException("A análise da IA só pode ser registrada enquanto o projeto está em processamento.");

        NotaNovidade = avaliacao.NotaNovidade;
        NotaCriatividade = avaliacao.NotaCriatividade;
        NotaSistematico = avaliacao.NotaSistematico;
        NotaTransferibilidade = avaliacao.NotaTransferibilidade;
        NotaIncerteza = avaliacao.NotaIncerteza;
        NotaIa = avaliacao.NotaTotal;
        ResumoIa = avaliacao.Resumo;
        ParecerIa = avaliacao.Parecer;
        RecomendacaoIa = avaliacao.Recomendacao;
        JustificativasIaJson = avaliacao.JustificativasJson;
        OrigemAnaliseIa = avaliacao.Origem;
        AnalisadoEm = agora;
        Status = StatusProjeto.AguardandoAnalista;
        AtualizadoEm = agora;
    }

    /// <summary>Decisão humana do analista: Aprovado, Reprovado ou Reanalise.</summary>
    public void Deliberar(StatusProjeto decisao, string? justificativa, Guid analistaId, DateTimeOffset agora)
    {
        if (decisao is not (StatusProjeto.Aprovado or StatusProjeto.Reprovado or StatusProjeto.Reanalise))
            throw new ArgumentException("A decisão deve ser Aprovado, Reprovado ou Reanalise.", nameof(decisao));
        if (Status != StatusProjeto.AguardandoAnalista)
            throw new InvalidOperationException("Só é possível deliberar projetos que aguardam o analista.");
        if (decisao != StatusProjeto.Aprovado && string.IsNullOrWhiteSpace(justificativa))
            throw new ArgumentException("Explique o motivo da reprovação ou o que precisa ser complementado.", nameof(justificativa));
        if (analistaId == Guid.Empty)
            throw new ArgumentException("O analista responsável é obrigatório.", nameof(analistaId));

        Status = decisao;
        JustificativaAnalista = string.IsNullOrWhiteSpace(justificativa) ? null : justificativa.Trim();
        AnalistaId = analistaId;
        DecididoEm = agora;
        AtualizadoEm = agora;
    }

    /// <summary>Após um pedido de reanálise, o proponente envia uma nova versão do PDF.</summary>
    public void SubstituirDocumento(DocumentoProposta documento, DateTimeOffset agora)
    {
        ArgumentNullException.ThrowIfNull(documento);
        if (Status != StatusProjeto.Reanalise)
            throw new InvalidOperationException("Só é possível reenviar o documento quando o analista solicita reanálise.");

        AplicarDocumento(documento);
        VersaoEnvio++;
        NotaIa = NotaNovidade = NotaCriatividade = NotaSistematico = NotaTransferibilidade = NotaIncerteza = null;
        ResumoIa = ParecerIa = RecomendacaoIa = JustificativasIaJson = OrigemAnaliseIa = null;
        AnalisadoEm = null;
        Status = StatusProjeto.ProcessandoIa;
        AtualizadoEm = agora;
    }

    private void AplicarDocumento(DocumentoProposta documento)
    {
        NomeArquivoOriginal = documento.NomeOriginal;
        CaminhoArquivo = documento.Caminho;
        TamanhoBytes = documento.TamanhoBytes;
        TextoExtraido = documento.TextoExtraido;
    }
}

/// <summary>Metadados e texto do PDF já armazenado.</summary>
public sealed record DocumentoProposta
{
    public string NomeOriginal { get; }
    public string Caminho { get; }
    public long TamanhoBytes { get; }
    public string TextoExtraido { get; }

    public DocumentoProposta(string nomeOriginal, string caminho, long tamanhoBytes, string textoExtraido)
    {
        if (string.IsNullOrWhiteSpace(nomeOriginal))
            throw new ArgumentException("O nome do arquivo é obrigatório.", nameof(nomeOriginal));
        if (string.IsNullOrWhiteSpace(caminho))
            throw new ArgumentException("O caminho do arquivo armazenado é obrigatório.", nameof(caminho));
        if (tamanhoBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(tamanhoBytes), "O arquivo está vazio.");

        NomeOriginal = nomeOriginal.Trim();
        Caminho = caminho;
        TamanhoBytes = tamanhoBytes;
        TextoExtraido = textoExtraido ?? string.Empty;
    }
}
