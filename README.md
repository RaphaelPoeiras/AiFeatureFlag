# AI Feature Flags Platform (.NET 8 + React)

Primary domain: register **feature flags** aimed at future **AI rollout integration**, including optional structured metadata (`AiIntegrationHintsJson`).

## User story

As an **AI platform engineer**, I want a lightweight registry where I can define **feature flags** that will later drive AI rollout experiments (prompt templates, streaming behavior, safety gates), so that I can:

- toggle capabilities safely per environment (**Development**, **Staging**, **Production**),
- attach structured **AI integration hints** as JSON metadata for upcoming automation,
- collaborate securely using **JWT-authenticated** CRUD APIs while still exposing an anonymous **public catalog summary**.

Acceptance highlights:

- A user can **register**, **login**, and see **JWT-protected** endpoints succeed only when authenticated.
- The same user can **create/read/update/delete** their flags via REST endpoints using correct HTTP verbs.
- Anonymous consumers can read a **non-sensitive summary list** without credentials.

## Prerequisites

- **.NET SDK 8.x**
- **Node.js 18+** (for the SPA)
- **Visual Studio 2022** (recommended) or VS Code + C# Dev Kit

## Configurations

The API reads JWT settings from configuration:

- For real demos outside local dev, **override** `Jwt:SigningKey` using environment variables or user-secrets.

## Run the API

Defaults:

- HTTPS Swagger UI (Development): `https://localhost:7288/swagger`
- SQLite file: `./src/AiFeatureFlags.Api/data/app.db` (created automatically)
- Demo seed enabled in Development (`SeedDemoData=true` in `appsettings.Development.json`)

### Demo credentials

- Email: `demo@example.com`
- Password: `Demo12345!`

## Run tests

From the repository root

```powershell
dotnet test
```

## Run the React UI

```powershell
cd .\frontend\feature-flags-web
npm install
npm run dev
```

Notes:

## HTTP status codes & errors

| `errorCode` | HTTP | Source exception | Typical causes |
|-------------|------|------------------|----------------|
| `VALIDATION_FAILED` | **400** | `ApplicationValidationException` | Invalid email/password rules, invalid flag key, unknown environment, invalid AI hints JSON, description length. |
| `INVALID_REQUEST_BODY` | **400** | `ArgumentNullException` | Missing required parameter / null body handling path. |
| `CONFLICT` | **409** | `ConflictException` | Duplicate email on register; duplicate flag key for same owner. |
| `NOT_FOUND` | **404** | `NotFoundException` | Flag id not found or not owned by current user (masked as not found). |
| `INVALID_CREDENTIALS` | **401** | `UnauthorizedCredentialsException` | Wrong email/password on login. |
| `MISSING_IDENTITY` | **401** | `IdentityResolutionException` | JWT present but required claims (`sub`, email, etc.) missing or invalid after authorization. |
| `INTERNAL_ERROR` | **500** | Any other `Exception` | Unexpected bugs, DB connectivity outside SQLite file scope, etc. |

---
## REST surface (quick reference)

Anonymous:

- `GET /health`
- `GET /api/public/feature-flags/summaries`

Auth:

- `POST /api/auth/register`
- `POST /api/auth/login`

JWT required:

- `GET /api/profile/me`
- `GET /api/feature-flags`
- `POST /api/feature-flags`
- `PUT /api/feature-flags/{id}`
- `DELETE /api/feature-flags/{id}`


## Docker (optional)

From repo root:

```powershell
docker build -t ai-feature-flags-api .
docker run --rm -p 8080:8080 ai-feature-flags-api
```

Swagger will be available at `http://localhost:8080/swagger` for quick demos.

## GenAI exercise appendix

You are generating an ASP.NET Core 8 **Web API** (not MVC views) for an internal console called **AI Feature Flags**.

Requirements:

- Persistence: SQLite using **ADO.NET only** (`Microsoft.Data.Sqlite`). NO Entity Framework, NO Dapper, NO Mediator libraries.
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

## Representative sample output

See `src/AiFeatureFlags.Api/Controllers/FeatureFlagsController.cs` and `src/AiFeatureFlags.Infrastructure/Repositories/SqlFeatureFlagRepository.cs`.

