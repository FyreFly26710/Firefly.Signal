# AGENTS.md

## Project Structure & Module Organization

Firefly Signal is a monorepo with both backend services and frontend apps.

- The backend under `services/` is the more complete part of the repo and should be treated as the current architectural source of truth for backend patterns.
- The frontend under `apps/web` is functional and has a settled architecture. Treat `apps/web/src/` as the authoritative frontend target shape.
- Shared implementation guidance lives in `.agents/skills/`.
- `AGENTS.md` is the shared repository operating contract for both Codex and Claude.

## GitHub Agent Bots

When an agent updates GitHub issues or pull requests from the local repo, it should use the matching GitHub App bot instead of a personal GitHub identity.

- Codex should use `codex-coder`.
- Claude should use `claudecode-coder`.
- A future reviewer agent should use `reviewer` when that app is created.

Use the local helper wrapper:

```bash
./scripts/with-github-app.sh <agent-slug> -- gh <...>
```

Examples:

```bash
./scripts/with-github-app.sh codex-coder -- gh issue comment 116 --body "..."
./scripts/with-github-app.sh claudecode-coder -- gh pr comment 123 --body "..."
```

Local app files live under `sources/github-apps/<agent-slug>/` and stay local because `sources/` is gitignored.

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

### Issue-Driven Workflow

All issue-driven work follows a three-round pattern. Use the `git-issue` and `git-pr` skills for step-by-step checklists.

- **Round 1 (Kickoff):** Fetch issue → label `in-progress` → create branch → create git worktree at `worktrees/<branch>` → enrich issue body → post branch + plan comments.
- **Further rounds (Implementation):** Work inside the worktree. Commit and push before ending every chat.
- **Final round (Ship):** Create PR → squash-merge → switch main repo to `main` → remove worktree → delete local branch.

Git worktrees allow multiple agents to work on the same server and repo concurrently without collision. Each branch gets its own worktree at `worktrees/<branch-name>`.

## Apps

### Architecture Boundaries

`apps/` is for user-facing application surfaces. The current app is `apps/web` — a React + Vite + TypeScript web app. Future surfaces (`apps/mobile/`, `apps/shared/`) do not exist yet; do not create them speculatively.

The `apps/web` folder shape is settled. Treat it as the authoritative frontend target, not a placeholder.

```
apps/web/src/
  api/          ← HTTP transport layer (raw DTOs, one folder per backend resource)
  app/          ← App bootstrap (AppRoot, AppProviders, AppRouter, theme)
  components/   ← Shared pure UI (AppHeader, SearchInput, SectionCard)
  features/     ← Primary code boundary — feature-first, colocated tests
    auth/
      components/   ← Route guards, login form
      store/        ← session.store.ts (Zustand)
      views/        ← LoginView
    jobs/
      components/   ← Job cards, editor sections, panels
      hooks/        ← useJobDetail (TanStack Query)
      mappers/      ← Response → view model
      types/        ← Feature types
      views/        ← JobDetailView, JobsListView, ManageJobView
    profile/
      views/        ← ProfileView
    search/
      components/   ← SearchForm, SearchResults, toolbar
      hooks/        ← useJobSearch (TanStack Query), useJobState
      lib/          ← search-query.ts (pure URL helpers)
      mappers/      ← search.mappers.ts
      types/        ← search.types.ts
      views/        ← SearchLandingView, SearchResultsView
    workspace/
      components/   ← Workspace panels
      views/        ← WorkspaceView
  lib/          ← Framework-agnostic utilities (async, auth, http, env)
  routes/       ← Thin wrappers: extract URL params, render one feature view
  test/         ← renderWithProviders, renderHookWithProviders, setupTests
```

Frontend architecture boundaries:

