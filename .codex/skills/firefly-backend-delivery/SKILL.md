---
name: firefly-backend-delivery
description: Implement backend changes in the Firefly Signal repository. Use when Codex is editing .NET APIs, shared backend libraries, Docker files, Compose files, backend tests, or backend build configuration in this repo.
---

# Firefly Backend Delivery

Start by reading:
- `AGENTS.md`
- `docs/plans.md`
- `docs/backend-designs.md`
- the relevant file in `docs/backend-designs/`

Implementation rules:
- keep `Program.cs` thin
- push DI into extension methods
- use minimal APIs with explicit route groups
- keep DTOs and typed results explicit
- centralize backend package and build conventions with `Directory.Build.*`
- preserve `.slnx` usage and do not introduce `.slnf`
- do not introduce Aspire

When touching persistence:
- keep migrations with the owning DbContext
- preserve the shared migration helper pattern
- keep seeders idempotent

When touching messaging:
- preserve `EventBus`, `EventBusRabbitMQ`, and integration event log layering
- preserve the explicit subscription model and queue-per-consuming-service shape
- add RabbitMQ only where the feature really needs async boundaries

When touching identity:
- keep the identity API lightweight
- use Google OAuth and JWT assumptions from repo docs
- avoid identity-server-style platform abstractions

When touching tests:
- keep MSTest and NSubstitute
- keep `Program.Testing.cs` and `InternalsVisibleTo`
- prefer `WebApplicationFactory` plus container-backed dependencies

Before finishing:
- run the relevant backend checks that exist
- summarize assumptions
- note any backend design docs that changed or should change
