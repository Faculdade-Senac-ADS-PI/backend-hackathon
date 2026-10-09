# Base de normas (RAG) — preparação, uso e verificação

Este documento explica como alimentar e usar a **base de conhecimento** que apoia o agente de
elegibilidade na Lei do Bem. O agente **não decide**: ele recupera trechos das normas oficiais e
cita as fontes para o analista. A decisão continua sendo do analista.

> Regra de ouro: a base só recebe **documentos oficiais fornecidos pelo usuário**. O código não
> contém texto de norma inventado. Pasta vazia ⇒ a indexação é recusada (HTTP 409) e a avaliação
> continua funcionando normalmente, sem RAG.

---

## 1. Onde ficam os documentos

```
backend-hackathon/
└── base-conhecimento/
    ├── normas/      ← ÚNICA pasta indexada (PDF, MD, TXT)
    └── exemplos/    ← material de apoio; NÃO é indexado
```

- Apenas arquivos `.pdf`, `.md` e `.txt` são lidos, inclusive em subpastas de `normas/`.
- Arquivos ocultos (começando com `.`) e arquivos iniciados por `_` são **ignorados**
  (use `_` para anotações que não devem virar trecho).
- Não coloque instruções em `.md`/`.txt` dentro de `normas/`: elas seriam indexadas como norma.
  As instruções ficam neste arquivo, fora da pasta.

Com o compose, a pasta `base-conhecimento` é montada em `/app/base-conhecimento` (somente leitura),
então basta adicionar os arquivos no host e reindexar — **sem rebuildar a imagem**.

---

## 2. Como indexar (ADM)

1. Suba a stack:

   ```bash
   docker compose up -d --build
   ```

2. Copie os documentos oficiais para `backend-hackathon/base-conhecimento/normas/`
   (ex.: a Lei 11.196/2005, o Decreto 5.798/2006, a IN RFB 1.187/2011, o Manual de Frascati).

3. Autentique como ADM e dispare a indexação (idempotente — reindexar substitui os trechos):

   ```bash
   TOKEN=$(curl -s -X POST http://127.0.0.1:5088/api/auth/entrar \
     -H "Content-Type: application/json" \
     -d '{"email":"helena.borges@hackthon.dev","senha":"Adm@2026"}' | jq -r .token)

   curl -s -X POST http://127.0.0.1:5088/api/normas/reindexar \
     -H "Authorization: Bearer $TOKEN" | jq
   ```

4. Confira o estado:

   ```bash
   curl -s http://127.0.0.1:5088/api/normas/estado \
     -H "Authorization: Bearer $TOKEN" | jq
   ```

### O que a indexação faz

- Lê cada documento e o divide em trechos: **por artigo** (leis/regulamentos) ou **por seção**
  (manuais), com pequena sobreposição.
- Gera os vetores (embeddings) via provedor de IA configurado.
- Grava documento + referência interna (ex.: `Art. 12`, `Seção 3`) + página + vetor em PostgreSQL
  com **pgvector**, criando índice **HNSW** para busca por similaridade de cosseno.

---

## 3. Configuração

Seção `Rag` (appsettings ou variáveis `Rag__*`):

| Chave | Padrão | Descrição |
| :--- | :--- | :--- |
| `Habilitado` | `true` | Liga/desliga o uso da base na avaliação e na consulta |
| `PastaBase` | `base-conhecimento/normas` | Pasta indexada (relativa à raiz da API) |
| `ModeloEmbedding` | `gemini-embedding-001` | Modelo de embeddings |
| `Dimensao` | `768` | Dimensão do vetor (a coluna do banco é `vector(768)`) |
| `TrechosPorCriterio` | `3` | Trechos recuperados por critério na avaliação |
| `TrechosPorConsulta` | `6` | Trechos recuperados na consulta do analista |
| `LimiarSimilaridade` | `0.10` | Similaridade mínima para aceitar um trecho |
| `TamanhoLote` | `32` | Tamanho do lote enviado ao provedor de embeddings |

O provedor/chave de IA continuam em `AgenteIa` (`AgenteIa__ApiKey`).
**Nunca versione a chave.** Use variável de ambiente ou `dotnet user-secrets`:

```bash
# Local (dotnet run)
export AgenteIa__ApiKey="sua-chave"
# ou
dotnet user-secrets set "AgenteIa:ApiKey" "sua-chave" --project src/Hackthon.Api

# Docker Compose (a partir da raiz do repositório)
cp .env.example .env   # e preencha AGENTE_IA_API_KEY
docker compose up -d --build api
```

