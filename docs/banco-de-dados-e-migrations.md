# Banco de Dados e Migrations — Hackthon

Este documento detalha o modelo relacional do PostgreSQL, a extensão `pgvector` e o fluxo de trabalho com Entity Framework Core Migrations.

---

## 1. Modelo Relacional

O banco de dados utiliza PostgreSQL 16. O modelo atual compreende duas tabelas relacionais e a extensão vetorial `vector`.

### 1.1. Tabela `usuarios`

Armazena as credenciais e o estado cadastral dos participantes e colaboradores.

| Coluna | Tipo | Restrições | Descrição |
| :--- | :--- | :--- | :--- |
| `Id` | `uuid` | PK | Identificador único gerado na aplicação |
| `Nome` | `character varying(160)` | NOT NULL | Nome completo da pessoa |
| `Email` | `character varying(180)` | NOT NULL, UNIQUE (`IX_usuarios_Email`) | E-mail normalizado (minúsculas) |
| `CpfCnpj` | `character varying(14)` | NOT NULL, UNIQUE (`IX_usuarios_CpfCnpj`) | CPF (11 dígitos) ou CNPJ (14 dígitos), normalizado |
| `SenhaHash` | `character varying(500)` | NOT NULL | Hash seguro PBKDF2 gerado pelo ASP.NET Identity |
| `Papel` | `character varying(20)` | NOT NULL | Enum gravado como texto: `Proponente`, `Assistente`, `Analista`, `Adm` |
| `Ativo` | `boolean` | NOT NULL | Indicador de conta ativa (`true`) ou desativada (`false`) |
| `CriadoEm` | `timestamp with time zone` | NOT NULL | Carimbo UTC de criação |
| `AtualizadoEm`| `timestamp with time zone` | NOT NULL | Carimbo UTC da última modificação |

### 1.2. Tabela `registros_acesso`

Diário de auditoria imutável que registra todas as alterações de conta e logins bem-sucedidos.

| Coluna | Tipo | Restrições | Descrição |
| :--- | :--- | :--- | :--- |
| `Id` | `uuid` | PK | Identificador único do registro |
| `UsuarioId` | `uuid` | FK (`usuarios.Id`), NULLABLE, INDEX | Usuário afetado pela ação |
| `AtorId` | `uuid` | NULLABLE | Usuário que executou a ação (se aplicável) |
| `Acao` | `character varying(40)` | NOT NULL | Tipo de ação: `Registro`, `Criacao`, `AlteracaoPapel`, `Desativacao`, `Reativacao`, `Login` |
| `Detalhe` | `character varying(500)` | NULLABLE | Descrição textual da ação |
| `CriadoEm` | `timestamp with time zone` | NOT NULL | Carimbo UTC do evento de auditoria |

### 1.3. Tabela `projetos`

Armazena as propostas e formulários de ideias de projetos de inovação tecnológica (Lei do Bem) submetidos exclusivamente por proponentes válidos e ativos.

