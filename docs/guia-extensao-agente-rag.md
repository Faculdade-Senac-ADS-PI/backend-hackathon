# Guia de Extensão: Agente RAG para Análise da Lei do Bem

Este guia detalha o ponto de encaixe arquitetural para implementar a inteligência do sistema de apoio à análise de elegibilidade (Lei nº 11.196/2005 e Manual de Frascati) via **Retrieval-Augmented Generation (RAG)** com **pgvector**.

---

## 1. Princípio Fundamental de Negócio

> **O agente apoia, a decisão é humana.**
> A ferramenta serve para acelerar a leitura das normas pelo analista interno de P&D, encontrar artigos correspondentes e montar a fundamentação da não rotina. A inteligência artificial nunca decide o enquadramento sozinha e deve sempre citar o documento e trecho de referência. O campo `DecisaoAutomatica` na resposta deve ser sempre retornado como `false`.

---

## 2. Ponto de Encaixe Arquitetural (Contrato Atual)

O contrato está definido na camada de aplicação em `src/Hackthon.Application/Agente/IAgenteElegibilidade.cs`:

```csharp
public interface IAgenteElegibilidade
{
    ContratoAgente DescreverContrato();
    Task<Resultado<ConsultaAgenteResposta>> ConsultarAsync(
        ConsultaAgenteComando comando, 
        CancellationToken ct);
}

public sealed record ConsultaAgenteComando(Guid? ProjetoId, string? Pergunta);

public sealed record TrechoNorma(string Documento, string Trecho, string? Referencia);

public sealed record ConsultaAgenteResposta(
    string RespostaApoio,
    IReadOnlyList<TrechoNorma> Citacoes,
    bool DecisaoAutomatica);
```

Atualmente, a classe `AgenteElegibilidadeNaoConfigurado` (`Hackthon.Infrastructure/Agente`) cumpre o contrato devolvendo **HTTP 501 (Not Implemented)**.

---

## 3. Passo a Passo para Implementar o Agente Real

### Passo 1: Modelagem e Ingestão de Embeddings no PostgreSQL

O container PostgreSQL já possui a extensão `vector` habilitada pelo script `docker/postgres/init.sql`.

1. Crie a entidade de fragmentos normativos (ex.: `DocumentoNormativo`):
```csharp
public sealed class DocumentoNormativo
{
    public Guid Id { get; set; }
    public string NomeDocumento { get; set; } = null!; // Ex: "Manual de Frascati", "Lei 11.196"
    public string Trecho { get; set; } = null!;
    public string? ArtigoOuCapitulo { get; set; }
    public Pgvector.Vector Embedding { get; set; } = null!; // pgvector
}
```

2. Adicione o pacote `Pgvector.EntityFrameworkCore` no projeto `Hackthon.Infrastructure.csproj`.
3. Crie e aplique uma migration do EF Core para criar a tabela com coluna vetorial (ex.: `vector(1536)` para OpenAI `text-embedding-3-small` ou `vector(768)` para modelos open-source/Gemini).
4. Crie um script ou comando de ingestão (chunking + embedding) dos documentos legais (Decreto 5.798/2006, Guia da Lei do Bem do MCTI, Instrução Normativa da RFB e Manual de Frascati).

### Passo 2: Implementar a Busca Semântica

Na busca por similaridade de cosseno utilizando o operador `<=>` do pgvector:

```csharp
var perguntaEmbedding = await _embeddingService.GerarAsync(comando.Pergunta);

var trechosRelevantes = await _db.DocumentosNormativos
    .OrderBy(d => d.Embedding.CosineDistance(perguntaEmbedding))
    .Take(4)
    .ToListAsync(ct);
```

### Passo 3: Criar o Agente em `Hackthon.Infrastructure/Agente/`

Implemente `IAgenteElegibilidade`:

```csharp
namespace Hackthon.Infrastructure.Agente;

public sealed class AgenteElegibilidadeRag : IAgenteElegibilidade
{
    private readonly ILlmClient _llm;
    private readonly HackthonDbContext _db;

    public ContratoAgente DescreverContrato() => new(
        Estado: "operacional",
        Responsabilidade: "Apoio à decisão com citações de normas técnicas e jurídicas.",
        ComoPlugar: "Agente conectado ao modelo LLM e base vetorial pgvector.",
        EntradasPrevistas: ["projetoId", "pergunta do analista"],
        Saida: "Parecer de apoio, citações com referências e decisaoAutomatica: false");

    public async Task<Resultado<ConsultaAgenteResposta>> ConsultarAsync(
        ConsultaAgenteComando comando, 
        CancellationToken ct)
    {
        // 1. Validar pergunta
        // 2. Recuperar trechos via similaridade semântica (RAG)
        // 3. Montar prompt com contexto das normas e projeto
        // 4. Invocar LLM com instrução para citar fontes
        // 5. Retornar ConsultaAgenteResposta com DecisaoAutomatica = false
    }
}
```

### Passo 4: Registrar no Injetor de Dependências

Em `src/Hackthon.Infrastructure/Dependencias.cs`, substitua o registro:

```csharp
// De:
services.AddSingleton<IAgenteElegibilidade, AgenteElegibilidadeNaoConfigurado>();

// Para:
services.AddScoped<IAgenteElegibilidade, AgenteElegibilidadeRag>();
```

Nenhuma linha do `AgenteController` ou de `Hackthon.Application` precisará ser modificada (Respeitando o **Open/Closed Principle** e **Liskov Substitution Principle**).
