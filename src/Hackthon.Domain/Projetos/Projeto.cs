namespace Hackthon.Domain.Projetos;

/// <summary>
/// Proposta de projeto enviada em PDF e/ou CSV por um colaborador (proponente) para apuração de
/// elegibilidade na Lei do Bem. A IA avalia os 5 critérios de Frascati e o analista delibera.
/// </summary>
public sealed class Projeto
{
    private readonly List<ArquivoProjetoItem> _arquivos = new();

    public Guid Id { get; private set; }
    public string Protocolo { get; private set; } = null!;
    public string Titulo { get; private set; } = null!;
    public string Setor { get; private set; } = null!;
    public StatusProjeto Status { get; private set; }
    public Guid ProponenteId { get; private set; }
    public int VersaoEnvio { get; private set; }

    // Documento principal/resumo (compatibilidade retroativa)
    public string NomeArquivoOriginal { get; private set; } = null!;
    public string CaminhoArquivo { get; private set; } = null!;
    public long TamanhoBytes { get; private set; }
    public string TextoExtraido { get; private set; } = null!;

    // Coleção de múltiplos arquivos anexados (PDFs e/ou CSVs)
    public IReadOnlyCollection<ArquivoProjetoItem> Arquivos => _arquivos.AsReadOnly();

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

    // Deliberação do analista (classificação + rastreabilidade — 4 classes do desafio)
    public string? JustificativaAnalista { get; private set; }
    public Guid? AnalistaId { get; private set; }
    public DateTimeOffset? DecididoEm { get; private set; }

    /// <summary>Com ressalvas: o que as evidências sustentam dentro do escopo pretendido.</summary>
    public string? RecorteSustentado { get; private set; }

    /// <summary>Com ressalvas: a limitação técnica ou de validação concreta.</summary>
    public string? LimitacaoTecnica { get; private set; }

    /// <summary>Com ressalvas: a evidência necessária para resolver a ressalva.</summary>
    public string? EvidenciaNecessaria { get; private set; }

    /// <summary>Evidência insuficiente: o elo ausente que impede a conclusão.</summary>
    public string? EloAusente { get; private set; }

    /// <summary>Evidência insuficiente: evidências adicionais a solicitar.</summary>
    public string? EvidenciasAdicionais { get; private set; }

    /// <summary>Evidências utilizadas na conclusão, identificadas por ID (uma por linha/vírgula).</summary>
    public string? EvidenciasUtilizadas { get; private set; }

    /// <summary>Evidências contrárias ou contraditórias, incluindo divergências entre depoimento e documento.</summary>
    public string? EvidenciasContrarias { get; private set; }

    /// <summary>Lacunas identificadas no dossiê.</summary>
    public string? Lacunas { get; private set; }

    /// <summary>Regra/critério aplicado para decidir (rastreabilidade: "qual regra foi usada").</summary>
    public string? RegraAplicada { get; private set; }

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
        IReadOnlyList<DocumentoProposta> documentos,
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
        if (documentos is null || documentos.Count == 0)
            throw new ArgumentException("Ao menos um documento deve ser fornecido.", nameof(documentos));

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
        projeto.AplicarDocumentos(documentos, agora);
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