| Coluna | Tipo | Restrições | Descrição |
| :--- | :--- | :--- | :--- |
| `Id` | `uuid` | PK | Identificador único da proposta |
| `Protocolo` | `character varying(30)` | NOT NULL, UNIQUE (`IX_projetos_Protocolo`) | Código legível de rastreamento (ex: `PROP-2026-1042`) |
| `Titulo` | `character varying(200)` | NOT NULL | Título descritivo do projeto de P&D |
| `Area` | `character varying(100)` | NOT NULL | Área tecnológica (Software & IA, Biotecnologia, etc.) |
| `AnoBase` | `character varying(10)` | NOT NULL | Exercício fiscal da apuração (ex: `2026`) |
| `IncertezaTecnica`| `character varying(4000)`| NOT NULL | Descrição da barreira técnica (Manual de Frascati) |
| `Metodologia` | `character varying(4000)`| NULLABLE | Hipóteses, delineamento experimental e testes |
| `DispendioEquipe` | `numeric(18,2)` | NOT NULL | Gastos com equipe técnica interna |
| `DispendioMateriais` | `numeric(18,2)` | NOT NULL | Gastos com materiais e reagentes |
| `DispendioTerceiros` | `numeric(18,2)` | NOT NULL | Gastos com serviços técnicos especializados |
| `DispendioTotal` | `numeric(18,2)` | NOT NULL | Investimento total orçado em P&D |
| `BeneficioEstimado`| `numeric(18,2)` | NOT NULL | Dedução estimada (~34% no Lucro Real) |
| `LucroReal` | `boolean` | NOT NULL | Declaração de operação no Lucro Real |
| `CndAtiva` | `boolean` | NOT NULL | Declaração de Certidão Negativa ativa |
| `Status` | `character varying(30)` | NOT NULL, INDEX (`IX_projetos_Status`) | `Triagem`, `EmAnalise`, `Elegivel`, `Diligencia`, `Inelegivel` |
| `ParecerAnalista`| `character varying(4000)`| NULLABLE | Parecer técnico emitido pelo analista de P&D |
| `ProponenteId` | `uuid` | NOT NULL, FK (`usuarios.Id`), INDEX | Chave estrangeira para o usuário proponente |
| `CriadoEm` | `timestamp with time zone` | NOT NULL | Carimbo UTC da submissão inicial |
| `AtualizadoEm` | `timestamp with time zone` | NOT NULL | Carimbo UTC da última modificação |

> **Nota:** A chave estrangeira `FK_projetos_usuarios_ProponenteId` possui `ON DELETE RESTRICT` para assegurar integridade histórica e impedir remoção física de proponentes com projetos vinculados.

---

## 2. Extensão `pgvector`

O container Docker do PostgreSQL utiliza a imagem oficial `pgvector/pgvector:pg16`.
No boot do container, o script `docker/postgres/init.sql` executa:

```sql
CREATE EXTENSION IF NOT EXISTS vector;
```

Esta extensão fica pré-configurada para receber tabelas futuras de fragmentos de normas, artigos da Lei do Bem e embeddings vetoriais de projetos submetidos.

---

## 3. Gestão de Migrações (EF Core)

As migrações ficam localizadas em `src/Hackthon.Infrastructure/Persistencia/Migrations`.
Para suporte aos comandos de CLI em tempo de design (quando a API não está rodando), a infraestrutura implementa `HackthonDbContextFactory`.

### 3.1. Pré-requisito: Ferramenta `dotnet-ef`

Caso ainda não possua a ferramenta instalada globalmente:
```bash
dotnet tool install --global dotnet-ef
```

### 3.2. Criar uma Nova Migração

Ao adicionar ou modificar entidades de domínio e configurações no `HackthonDbContext`:

```bash
cd backend
dotnet ef migrations add NomeDaSuaMudanca \
  --project src/Hackthon.Infrastructure \
  --startup-project src/Hackthon.Api \
  --output-dir Persistencia/Migrations
```

### 3.3. Aplicar Migrações Manualmente

Para aplicar migrações pendentes diretamente no banco de dados via CLI:

```bash
cd backend
dotnet ef database update \
  --project src/Hackthon.Infrastructure \
  --startup-project src/Hackthon.Api
```

### 3.4. Aplicação Automática no Boot (Development)

Em ambiente de desenvolvimento (`ASPNETCORE_ENVIRONMENT=Development`), a classe `BancoInicial` executa automaticamente durante a inicialização da API:

1. `await db.Database.MigrateAsync(ct);` — Aplica todas as migrações pendentes.
2. `await SemeadorDesenvolvimento.SemearAsync(db, senhas, ct);` — Se a tabela `usuarios` estiver vazia, cria os 3 usuários semente (`Marina Costa`, `Rafael Duarte`, `Helena Borges`).

Para desligar essa rotina (por exemplo, em scripts de CI ou testes sem banco rodando):
```bash
export EF_SKIP_DB_INIT=1
```
