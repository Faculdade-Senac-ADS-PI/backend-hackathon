using System.Security.Cryptography;
using System.Text;
using Hackthon.Application.Acesso;
using Hackthon.Application.Comum;
using Hackthon.Domain.Projetos;
using Hackthon.Domain.Usuarios;
using Microsoft.Extensions.Logging;

namespace Hackthon.Application.Projetos;

/// <summary>
/// Recebe múltiplos arquivos (PDF e CSV) do proponente, extrai seus textos, armazena,
/// aciona o agente de IA (5 critérios de Frascati) e grava o projeto na fila do analista.
/// </summary>
public sealed class EnviarPropostaServico : IEnviarProposta
{
    private static readonly byte[] AssinaturaPdf = Encoding.ASCII.GetBytes("%PDF-");

    private readonly IProjetoRepositorio _projetos;
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IArmazenamentoArquivos _armazenamento;
    private readonly IExtratorTextoArquivo _extrator;
    private readonly IAnalisadorElegibilidade _analisador;
    private readonly IUnidadeDeTrabalho _unidade;
    private readonly IRelogio _relogio;
    private readonly ILogger<EnviarPropostaServico> _logger;

    public EnviarPropostaServico(
        IProjetoRepositorio projetos,
        IUsuarioRepositorio usuarios,
        IArmazenamentoArquivos armazenamento,
        IExtratorTextoArquivo extrator,
        IAnalisadorElegibilidade analisador,
        IUnidadeDeTrabalho unidade,
        IRelogio relogio,
        ILogger<EnviarPropostaServico> logger)
    {
        _projetos = projetos;
        _usuarios = usuarios;
        _armazenamento = armazenamento;
        _extrator = extrator;
        _analisador = analisador;
        _unidade = unidade;
        _relogio = relogio;
        _logger = logger;
    }

    public async Task<Resultado<ProjetoProponenteResposta>> EnviarAsync(EnviarPropostaComando comando, CancellationToken ct)
    {
        var erroProponente = await ValidarProponenteAsync(comando.ProponenteId, ct);
        if (erroProponente is not null)
            return erroProponente;

        var titulo = comando.Titulo?.Trim() ?? string.Empty;
        if (titulo.Length is < 5 or > 200)
            return Falha(400, "Título inválido", "O título deve ter entre 5 e 200 caracteres.");

        var setor = comando.Setor?.Trim() ?? string.Empty;
        if (setor.Length is < 2 or > 100)
            return Falha(400, "Setor inválido", "Informe o setor da empresa ao qual a proposta se destina.");

        if (comando.Arquivos is null || comando.Arquivos.Count == 0)
            return Falha(400, "Arquivos ausentes", "Envie pelo menos um arquivo (PDF ou CSV) com a proposta e dados do projeto.");

        var resultadoDocs = await ProcessarDocumentosAsync(comando.Arquivos, ct);
        if (resultadoDocs.Erro is not null)
            return resultadoDocs.Erro;

        var documentos = resultadoDocs.Valor!;
        var agora = _relogio.Agora();
        var protocolo = await GerarProtocoloUnicoAsync(agora.Year, ct);
        var projeto = Projeto.Criar(protocolo, titulo, setor, comando.ProponenteId, documentos, agora);

        var erroIa = await AvaliarAsync(projeto, ct);
        if (erroIa is not null)
        {
            RemoverArquivos(documentos);
            return erroIa;
        }

        try
        {
            await _projetos.AdicionarAsync(projeto, ct);
            await _unidade.SalvarAsync(ct);
        }
        catch
        {
            RemoverArquivos(documentos);
            throw;
        }

        var criado = MapeadorProjetos.ParaProponente(projeto);
        return Resultado<ProjetoProponenteResposta>.Ok(criado);
    }

