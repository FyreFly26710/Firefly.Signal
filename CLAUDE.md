# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Firefly Signal is a personal career intelligence platform — a monorepo containing a React frontend (`apps/web/`), a .NET 10 backend (`services/api/`), infrastructure assets (`infra/`), and documentation (`docs/`).

## Commands

### Frontend (`apps/web/`)

```bash
npm run dev          # Start dev server
npm run build        # TypeScript check + Vite build
npm run lint         # ESLint
npm test             # Run tests once (Vitest)
npm run test:watch   # Tests in watch mode
# Filter tests: npm test -- --grep "pattern"
```

### Backend (`services/api/`)

```bash
# Always use .slnx — do not use .sln or .slnf
dotnet restore services/api/Firefly.Signal.Api.slnx
dotnet build services/api/Firefly.Signal.Api.slnx
dotnet test services/api/Firefly.Signal.Api.slnx

# Single test project
dotnet test services/api/tests/Firefly.Signal.JobSearch.UnitTests/Firefly.Signal.JobSearch.UnitTests.csproj
# Filter by class or method
dotnet test <project.csproj> --filter "ClassName=MyTests" --verbosity detailed
```

### Local infrastructure

```bash
cd services/api && docker-compose up
# PostgreSQL :5432, pgweb :5050, Redis :6379, RedisInsight :5540, RabbitMQ :5672/:15672
```

## Architecture

### Backend service layout

The backend is a microservice-oriented solution behind an API gateway:

```
Gateway API         — entry point, auth forwarding, routing
Identity API        — JWT auth, Google OAuth, user profiles, roles
Job Search API      — job ingestion, search, filtering, workflow state
AI API              — job rating and explanations
Shared libs         — EventBus, EventBusRabbitMQ, IntegrationEventLogEF (outbox), SharedKernel, ServiceDefaults
```

Each API follows Clean Architecture:
- `Application/` — use cases, command/query handlers
- `Domain/` — entities and domain logic
- `Endpoints/` — minimal API route handlers
- `Infrastructure/` — EF Core DbContext, migrations, external integrations
- `Program.cs` — DI and middleware

Cross-cutting conventions live in `Directory.Build.props` (warnings as errors, nullable enabled, implicit usings) and `Directory.Packages.props` (centralized NuGet versions).

### Frontend structure

```
apps/web/src/
├── api/        — typed HTTP client modules (one per backend domain)
├── app/        — app shell, theme, router, providers
├── components/ — shared presentational components
├── features/   — feature folders (auth, jobs, search, workspace, profile)
├── lib/        — HTTP utilities, async helpers
├── routes/     — thin route entry components
├── store/      — Zustand stores (global state only)
└── test/       — test utilities and setup
```

State management preference: local component state → feature hook → Zustand (global only).

### Messaging

RabbitMQ is used only for workflows that genuinely need async decoupling. Prefer synchronous HTTP for request-response flows. Integration events use an outbox pattern via `IntegrationEventLogEF`.

## Key constraints

- Use `.slnx` solution files — not `.sln`, not `.slnf`, not .NET Aspire.
- Keep boundaries explicit between frontend, gateway, and backend services.
- Follow existing patterns before introducing new abstractions; do not refactor unrelated code.
- Do not rename files, symbols, or APIs unless required by the task.
- Do not add dependencies without justification in the PR summary.

## Branch and PR conventions

- Branch: `issue-<number>-<descriptive-title>`
- PR title: `<type>(<scope>): <description> (#<issue-number>)`
- PR types: `feat`, `fix`, `refactor`, `test`, `chore`, `agent`
- Add `Closes #<issue-number>` to PR body
- Squash merge only

## Skills

All skill guidance lives in `AGENTS.md` under `## Skills`. Read the relevant subsection before starting work:

| Skill | When to use |
|---|---|
| `firefly-planning` | Turning ideas or issue outlines into phased plans |
| `firefly-delivery` | Direct coding tasks not driven by a GitHub issue |
| `firefly-github-delivery` | GitHub issue-driven work — refinement through PR |
| `firefly-frontend-architecture` | Planning React app structure, state, routing, styling |
| `firefly-frontend-delivery` | Editing the React web app, tests, or build config |
| `firefly-backend-architecture` | Planning service boundaries, .NET structure, messaging |
| `firefly-backend-delivery` | Editing .NET APIs, Docker files, backend tests |

## Documentation

Before larger changes, read the relevant docs:
- `docs/product-requirements-document.md` — product source of truth
- `docs/frontend-designs/` — frontend coding style and architecture
- `docs/backend-designs/` — backend coding style and infrastructure
- `docs/plans.md` — phased delivery and sequencing
- `AGENTS.md` — full AI workflow guidance, delivery loop, and skills