    /// <summary>
    /// Decisão humana do analista: uma das quatro classes do desafio
    /// (Elegível, Com ressalvas, Não elegível, Evidência insuficiente) ou pedido de Reanálise.
    /// </summary>
    public void Deliberar(StatusProjeto decisao, DeliberacaoAnalista dados, Guid analistaId, DateTimeOffset agora)
    {
        ArgumentNullException.ThrowIfNull(dados);
        if (decisao is not (StatusProjeto.Elegivel or StatusProjeto.ComRessalvas
            or StatusProjeto.NaoElegivel or StatusProjeto.EvidenciaInsuficiente or StatusProjeto.Reanalise))
            throw new ArgumentException(
                "A decisão deve ser Elegível, Com ressalvas, Não elegível, Evidência insuficiente ou Reanalise.",
                nameof(decisao));
        if (Status != StatusProjeto.AguardandoAnalista)
            throw new InvalidOperationException("Só é possível deliberar projetos que aguardam o analista.");

        var justificativa = dados.Justificativa?.Trim();
        if (string.IsNullOrWhiteSpace(justificativa))
            throw new ArgumentException(
                "A justificativa da conclusão é obrigatória: registre o motivo da decisão com fontes.",
                nameof(dados));
        if (analistaId == Guid.Empty)
            throw new ArgumentException("O analista responsável é obrigatório.", nameof(analistaId));

        // Guardas de domínio por classe (a camada de aplicação devolve 400 amigável antes de chegar aqui).
        if (decisao != StatusProjeto.Reanalise)
        {
            if (decisao == StatusProjeto.ComRessalvas &&
                (string.IsNullOrWhiteSpace(dados.RecorteSustentado) ||
                 string.IsNullOrWhiteSpace(dados.LimitacaoTecnica) ||
                 string.IsNullOrWhiteSpace(dados.EvidenciaNecessaria)))
                throw new ArgumentException(
                    "Com ressalvas exige recorte sustentado, limitação específica e evidência necessária.",
                    nameof(dados));

            if (decisao == StatusProjeto.EvidenciaInsuficiente &&
                (string.IsNullOrWhiteSpace(dados.EloAusente) ||
                 string.IsNullOrWhiteSpace(dados.EvidenciasAdicionais)))
                throw new ArgumentException(
                    "Evidência insuficiente exige o elo ausente e as evidências adicionais a solicitar.",
                    nameof(dados));

            if (string.IsNullOrWhiteSpace(dados.EvidenciasUtilizadas))
                throw new ArgumentException(
                    "Informe as evidências utilizadas na conclusão, identificadas por ID.",
                    nameof(dados));
        }

        if (decisao == StatusProjeto.Reanalise)
        {
            // Reanálise é estado de fluxo, não classificação: os campos de classe ficam vazios
            // até que o proponente envie a nova versão e o analista classifique.
            RecorteSustentado = null;
            LimitacaoTecnica = null;
            EvidenciaNecessaria = null;
            EloAusente = null;
            EvidenciasAdicionais = null;
            EvidenciasUtilizadas = null;
            EvidenciasContrarias = null;
            Lacunas = null;
        }
        else
        {
            RecorteSustentado = Normalizar(dados.RecorteSustentado);
            LimitacaoTecnica = Normalizar(dados.LimitacaoTecnica);
            EvidenciaNecessaria = Normalizar(dados.EvidenciaNecessaria);
            EloAusente = Normalizar(dados.EloAusente);
            EvidenciasAdicionais = Normalizar(dados.EvidenciasAdicionais);
            EvidenciasUtilizadas = Normalizar(dados.EvidenciasUtilizadas);
            EvidenciasContrarias = Normalizar(dados.EvidenciasContrarias);
            Lacunas = Normalizar(dados.Lacunas);
        }

        RegraAplicada = Normalizar(dados.RegraAplicada);
        Status = decisao;
        JustificativaAnalista = justificativa;
        AnalistaId = analistaId;
        DecididoEm = agora;
        AtualizadoEm = agora;
    }

    private static string? Normalizar(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

    /// <summary>Após um pedido de reanálise, o proponente envia uma nova versão dos documentos.</summary>
    public void SubstituirDocumentos(IReadOnlyList<DocumentoProposta> documentos, DateTimeOffset agora)
    {
        if (documentos is null || documentos.Count == 0)
            throw new ArgumentException("Ao menos um documento deve ser fornecido.", nameof(documentos));
        if (Status != StatusProjeto.Reanalise)
            throw new InvalidOperationException("Só é possível reenviar o documento quando o analista solicita reanálise.");

        AplicarDocumentos(documentos, agora);
        VersaoEnvio++;
        NotaIa = NotaNovidade = NotaCriatividade = NotaSistematico = NotaTransferibilidade = NotaIncerteza = null;
        ResumoIa = ParecerIa = RecomendacaoIa = JustificativasIaJson = OrigemAnaliseIa = null;
        AnalisadoEm = null;
        Status = StatusProjeto.ProcessandoIa;
        AtualizadoEm = agora;
    }

    private void AplicarDocumentos(IReadOnlyList<DocumentoProposta> documentos, DateTimeOffset agora)
    {
        _arquivos.Clear();
        long tamanhoTotal = 0;
        var textosAgregados = new System.Text.StringBuilder();

        for (var i = 0; i < documentos.Count; i++)
        {
            var doc = documentos[i];
            var extensao = Path.GetExtension(doc.NomeOriginal).ToLowerInvariant();
            var item = new ArquivoProjetoItem(
                id: Guid.NewGuid(),
                projetoId: Id,
                nomeOriginal: doc.NomeOriginal,
                caminho: doc.Caminho,
                tamanhoBytes: doc.TamanhoBytes,
                extensao: extensao,
                criadoEm: agora);

            _arquivos.Add(item);
            tamanhoTotal += doc.TamanhoBytes;

            textosAgregados.AppendLine($"\n=== ARQUIVO [{i + 1}/{documentos.Count}]: {doc.NomeOriginal} ({extensao.ToUpperInvariant().TrimStart('.')}) ===");
            textosAgregados.AppendLine(doc.TextoExtraido);
        }

        var primeiro = documentos[0];
        NomeArquivoOriginal = documentos.Count == 1 
            ? primeiro.NomeOriginal 
            : $"{primeiro.NomeOriginal} (+{documentos.Count - 1} arquivo(s))";
        CaminhoArquivo = primeiro.Caminho;
        TamanhoBytes = tamanhoTotal;
        TextoExtraido = textosAgregados.ToString().Trim();
    }
}

/// <summary>Metadados e texto do arquivo já processado e armazenado.</summary>
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
