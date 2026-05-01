# Guia de estudo (PT-BR) — como revisar este projeto com eficiência

Este guia está em português para facilitar sua preparação; **o código e o README principal permanecem em inglês**, como combinado para o exercício/portfolio.

## O que você deve conseguir explicar em uma entrevista (30–90s cada)

1. **Por que Clean Architecture aqui?**
   - Separação entre **Domain** (modelo), **Application** (regras), **Infrastructure** (SQL/JWT/bcrypt), **Api** (HTTP/DI/composition root).
   - Benefício: testes mais baratos e menos acoplamento entre “como persistimos” e “o que o produto permite”.

2. **Por que ADO.NET sem EF/Dapper/Mediator?**
   - Demonstra domínio de SQL parametrizado e limites explícitos da camada de dados.
   - Evita ORM “ajudando demais” neste exercício específico.

3. **Autenticação**
   - Registro/login persistem usuário + hash (`BCrypt`).
   - JWT é emitido pela infraestrutura (`JwtTokenIssuer`) e validado pelo ASP.NET (`JwtBearer`).
   - Endpoints públicos vs protegidos ilustram autorização por middleware/controller attributes.

4. **Camadas de validação vs persistência**
   - Validações de negócio ficam na Application (`FeatureFlagBusinessRules`, `AuthBusinessRules`).
   - Repositories só aplicam invariantes estruturais (FK/unique via DB + tratamento de affected rows).

5. **Frontend**
   - SPA React consome REST via proxy do Vite (desenvolvimento), mantém JWT em `localStorage` (**somente demo**).

## Roteiro prático de estudo (2–4 horas)

1. Rode API + UI seguindo `README.md`.
2. Separação física no repo: código produtivo em `src/`, **projetos de teste em `test/`** (solution folder **test** no Visual Studio).
3. Percorra no VS:
   - `Program.cs` → entenda bootstrap do SQLite + seed demo + DI.
   - `FeatureFlagService` → identifique invariantes de negócio antes do SQL.
   - `SqlFeatureFlagRepository` → compare cada método com um endpoint HTTP.
4. Rode testes:
   - `dotnet test`
   - Entenda por que há mocks na Application e SQLite na Infrastructure/API.

## Ideias para evoluir (se pedirem “e depois?”)

- Refresh tokens + cookie HttpOnly (substituir localStorage).
- Auditoria (`CreatedBy`, `UpdatedBy`), paginação e filtros por ambiente.
- Versionamento do payload `AiIntegrationHintsJson` por schema/version field.
- Pipeline CI com `dotnet test` + `npm run build`.