    public async Task<Resultado<ProjetoProponenteResposta>> ReenviarAsync(ReenviarPropostaComando comando, CancellationToken ct)
    {
        var erroProponente = await ValidarProponenteAsync(comando.ProponenteId, ct);
        if (erroProponente is not null)
            return erroProponente;

        var projeto = await _projetos.ObterPorIdAsync(comando.ProjetoId, ct);
        if (projeto is null || projeto.ProponenteId != comando.ProponenteId)
            return Falha(404, "Projeto não encontrado", "Não localizamos esta proposta entre os seus envios.");

        if (projeto.Status != StatusProjeto.Reanalise)
            return Falha(409, "Reenvio não permitido", "Só é possível reenviar o documento quando o analista solicita reanálise.");

        if (comando.Arquivos is null || comando.Arquivos.Count == 0)
            return Falha(400, "Arquivos ausentes", "Envie pelo menos um arquivo (PDF ou CSV) atualizado da proposta.");

        var caminhosAnteriores = projeto.Arquivos.Select(a => a.Caminho).ToList();
        if (caminhosAnteriores.Count == 0 && !string.IsNullOrWhiteSpace(projeto.CaminhoArquivo))
        {
            caminhosAnteriores.Add(projeto.CaminhoArquivo);
        }

        var resultadoDocs = await ProcessarDocumentosAsync(comando.Arquivos, ct);
        if (resultadoDocs.Erro is not null)
            return resultadoDocs.Erro;

        var documentos = resultadoDocs.Valor!;
        projeto.SubstituirDocumentos(documentos, _relogio.Agora());

        var erroIa = await AvaliarAsync(projeto, ct);
        if (erroIa is not null)
        {
            RemoverArquivos(documentos);
            return erroIa;
        }

        try
        {
            await _unidade.SalvarAsync(ct);
        }
        catch
        {
            RemoverArquivos(documentos);
            throw;
        }

        foreach (var caminho in caminhosAnteriores)
        {
            _armazenamento.Remover(caminho);
        }

        return Resultado<ProjetoProponenteResposta>.Ok(MapeadorProjetos.ParaProponente(projeto));
    }

    private async Task<Resultado<ProjetoProponenteResposta>?> ValidarProponenteAsync(Guid proponenteId, CancellationToken ct)
    {
        var usuario = await _usuarios.ObterPorIdAsync(proponenteId, ct);
        if (usuario is null)
            return Falha(404, "Proponente não encontrado", "O cadastro do proponente não foi localizado.");
        if (!usuario.Ativo)
            return Falha(403, "Cadastro inativo", "Sua conta está desativada. Não é possível enviar propostas.");
        if (usuario.Papel != Papel.Proponente)
            return Falha(403, "Acesso negado", "Somente usuários com papel Proponente podem enviar propostas.");
        return null;
    }

