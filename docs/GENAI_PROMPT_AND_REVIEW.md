## Prompt I would send to a GenAI coding assistant

You are generating an ASP.NET Core 8 **Web API** (not MVC views) for an internal console called **AI Feature Flags**.

Requirements:

- Persistence: SQLite using **ADO.NET only** (`Microsoft.Data.Sqlite`). No Entity Framework, no Dapper, no Mediator libraries.
- Tables:
  - `Users` with GUID primary key and fields: email (unique), password hash, display name, created timestamp (UTC).
  - `FeatureFlags` with GUID primary key and fields: owner user id (FK), key (unique per owner), description, enabled flag, environment (`Development|Staging|Production`), `AiIntegrationHintsJson` (TEXT storing JSON), created/updated timestamps (UTC).
- Architecture:
  - Domain entities
  - Application services containing validation/business rules (key naming rules, JSON validity for hints, environment validation)
  - Infrastructure repositories executing parameterized SQL
  - API controllers + JWT bearer authentication for owner CRUD
  - Anonymous read-only endpoint returning `{ key, isEnabled, environment }` summaries for all flags
- Endpoints:
  - `POST /api/auth/register`, `POST /api/auth/login`
  - CRUD `/api/feature-flags` authorized
  - `GET /api/public/feature-flags/summaries` anonymous
  - Optional `GET /api/profile/me` authorized (JWT claims only)

Deliverables must compile and include **xUnit** tests with mocks for application services plus SQLite integration tests for repositories using `:memory:?cache=shared`.

Output code should favor clarity over cleverness; avoid leaking SQL injection risks; store passwords using bcrypt.

## Representative sample output

See `src/AiFeatureFlags.Api/Controllers/FeatureFlagsController.cs` and `src/AiFeatureFlags.Infrastructure/Repositories/SqlFeatureFlagRepository.cs`.

## How I validated the AI output

- **Correctness**: exercised automated tests (`dotnet test`) focusing on repository SQL mappings and HTTP flows (`WebApplicationFactory`).
- **Security**: ensured parameterized queries everywhere; passwords hashed with bcrypt; JWT signing key length enforced at token issuance time.
- **Architecture boundaries**: confirmed Application layer knows repository interfaces only; Infrastructure references Application + packages; API wires composition root.
- **Edge cases**: invalid emails/passwords; duplicate emails; duplicate flag keys per owner; invalid AI hints JSON; missing/expired JWT behavior via unauthorized HTTP responses.

## Corrections / improvements I commonly apply to AI drafts

- Replace template MVC leftovers with Web API controllers aligned with assignment wording.
- Replace EF Core snippets with explicit SQL DDL migration bootstrap executed at startup for demos.
- Add explicit foreign keys + `Foreign Keys=True` connection configuration for SQLite.
- Normalize inbound validation ordering for predictable error messages.
