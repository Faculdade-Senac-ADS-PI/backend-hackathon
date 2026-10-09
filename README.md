# Hackthon — Backend (.NET 8 / C#)

API REST e serviços do sistema de apoio à decisão para a análise preliminar de elegibilidade da **Lei do Bem**, no contexto do Hackathon STS 2026 (Banco do Nordeste).

---

## 📌 Visão Geral

O backend foi construído em **.NET 8** utilizando princípios de **Clean Architecture** (Arquitetura Limpa) e **Domain-Driven Design (DDD) tático**. Ele gerencia o controle de acesso por papéis (*Role-Based Access Control - RBAC*), cadastro público de usuários com validação de CPF/CNPJ, rastreabilidade com diário de auditoria imutável, e define as portas de extensão para o agente inteligente (RAG com PostgreSQL + `pgvector`).

### Tecnologias Utilizadas

- **Plataforma:** .NET 8 (C# 12)
- **Framework Web:** ASP.NET Core Web API
- **Banco de Dados & ORM:** PostgreSQL 16 com extensão `pgvector`, Entity Framework Core 8 (`Npgsql.EntityFrameworkCore.PostgreSQL`)
- **Autenticação & Autorização:** JWT Bearer Tokens (com suporte a header `Authorization` e cookie `hackthon_token`)
- **Criptografia de Senhas:** `Microsoft.AspNetCore.Identity.PasswordHasher<T>` (PBKDF2 com HMAC-SHA512)
- **Documentação da API:** Swagger / OpenAPI (Swashbuckle)
- **Containerização:** Docker e Docker Compose (`pgvector/pgvector:pg16`)

---

## 🏗️ Estrutura de Pastas e Projetos

A solução `Hackthon.sln` organiza o código em quatro camadas com fluxo de dependência unidirecional:

```text
Hackthon.Api  ───────►  Hackthon.Application  ───────►  Hackthon.Domain
      │                         ▲
      └──────► Hackthon.Infrastructure ───────┘
```

| Camada / Pasta | Responsabilidade | Dependências |
| :--- | :--- | :--- |
| **`src/Hackthon.Domain`** | Regras de negócio essenciais, entidades de domínio (`Usuario`, `RegistroAcesso`), enums (`Papel`, `AcaoAcesso`). Não possui dependências externas ou acoplamento a frameworks. | Nenhuma |
| **`src/Hackthon.Application`** | Casos de uso da aplicação (serviços de negócio isolados), DTOs, mapeadores, validações cadastrais (`Cadastro`), abstrações/portas (`IUsuarioRepositorio`, `IUnidadeDeTrabalho`, `IAgenteElegibilidade`). | `Hackthon.Domain` |
| **`src/Hackthon.Infrastructure`** | Implementação das portas da aplicação: contexto do EF Core (`HackthonDbContext`), migrações (`Migrations/`), repositórios concretos, emissão de JWT (`EmissorToken`), hashing de senhas e implementação stub do agente (`AgenteElegibilidadeNaoConfigurado`). | `Hackthon.Application`, EF Core, Npgsql |
| **`src/Hackthon.Api`** | Ponto de entrada HTTP: Controllers finos, configuração de autenticação JWT, políticas de autorização (`SomenteAdm`, `AnalistaOuAdm`), CORS, Swagger e formatação de respostas padrão (`ProblemDetails`). | `Hackthon.Application`, `Hackthon.Infrastructure` |
| **`docker/`** | Scripts de inicialização do banco (`postgres/init.sql` com extensão `vector`) e configuração do container PostgreSQL. | Docker |
| **`docs/`** | Documentação técnica aprofundada sobre arquitetura, migrations e integração com o agente RAG. | Markdown |

---

## 🔐 Papéis e Sistema de Cadastro

O sistema conta com fluxo de cadastro e controle de acesso com os seguintes papéis:

1. **Usuário Proponente (ou Preponente):**
   - Cadastro público via `POST /api/auth/registrar` (informando `Nome`, `Email`, `CPF/CNPJ`, `Senha` e papel `Proponente`).
   - Destinado ao envio de material de projetos e acompanhamento da análise.
2. **Usuário Assistente:**
   - Cadastro público via `POST /api/auth/registrar` (informando `Nome`, `Email`, `CPF/CNPJ`, `Senha` e papel `Assistente`).
   - Possui privilégios equivalentes ao Analista: acessa a fila de análise e ferramentas de apoio da Lei do Bem.
3. **Analista:**
   - Criado pela administração via `POST /api/usuarios`.
   - Acessa o contrato do agente e a fila de análise.
4. **ADM (Administrador):**
   - Criado pela administração via `POST /api/usuarios` (já nasce com uma semente de desenvolvimento).
   - Gerencia contas, altera papéis, desativa e reativa acessos.
   - **Regra de integridade:** Deve sempre haver pelo menos um ADM ativo no sistema. Ninguém desativa a própria conta.
5. **Auditoria Transacional:**
   - Toda alteração de estado (criação, troca de papel, desativação, reativação) grava um `RegistroAcesso` na mesma transação atômica do EF Core. Se a auditoria falhar, a operação é revertida por completo.
6. **Unicidade e Validação:**
   - E-mail e CPF/CNPJ possuem **índices únicos** na base de dados (`IX_usuarios_Email` e `IX_usuarios_CpfCnpj`).
   - O documento é validado matematicamente com os algoritmos de dígitos verificadores oficiais (11 dígitos para CPF, 14 dígitos para CNPJ) e armazenado de forma normalizada (apenas números).

---

## 👤 Contas de Desenvolvimento (Semente)

Ao subir em ambiente `Development` com banco vazio, as seguintes credenciais são semeadas automaticamente:

| Papel | Nome | E-mail | CPF de Teste | Senha |
| :--- | :--- | :--- | :--- | :--- |
| **Proponente** | Marina Costa | `marina.costa@hackthon.dev` | `11144477735` | `Proponente@2026` |
| **Analista** | Rafael Duarte | `rafael.duarte@hackthon.dev` | `22255588846` | `Analista@2026` |
| **Assistente** | Carlos Souza | `carlos.souza@hackthon.dev` | `44477700083` | `Assistente@2026` |
| **ADM** | Helena Borges | `helena.borges@hackthon.dev` | `33366699957` | `Adm@2026` |

---

## 🚀 Como Rodar o Backend

### Pré-requisitos
- [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0) (ou superior com rollForward)
- [Docker](https://www.docker.com/) e Docker Compose (para o Postgres com pgvector)

### 1. Iniciar o Banco de Dados

Dentro da pasta `backend`:
```bash
docker compose up -d
```
*(Ou a partir da raiz do repositório: `docker compose up -d`)*

Isso iniciará o container `hackthon-postgres` na porta `54329` com a extensão `vector` habilitada.

### 2. Compilar a Solução

```bash
dotnet build Hackthon.sln
```

### 3. Executar a API

```bash
cd src/Hackthon.Api
dotnet run
```

A API estará disponível em:
- **HTTP:** `http://127.0.0.1:5088`
- **Swagger UI:** `http://127.0.0.1:5088/swagger`

> **Nota:** Em modo `Development`, as migrações do Entity Framework são aplicadas automaticamente ao inicializar a aplicação. Para ignorar a inicialização do banco caso esteja testando apenas compilação ou rotinas offline, configure a variável de ambiente `EF_SKIP_DB_INIT=1`.

---

## 📡 Endpoints da API

### Autenticação & Cadastro (`/api/auth`)
| Método | Endpoint | Autorização | Descrição |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/auth/registrar` | Pública | Cadastro de **Usuário Proponente** ou **Usuário Assistente** (`nome`, `email`, `cpfCnpj`, `senha`, `papel`). Devolve JWT e perfil criado |
| `POST` | `/api/auth/entrar` | Pública | Login com `email` e `senha`. Devolve JWT e dados do usuário |
| `GET` | `/api/auth/eu` | Autenticado | Retorna os dados do perfil autenticado atual |

#### Exemplo de Payload para Cadastro Público (`POST /api/auth/registrar`):
```json
{
  "nome": "João da Silva",
  "email": "joao.silva@exemplo.com",
  "cpfCnpj": "11144477735",
  "senha": "SenhaForte@2026",
  "papel": "Assistente"
}
```
*(Valores aceitos para `papel` no cadastro público: `"Proponente"` ou `"Assistente"`. Omissão assume `"Proponente"`).*

### Gestão de Usuários (`/api/usuarios`)
| Método | Endpoint | Autorização | Descrição |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/usuarios` | `SomenteAdm` | Lista todos os usuários cadastrados |
| `POST` | `/api/usuarios` | `SomenteAdm` | Cria nova conta interna (`nome`, `email`, `cpfCnpj`, `senha`, `papel`) |
| `PATCH`| `/api/usuarios/{id}/papel` | `SomenteAdm` | Altera o papel de uma conta existente |
| `POST` | `/api/usuarios/{id}/desativar` | `SomenteAdm` | Desativa uma conta (impede login) |
| `POST` | `/api/usuarios/{id}/reativar` | `SomenteAdm` | Reativa uma conta desativada |

### Agente de Elegibilidade (`/api/agente`)
| Método | Endpoint | Autorização | Descrição |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/agente/contrato` | `AnalistaOuAdm` | Descreve os dados de entrada, saída e o estado do RAG |
| `POST` | `/api/agente/consulta` | `AnalistaOuAdm` | Consulta em linguagem natural às normas, com citações recuperadas da base |

### Base de normas — RAG (`/api/normas`)
| Método | Endpoint | Autorização | Descrição |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/normas/reindexar` | `SomenteAdm` | Prepara/reindexa a base a partir da pasta `base-conhecimento/normas` (idempotente) |
| `GET` | `/api/normas/estado` | `AnalistaOuAdm` | Estado da base: documentos e trechos indexados |

A preparação da base (`reindexar`) retorna **HTTP 409** quando a pasta não contém documentos —
a base é alimentada apenas com documentos oficiais fornecidos pelo usuário.

### Saúde (`/api/saude`)
| Método | Endpoint | Autorização | Descrição |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/saude` | Pública | Healthcheck básico (`{ "status": "ok" }`) |

---

## 📚 Documentação Técnica Adicional

Consulte a pasta [`docs/`](docs/) para detalhes aprofundados:

- [Arquitetura do Backend](docs/arquitetura-backend.md) — Diagrama de camadas, padrões SOLID, ciclo transacional e segurança.
- [Banco de Dados e Migrations](docs/banco-de-dados-e-migrations.md) — Modelagem relacional, histórico de migrações e comandos do EF Core.
- [Guia da Base de Normas (RAG)](docs/RAG-BASE-NORMAS.md) — Como preparar a pasta `base-conhecimento`, indexar, configurar e verificar o RAG passo a passo.
