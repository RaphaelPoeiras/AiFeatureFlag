# AI Feature Flags Platform (.NET 8 + React)

Small **portfolio-grade** solution for an assignment-style exercise: **ASP.NET Core Web API** (no MVC views), **Clean Architecture**, **raw SQLite via ADO.NET** (no Entity Framework / Dapper / Mediator), **JWT auth**, **xUnit tests**, and a **React (Vite + TypeScript)** UI.

Primary domain: register **feature flags** aimed at future **AI rollout integration**, including optional structured metadata (`AiIntegrationHintsJson`).

## User story

See `docs/USER_STORY.md`.

## Solution layout

- `src/AiFeatureFlags.Domain`: entities + tiny domain constants.
- `src/AiFeatureFlags.Application`: business rules + application services + repository interfaces + exceptions.
- `src/AiFeatureFlags.Infrastructure`: SQLite repositories + bcrypt hashing + JWT issuer + demo seed.
- `src/AiFeatureFlags.Api`: controllers + middleware + Swagger + composition root.
- `test/*`: xUnit coverage across layers (including HTTP integration tests via `WebApplicationFactory`).
- `frontend/feature-flags-web`: responsive SPA consuming the REST API.

## Prerequisites

- **.NET SDK 8.x**
- **Node.js 18+** (for the SPA)
- **Visual Studio 2022** (recommended) or VS Code + C# Dev Kit

## Configure secrets (important)

The API reads JWT settings from configuration:

- For real demos outside local dev, **override** `Jwt:SigningKey` using environment variables or user-secrets.
- Signing keys must be **at least 32 UTF-8 bytes** (the issuer validates this).

Example (PowerShell, session-only):

```powershell
$env:Jwt__SigningKey="please-change-this-to-a-long-random-secret-value!"
```

## Run the API

From repo root:

```powershell
dotnet run --project .\src\AiFeatureFlags.Api\AiFeatureFlags.Api.csproj
```

Visual Studio:

- Open `AiFeatureFlagsPlatform.sln`
- Set `AiFeatureFlags.Api` as startup project
- F5

Defaults:

- HTTPS Swagger UI (Development): `https://localhost:7288/swagger`
- SQLite file: `./src/AiFeatureFlags.Api/data/app.db` (created automatically)
- Demo seed enabled in Development (`SeedDemoData=true` in `appsettings.Development.json`)

### Demo credentials

- Email: `demo@example.com`
- Password: `Demo12345!`

## Run tests

From the repository root (the folder containing `AiFeatureFlagsPlatform.sln`):

```powershell
dotnet test
```

## Run the React UI

```powershell
cd .\frontend\feature-flags-web
npm install
npm run dev
```

The UI proxies `/api` and `/health` to `https://localhost:7288` (see `frontend/feature-flags-web/vite.config.ts`).

Notes:

- Because the proxy targets HTTPS with a dev certificate, if your browser/OS rejects it, trust the ASP.NET dev cert (`dotnet dev-certs https --trust`).

## HTTP status codes & errors

See **`docs/API_RESPONSE_STATUS_CODES.md`** for every status code, the shared **`application/problem+json`** shape (`ApiProblemResponse`), and stable **`errorCode`** values mapped from application exceptions.

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

## Scripts

- `scripts/run-api.ps1`
- `scripts/run-web.ps1`

## Docker (optional)

From repo root:

```powershell
docker build -t ai-feature-flags-api .
docker run --rm -p 8080:8080 ai-feature-flags-api
```

Swagger will be available at `http://localhost:8080/swagger` for quick demos.

## Assignment alignment checklist

- Clean Architecture layering with independent Application rules.
- CRUD REST endpoints + HTTP verbs/status codes.
- Users persisted + login/register flows + authorized vs unauthorized endpoints.
- Two persisted aggregates/tables: `Users`, `FeatureFlags`.
- Data access implemented without EF/Dapper/Mediator (ADO.NET SQL).
- Automated tests across Domain/Application/Infrastructure/API layers.

## Study guide

Portuguese companion notes live in `docs/STUDY_GUIDE.pt-BR.md`.

## GenAI exercise appendix

See `docs/GENAI_PROMPT_AND_REVIEW.md`.
