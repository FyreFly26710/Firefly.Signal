---
name: firefly-backend-architecture
description: Design or refine backend architecture in the Firefly Signal repository. Use when Codex is planning backend service boundaries, solution structure, messaging, migrations, identity, Docker, testing, or .NET project organization for this repo.
---

# Firefly Backend Architecture

Read these files before making backend architecture recommendations:
- `AGENTS.md`
- `docs/plans.md`
- `docs/backend-designs.md`
- `docs/backend-designs/overview.md`
- `docs/backend-designs/solution-structure.md`
- `docs/backend-designs/messaging-migrations-and-outbox.md`
- `docs/backend-designs/identity-api-direction.md`

Preserve the repo's backend direction:
- `.NET 10`
- `.slnx` only
- no `.slnf`
- no Aspire
- thin gateway
- custom identity API with Google OAuth and JWT
- PostgreSQL with EF Core
- RabbitMQ for async integration when justified
- Redis only when it clearly adds value
- Dockerfiles per API
- Docker Compose for local infrastructure

When proposing structure:
- keep project counts low until a boundary is real
- prefer explicit startup and DI wiring
- keep shared code limited to stable cross-service infrastructure
- preserve the shared migration helper and the repo's explicit event bus pattern

Messaging expectations:
- prefer HTTP for synchronous API-to-API calls
- use RabbitMQ for asynchronous follow-up work
- preserve the repo-owned `EventBus` and `EventBusRabbitMQ` projects
- keep explicit `AddRabbitMqEventBus("service-name").AddSubscription<TEvent, THandler>()` registration

When proposing issue breakdowns:
- keep backend issues vertical and reviewable
- separate gateway, identity, job-search, messaging, and Docker concerns cleanly
- call out assumptions and what should stay out of scope
