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

> **Nota:** A chave estrangeira `FK_registros_acesso_usuarios_UsuarioId` possui `ON DELETE RESTRICT` para impedir a exclusão física de usuários que possuem histórico de auditoria.

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
