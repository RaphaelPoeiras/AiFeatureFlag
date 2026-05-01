# Informal user story (demo narrative)

As an **AI platform engineer**, I want a lightweight registry where I can define **feature flags** that will later drive AI rollout experiments (prompt templates, streaming behavior, safety gates), so that I can:

- toggle capabilities safely per environment (**Development**, **Staging**, **Production**),
- attach structured **AI integration hints** as JSON metadata for upcoming automation,
- collaborate securely using **JWT-authenticated** CRUD APIs while still exposing an anonymous **public catalog summary**.

Acceptance highlights:

- A user can **register**, **login**, and see **JWT-protected** endpoints succeed only when authenticated.
- The same user can **create/read/update/delete** their flags via REST endpoints using correct HTTP verbs.
- Anonymous consumers can read a **non-sensitive summary list** without credentials.

This story matches the assignment constraints (Clean Architecture separation, raw SQL data access without EF/Dapper/Mediator, unit coverage across layers, plus a responsive SPA frontend).