    private async Task<(List<DocumentoProposta>? Valor, Resultado<ProjetoProponenteResposta>? Erro)> ProcessarDocumentosAsync(
        IReadOnlyList<ArquivoEntradaComando> entradas,
        CancellationToken ct)
    {
        long tamanhoTotal = 0;
        var documentosProcessados = new List<DocumentoProposta>();

        foreach (var entrada in entradas)
        {
            if (entrada.Conteudo is null || entrada.TamanhoBytes <= 0)
                return (null, Falha(400, "Arquivo inválido", $"O arquivo '{entrada.NomeArquivo}' está vazio ou ausente."));

            if (entrada.TamanhoBytes > RegrasDocumento.TamanhoMaximoBytesPorArquivo)
                return (null, Falha(413, "Arquivo muito grande", $"O arquivo '{entrada.NomeArquivo}' excede o limite individual de 15 MB."));

            tamanhoTotal += entrada.TamanhoBytes;
            if (tamanhoTotal > RegrasDocumento.TamanhoMaximoBytesTotal)
                return (null, Falha(413, "Volume total excedido", "O tamanho somado dos arquivos não pode ultrapassar 30 MB."));

            var nome = Path.GetFileName(entrada.NomeArquivo ?? string.Empty).Trim();
            var extensao = Path.GetExtension(nome).ToLowerInvariant();

            if (extensao is not (".pdf" or ".csv"))
            {
                return (null, Falha(415, "Formato não suportado", $"O arquivo '{nome}' possui extensão '{extensao}'. Somente arquivos .pdf e .csv são permitidos."));
            }

            if (nome.Length > 255)
                nome = nome[^255..];

            await using var memoria = new MemoryStream();
            await entrada.Conteudo.CopyToAsync(memoria, ct);
            var bytes = memoria.ToArray();

            // Validação de assinatura mágica para PDF
            if (extensao == ".pdf")
            {
                if (bytes.Length < AssinaturaPdf.Length || !bytes.AsSpan(0, AssinaturaPdf.Length).SequenceEqual(AssinaturaPdf))
                {
                    return (null, Falha(415, "Formato não suportado", $"O arquivo '{nome}' não é um documento PDF válido."));
                }
            }

            string texto;
            try
            {
                using var leitura = new MemoryStream(bytes, writable: false);
                texto = await _extrator.ExtrairAsync(leitura, extensao, ct);
            }
            catch (DocumentoInvalidoException ex)
            {
                return (null, Falha(422, "Arquivo ilegível", $"Erro ao processar '{nome}': {ex.Message}"));
            }

            using var gravacao = new MemoryStream(bytes, writable: false);
            var caminho = await _armazenamento.SalvarAsync(gravacao, extensao, ct);
            documentosProcessados.Add(new DocumentoProposta(nome, caminho, bytes.LongLength, texto));
        }

        var totalCaracteres = documentosProcessados.Sum(d => d.TextoExtraido.Trim().Length);
        if (totalCaracteres < RegrasDocumento.MinimoCaracteresTexto)
        {
            RemoverArquivos(documentosProcessados);
            return (null, Falha(
                422,
                "Texto insuficiente nos documentos",
                "O conteúdo textual total extraído dos arquivos foi muito reduzido. Se algum PDF for digitalizado como imagem, forneça uma versão com texto selecionável ou complemente com planilha CSV."));
        }

        return (documentosProcessados, null);
    }

    private void RemoverArquivos(IEnumerable<DocumentoProposta> docs)
    {
        foreach (var doc in docs)
        {
            try
            {
                _armazenamento.Remover(doc.Caminho);
            }
            catch
            {
                // Ignora falhas de limpeza
            }
        }
    }

    private async Task<Resultado<ProjetoProponenteResposta>?> AvaliarAsync(Projeto projeto, CancellationToken ct)
    {
        try
        {
            var avaliacao = await _analisador.AvaliarAsync(
                new PropostaParaAvaliacao(projeto.Titulo, projeto.Setor, projeto.TextoExtraido),
                ct);
            projeto.RegistrarAnaliseIa(avaliacao, _relogio.Agora());
            return null;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Falha ao avaliar a proposta {Protocolo} com o agente de IA.", projeto.Protocolo);
            return Falha(503, "Agente de IA indisponível", "Não foi possível avaliar a proposta agora. Tente novamente em instantes.");
        }
    }

    private async Task<string> GerarProtocoloUnicoAsync(int ano, CancellationToken ct)
    {
        for (var tentativa = 0; tentativa < 10; tentativa++)
        {
            var protocolo = $"PROP-{ano}-{RandomNumberGenerator.GetInt32(1000, 10000):D4}";
            if (!await _projetos.ExisteProtocoloAsync(protocolo, ct))
                return protocolo;
        }

        return $"PROP-{ano}-{RandomNumberGenerator.GetInt32(10000, 100000):D5}";
    }

    private static Resultado<ProjetoProponenteResposta> Falha(int status, string titulo, string detalhe) =>
        Resultado<ProjetoProponenteResposta>.Falha(status, titulo, detalhe);
}
