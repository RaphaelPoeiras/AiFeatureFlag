# API response status codes & error contract

All successful JSON responses use conventional HTTP status codes (`200 OK`, `201 Created`, `204 No Content`).  
Failures thrown by application/domain code are returned as **`application/problem+json`** with a stable **`errorCode`** string for programmatic handling.

## Problem response shape (`ApiProblemResponse`)

Every handled domain/infrastructure failure serialized by `ExceptionHandlingMiddleware` follows:

| Field       | Type   | Description |
|------------|--------|-------------|
| `title`    | string | Short human-readable category (e.g. `Validation failed`). |
| `detail`   | string | Specific message (safe for logs/UI; avoid leaking stack traces). |
| `status`   | number | Same integer as the HTTP status code. |
| `errorCode`| string | Stable machine identifier (see table below). |
| `traceId`  | string | Request correlation id (`HttpContext.TraceIdentifier`). |

Example (`400 Bad Request`):

```json
{
  "title": "Validation failed",
  "detail": "Email format is invalid.",
  "status": 400,
  "errorCode": "VALIDATION_FAILED",
  "traceId": "00-abcdef..."
}
```

---

## Error codes ↔ exceptions ↔ HTTP status

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

## JWT bearer failures (no problem body)

When a request hits an **`[Authorize]`** endpoint **without** a valid JWT (missing, malformed, expired signature), ASP.NET Core JWT middleware responds with **`401 Unauthorized`** **before** your controllers run.  

That response is **not** guaranteed to use the `ApiProblemResponse` shape above (often empty body + `WWW-Authenticate` header).  
Treat **`401`** on protected routes as “not authenticated” and inspect headers / token refresh flow.

---

## Endpoint matrix

### `POST /api/auth/register`

| Status | Body type | Notes |
|--------|-----------|--------|
| **201** | `AuthResponse` | Success. |
| **400** | `ApiProblemResponse` | `VALIDATION_FAILED` or `INVALID_REQUEST_BODY`. |
| **409** | `ApiProblemResponse` | `CONFLICT` — email already registered. |

### `POST /api/auth/login`

| Status | Body type | Notes |
|--------|-----------|--------|
| **200** | `AuthResponse` | Success. |
| **400** | `ApiProblemResponse` | `VALIDATION_FAILED` / `INVALID_REQUEST_BODY`. |
| **401** | `ApiProblemResponse` | `INVALID_CREDENTIALS`. |

### `GET /api/profile/me` *(JWT required)*

| Status | Body type | Notes |
|--------|-----------|--------|
| **200** | `ProfileMeResponse` | Success. |
| **401** | JWT middleware **or** `ApiProblemResponse` | Missing/invalid JWT → middleware. Token ok but claims incomplete → `MISSING_IDENTITY`. |

### `GET /api/feature-flags` *(JWT required)*

| Status | Body type | Notes |
|--------|-----------|--------|
| **200** | `FeatureFlagResponse[]` | Success (may be empty). |
| **401** | JWT middleware **or** `ApiProblemResponse` | Same split as profile. |

### `POST /api/feature-flags` *(JWT required)*

| Status | Body type | Notes |
|--------|-----------|--------|
| **201** | `FeatureFlagResponse` | Created. |
| **400** | `ApiProblemResponse` | Validation / invalid JSON hints. |
| **401** | JWT middleware **or** `ApiProblemResponse` | Auth / identity resolution. |
| **409** | `ApiProblemResponse` | Duplicate flag key for owner. |

### `PUT /api/feature-flags/{id}` *(JWT required)*

| Status | Body type | Notes |
|--------|-----------|--------|
| **200** | `FeatureFlagResponse` | Updated. |
| **400** | `ApiProblemResponse` | Validation. |
| **401** | JWT middleware **or** `ApiProblemResponse` | Auth / identity. |
| **404** | `ApiProblemResponse` | `NOT_FOUND`. |

### `DELETE /api/feature-flags/{id}` *(JWT required)*

| Status | Body type | Notes |
|--------|-----------|--------|
| **204** | *(empty)* | Deleted. |
| **401** | JWT middleware **or** `ApiProblemResponse` | Auth / identity. |
| **404** | `ApiProblemResponse` | `NOT_FOUND`. |

### `GET /api/public/feature-flags/summaries` *(anonymous)*

| Status | Body type | Notes |
|--------|-----------|--------|
| **200** | `PublicFeatureFlagSummary[]` | Success. |
| **500** | `ApiProblemResponse` | `INTERNAL_ERROR` — unexpected failure while reading storage. |

### `GET /health` *(anonymous)*

| Status | Body type | Notes |
|--------|-----------|--------|
| **200** | `{ "status": "healthy" }` | Liveness probe. |

---

## Swagger / OpenAPI

Controller actions declare `[ProducesResponseType(typeof(ApiProblemResponse), …)]` for documented error statuses.  
See **`AiFeatureFlags.Api.Contracts.ApiProblemResponse`** and **`ApiErrorCode`** constants in source for the canonical contract.