Sem chave, a ingestão/consulta RAG não gera embeddings e a avaliação cai no motor
heurístico — o fluxo continua funcional, apenas sem o apoio das normas.

---

## 4. Como o RAG entra na avaliação

Para cada projeto, **antes de pontuar**, o agente faz uma consulta por critério de Frascati
(título + setor + conteúdo do dossiê + definição do critério), recupera trechos, deduplica,
descarta os fracos e monta um bloco numerado `[1] (documento — Art. N, p. X): ...`.
O prompt exige que o modelo:

- recomende **uma das quatro classes oficiais** (Elegível · Com ressalvas · Não elegível ·
  Evidência insuficiente) de forma **qualitativa**, não como soma das notas;
- informe o **motivo** da classe, as **normas citadas** por critério, as **evidências do dossiê**,
  as **contradições** e as **lacunas**;
- não invente referências quando não há trechos.

O resultado é registrado no projeto (`ClasseRecomendadaIa`, `MotivoClasseIa`, `NormasCitadasIaJson`,
`EvidenciasDossieIa`, `ContradicoesIa`, `LacunasIa`, `UsouRag`, `TrechosConsultadosIa`) e o
**parecer** passa a listar as normas citadas.

---

## 5. Comportamento seguro (o RAG não quebra o fluxo)

- Base vazia, indisponível ou com falha de embeddings ⇒ a avaliação roda **como antes**
  (inclusive com fallback heurístico). A submissão do projeto **nunca falha** por causa do RAG.
- Sem classe informada pelo modelo ⇒ usa-se a faixa por nota como plano B.
- A decisão final é sempre do analista; a IA apenas apoia e cita fontes.

---

## 6. Verificação rápida

```bash
# 1) Saúde
curl -s http://127.0.0.1:5088/api/saude

# 2) Estado da base (após login ADM)
curl -s http://127.0.0.1:5088/api/normas/estado -H "Authorization: Bearer $TOKEN"

# 3) Base vazia deve responder 409 com mensagem amigável
curl -s -o /dev/null -w "%{http_code}\n" -X POST http://127.0.0.1:5088/api/normas/reindexar \
  -H "Authorization: Bearer $TOKEN"
```

---

## 7. Roteiro de teste passo a passo (demonstração)

1. **Sem documentos** (estado inicial):
   - `docker compose up -d --build`
   - Login ADM; `GET /api/normas/estado` ⇒ `Configurada:false`.
   - `POST /api/normas/reindexar` ⇒ **409** "Base de conhecimento vazia". A API segue saudável.
2. **Enviar um projeto** como Proponente (`marina.costa@hackthon.dev` / `Proponente@2026`):
   - A avaliação da IA conclui normalmente mesmo sem base (degradação segura).
   - No parecer, a seção "Apoio do agente de IA" mostra `UsouRag:false`.
3. **Alimentar a base**:
   - Copiar os documentos oficiais para `base-conhecimento/normas/`.
   - `POST /api/normas/reindexar` ⇒ **200** com contagem de documentos/trechos.
   - `GET /api/normas/estado` ⇒ `Configurada:true` e lista de documentos.
4. **Nova avaliação** (reenviar/analisar um projeto) ⇒ a recomendação traz **classe**, **motivo**,
   **normas citadas [n]** e o parecer lista as normas. `UsouRag:true` e nº de trechos > 0.
5. **Consulta do analista**: `POST /api/agente/consulta` (`rafael.duarte@hackthon.dev` /
   `Analista@2026`) com `{ "projetoId": "...", "pergunta": "O projeto se enquadra como P&D?" }`
   ⇒ resposta citando as fontes `[n]`.
6. **Reindexação idempotente**: rodar `reindexar` de novo ⇒ mesmo total de trechos (sem duplicar).

---

## 8. Limitações conhecidas

- A coluna `Embedding` é `vector(768)`. Alterar `Rag__Dimensao` exige uma nova migration.
- A página do trecho é registrada quando o PDF permite; em MD/TXT fica nula.
- O conteúdo enviado ao modelo é truncado por configuração (`AgenteIa:MaxCaracteresDocumento`),
  e cada trecho citado é recortado para caber no prompt.
- Sem documentos oficiais na pasta, não há como validar enquadramento real (por desenho).