- `src/routes/` files are thin — param extraction + one view render only. No logic.
- `src/api/` is the HTTP transport layer. Feature code calls `src/api/` functions; views call feature hooks.
- `src/features/<feature>/hooks/` wraps `src/api/` calls in TanStack Query. Views consume hooks, not API modules.
- `src/features/<feature>/store/` holds Zustand stores. Auth session store lives at `src/features/auth/store/session.store.ts`.
- `src/lib/` holds framework-agnostic utilities with no business logic.
- `src/components/` holds shared pure UI that no single feature owns.
- Tests are colocated: `*.test.ts` or `*.test.tsx` next to the file under test.

### Key Packages and Their Roles

| Package | Role |
|---|---|
| `@tanstack/react-query` | Server-state reads: queries, caching, loading/error states |
| `zustand` | Client-owned global state (auth session, UI state that spans routes) |
| `react-router-dom` | Routing and navigation |
| `@mui/material` | Form controls, feedback components (Alert, TextField, Button) |
| `tailwindcss` | Layout, spacing, typography, design tokens |
| `vitest` + `@testing-library/react` | Unit, hook, and view tests |
| `@playwright/test` | E2E browser smoke tests |

Do not add `axios` — the fetch client in `src/lib/http/client.ts` handles all HTTP concerns. Do not use React Suspense for data fetching — use TanStack Query's `isPending`/`isError` states instead.

### QueryClient Architecture

The production `QueryClient` lives in `src/app/AppRoot.tsx` (`staleTime: 30_000`, `retry: 1`). `AppProviders` handles session hydration and MUI theme — it does NOT own a `QueryClient`. Tests get a fresh isolated `QueryClient` from `renderWithProviders` or `renderHookWithProviders` (`retry: false`, `staleTime: 0`).

Never set `retry` at the individual query level — it overrides the `QueryClient` default and breaks test isolation.

### Build, Test, and Development Commands

All commands run from `apps/web`:

```bash
npm run dev          # local dev server
npm run build        # production build (runs tsc + vite build)
npm run lint         # ESLint
npm test             # Vitest (src/**/*.test.{ts,tsx} only)
npm run test:e2e     # Playwright (tests/e2e/*.spec.ts)
```

Targeted test commands:

```bash
npm test -- JobDetailView     # run one test file by name
npm run test:e2e -- --grep "Search landing"
```

Verification before finishing any frontend task:

```bash
npm run lint && npm test && npm run build
```

### Coding Style & Naming Conventions

- Feature code belongs inside its feature folder. Do not reach across feature boundaries.
- Views consume feature hooks. Feature hooks wrap `src/api/` modules. Views do not import from `src/api/` directly.
- Route files in `src/routes/` contain only param extraction and one view render.
- Zustand stores belong colocated with the owning feature, not in a top-level `src/store/`.
- File naming: `*.api.ts`, `*.types.ts`, `*.mappers.ts`, `*.store.ts`, `use*.ts` / `use*.tsx`, `*View.tsx`, `*Page.tsx`.
- Tests colocated: `JobDetailView.test.tsx` next to `JobDetailView.tsx`.
- Use `renderWithProviders` for view tests and `renderHookWithProviders` for hook tests. Do not rebuild providers in individual test files.

Issue and branch conventions for frontend work:

- Treat the GitHub issue as the source of truth.
- Branches should use `issue-<number>-<descriptive-title>`.
- PR titles should use `<type>(<scope>): <description> (#<issue-number>)`.
- PR bodies should include `Closes #<issue-number>`.

### Skills

Use these skills when doing frontend work in `apps/web`:

- `frontend-patterns` — folder structure, TanStack Query hooks, Zustand, HTTP client, component composition
- `frontend-tdd-workflow` — TDD workflow, test placement rules, renderWithProviders, renderHookWithProviders, mock patterns
- `frontend-design` — visual direction, design system, composition, accessibility
- `frontend-e2e-testing` — Playwright smoke flows, route and auth coverage
- `documentation-lookup` — fetch up-to-date docs for TanStack Query, React Router, MUI, Tailwind, Zustand
