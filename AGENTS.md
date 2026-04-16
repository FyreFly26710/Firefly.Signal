# AGENTS.md

## Project Structure & Module Organization

Firefly Signal is a monorepo with both backend services and frontend apps.

- The backend under `services/` is the more complete part of the repo and should be treated as the current architectural source of truth.
- The frontend under `apps/` exists and is functional, but it is expected to be refactored. Do not treat the current frontend folder shape as the long-term target shape.
- Shared implementation guidance lives in `.agents/skills/`.
- `AGENTS.md` is the shared repository operating contract for both Codex and Claude.

## Services

### Architecture Boundaries

`services/` is for backend and shared service-layer code.

- `services/api/src/Firefly.Signal.Gateway.Api`
- `services/api/src/Firefly.Signal.Identity.Api`
- `services/api/src/Firefly.Signal.JobSearch.Api`
- `services/api/src/Firefly.Signal.Ai.Api`
- `services/api/src/Firefly.Signal.SharedKernel`
- `services/api/src/Firefly.Signal.ServiceDefaults`
- `services/api/src/Firefly.Signal.EventBus`
- `services/api/src/Firefly.Signal.EventBusRabbitMQ`
- `services/api/src/Firefly.Signal.IntegrationEventLogEF`

Follow these backend boundaries:

- Keep `Program.cs` thin.
- Keep service registration in `Extensions/ApplicationServiceExtensions.cs`.
- Keep endpoint modules in `Apis/`.
- Keep transport contracts in `Contracts/Requests` and `Contracts/Responses`.
- Keep write behavior in MediatR command handlers under `Application/Commands`.
- Keep read behavior in explicit query classes under `Application/Queries`.
- Keep domain logic on domain entities where it is truly domain behavior.
- Keep persistence under `Infrastructure/Persistence`.
- Keep provider and storage integrations under `Infrastructure/Services` or other service-owned infrastructure folders.
- Keep mapping explicit. 

Use these shared skills when backend work matches them:

- `backend-patterns`
- `backend-tdd-workflow`
- `documentation-lookup`
- `git-issue`
- `git-pr`

### Build, Test, and Development Commands

Primary backend commands:

```bash
dotnet restore services/api/Firefly.Signal.Api.slnx
dotnet build services/api/Firefly.Signal.Api.slnx
dotnet test services/api/Firefly.Signal.Api.slnx
```

Targeted backend test commands:

```bash
dotnet test services/api/tests/Firefly.Signal.JobSearch.UnitTests/Firefly.Signal.JobSearch.UnitTests.csproj
dotnet test services/api/tests/Firefly.Signal.JobSearch.FunctionalTests/Firefly.Signal.JobSearch.FunctionalTests.csproj
dotnet test services/api/tests/Firefly.Signal.Identity.UnitTests/Firefly.Signal.Identity.UnitTests.csproj
dotnet test services/api/tests/Firefly.Signal.Identity.FunctionalTests/Firefly.Signal.Identity.FunctionalTests.csproj
```

Local infrastructure:

```bash
cd services/api
docker-compose up
```

Backend testing rules:

- Start with a failing test first.
- Use `UnitTests` for pure domain and pure helper logic.
- Use `FunctionalTests` for command handlers, query classes, endpoint behavior, auth, and persistence-backed workflows.
- Do not mock `DbContext`.
- Do not mock MediatR just to test API behavior.
- Do not add tests for thin `Program.cs`.
- Keep service test fixtures small and local to `services/api/tests`.

### Coding Style & Naming Conventions

Backend conventions:

- Prefer small, reviewable changes over broad reshaping.
- Keep contracts explicit across API, application, and infrastructure boundaries.
- Follow existing service and folder names before introducing abstractions.
- Do not rename services, folders, files, or contracts unless the task requires it.
- Prefer one command and one handler per write action.
- Prefer explicit query interfaces for reads.
- Keep custom business exceptions in feature application layers and shared cross-cutting exceptions in shared kernel code.

Issue and branch conventions for backend work:

- Treat the GitHub issue as the source of truth.
- Branches should use `issue-<number>-<descriptive-title>`.
- PR titles should use `<type>(<scope>): <description> (#<issue-number>)`.
- PR bodies should include `Closes #<issue-number>`.

## Apps

### Architecture Boundaries

`apps/` is for user-facing application surfaces.

Target shape placeholder:

- `apps/web/`
- `apps/mobile/`
- `apps/shared/`

Placeholder rules for future frontend refactor:

- Keep app-specific UI, routes, and state inside the owning app.
- Move reusable app-facing UI and client utilities into shared frontend modules only when reuse is real.
- Keep transport contracts explicit between apps and backend services.
- Do not treat the current frontend folder layout as final.

Use these skills when frontend work is being reshaped:

- `frontend-patterns`
- `frontend-design`
- `documentation-lookup`

### Build, Test, and Development Commands

Current web app commands:

```bash
cd apps/web
npm run dev
npm run build
npm run lint
npm test
```

Placeholder expectations for future app structure:

- Each app should have its own local run, build, lint, and test commands.
- Shared frontend tooling should be introduced intentionally rather than inferred from the current app layout.

### Coding Style & Naming Conventions

Placeholder rules for the frontend refactor:

- Prefer explicit module ownership by feature or app surface.
- Keep route, state, and API-boundary code easy to trace.
- Avoid spreading shared abstractions too early while the frontend structure is still being refactored.
- Preserve a clean path from the current web-first product to later app surfaces.
