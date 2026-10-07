# Arquitetura do Backend — Hackthon STS 2026

Este documento descreve as decisões de arquitetura de software, padrões de projeto, fluxo de dados e mecanismos de segurança adotados no backend do sistema Hackthon.

---

## 1. Visão Arquitetural

A aplicação adota **Clean Architecture (Onion/Hexagonal)** com dependência orientada para o núcleo (Domain):

```
┌────────────────────────────────────────────────────────┐
│                      Hackthon.Api                      │
│             (Controllers, Middlewares, DI)             │
└───────────────────────────┬────────────────────────────┘
                            │
┌───────────────────────────▼────────────────────────────┐
│                 Hackthon.Infrastructure                │
│    (EF Core, Npgsql, JWT, Hashing, Repositórios, Agente)│
└───────────────────────────┬────────────────────────────┘
                            │
┌───────────────────────────▼────────────────────────────┐
│                  Hackthon.Application                  │
│       (Casos de Uso, Portas/Interfaces, DTOs, Regras)   │
└───────────────────────────┬────────────────────────────┘
                            │
┌───────────────────────────▼────────────────────────────┐
│                    Hackthon.Domain                     │
│         (Entidades de Domínio, Enums, Integridade)     │
└────────────────────────────────────────────────────────┘
```

### Regras de Dependência:
1. **Domain** não referencia nenhum outro projeto ou framework.
2. **Application** referencia apenas o Domain. Não conhece EF Core, ASP.NET Core, HTTP ou bibliotecas de infraestrutura.
3. **Infrastructure** implementa as interfaces definidas em Application utilizando tecnologias externas (EF Core, Npgsql, JWT).
4. **Api** é o hospedeiro de apresentação HTTP. Compõe o container de injeção de dependência (`Program.cs`) e delega para os casos de uso.

---

## 2. Camadas do Sistema

### 2.1. Domain (`Hackthon.Domain`)
- **Entidades:**
  - `Usuario`: Representa os usuários do sistema. Protege mutações de estado através de métodos ricos (`AlterarPapel`, `Desativar`, `Reativar`).
  - `RegistroAcesso`: Registro imutável de auditoria que rastreia ações de acesso (criação, login, troca de papel, ativação).
- **Enums:**
  - `Papel`: `Proponente` (1), `Analista` (2), `Adm` (3).
  - `AcaoAcesso`: `Registro` (1), `Criacao` (2), `AlteracaoPapel` (3), `Desativacao` (4), `Reativacao` (5), `Login` (6).

### 2.2. Application (`Hackthon.Application`)
- **Padrão Caso de Uso (One Service per Use Case):**
  - Cada operação possui uma interface e implementação única:
    - `IRegistrarProponente` / `RegistrarProponenteServico`
    - `IEntrar` / `EntrarServico`
    - `IObterPerfil` / `ObterPerfilServico`
    - `IListarUsuarios` / `ListarUsuariosServico`
    - `ICriarUsuario` / `CriarUsuarioServico`
    - `IAlterarPapel` / `AlterarPapelServico`
    - `IAlterarAtivacao` / `AlterarAtivacaoServico`
- **Portas de Infraestrutura (DIP - Dependency Inversion Principle):**
  - `IUsuarioRepositorio`: Acesso e persistência a dados de usuários.
  - `IRegistroAcessoRepositorio`: Inserção de trilhas de auditoria.
  - `IUnidadeDeTrabalho`: Controle transacional e gravação em lote.
  - `ISenhaHasher`: Contrato de hash e verificação de senhas.
  - `IEmissorToken`: Contrato de geração do JWT.
  - `IAgenteElegibilidade`: Contrato de consulta do analista com assistência de IA.
  - `IRelogio`: Abstração de tempo (`DateTimeOffset.UtcNow`) para permitir testes reprodutíveis.
- **Padrão de Resposta (`Resultado<T>`):**
  - Evita o uso de exceções para fluxo de negócio regular. Retorna `Resultado<T>.Ok(valor)` ou `Resultado<T>.Falha(status, titulo, detalhe)`.

### 2.3. Infrastructure (`Hackthon.Infrastructure`)
- **Persistência Relacional:** `HackthonDbContext` mapeia entidades com Fluent API e conversão de enums para string no Postgres (`character varying`).
- **Segurança:**
  - `SenhaHasher`: Implementado com `PasswordHasher<T>` da Identity Core (PBKDF2 com derivação segura de chaves).
  - `EmissorToken`: Emissão de JWT assinado com chave HMAC-SHA256 (`SymmetricSecurityKey`).
- **Stub do Agente:** `AgenteElegibilidadeNaoConfigurado` cumpre a interface `IAgenteElegibilidade` devolvendo HTTP 501 ("Não Implementado"), mantendo o sistema desacoplado da integração de IA.

### 2.4. Api (`Hackthon.Api`)
- **Controllers Finos:** Controllers apenas validam o payload HTTP, chamam o caso de uso e formatam a resposta.
- **Respostas de Erro:** Implementação do padrão **RFC 7807 (Problem Details)** em português (`ResultadoHttp.Problema`).
- **Autenticação Dupla:** Tokens JWT são lidos automaticamente do cabeçalho `Authorization: Bearer <token>` ou do cookie `hackthon_token`.
- **Políticas de Autorização:**
  - `SomenteAdm`: Exige role `Adm`.
  - `AnalistaOuAdm`: Exige role `Analista` ou `Adm`.

---

## 3. Integridade e Transações ACID

Operações que alteram dados do sistema (ex.: criação de usuário ou alteração de privilégios) exigem garantia de auditoria:

```csharp
await _unidade.ExecutarEmTransacaoAsync(async token =>
{
    // 1. Validações de regra (ex: garantir pelo menos 1 ADM ativo)
    // 2. Modifica a entidade Usuario
    // 3. Insere o RegistroAcesso na auditoria
}, ct);
```

- Se qualquer etapa falhar, o EF Core executa rollback imediato da transação do PostgreSQL.
- Violação de chave única de e-mail é capturada e traduzida para `ConflitoDeEmailException` (HTTP 409).

---

## 4. Segurança

- **Chave de Desenvolvimento:** Chave padrão de 32 bytes definida para desenvolvimento local (`dev-only-hackthon-sts-2026-chave-com-32-bytes-min`). Em produção, deve ser informada via variável de ambiente `Jwt:Secret`.
- **CORS Configurado:** Permite credenciais (`AllowCredentials()`) para as origens explicitamente configuradas em `appsettings.*.json`.
- **Proteção contra Auto-Desativação:** Nenhum usuário pode desativar a própria conta.
- **Garantia de ADM:** A desativação ou rebaixamento de cargo do último ADM do sistema é bloqueada com HTTP 409.
