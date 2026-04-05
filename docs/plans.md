# Firefly Signal Plan

## Planning Scope
This document captures the initial execution plan for the repository before application scaffolding begins.
It is intentionally biased toward a practical personal-use MVP, with enough structure to support future AI-assisted development and GitHub issue driven delivery.

## Working Assumptions
- The first shipped experience is a web app for UK job discovery.
- The frontend is client-side only and will talk directly to backend APIs through a Cloudflare-exposed entry point.
- The backend will run on a Mac mini in Docker behind Cloudflare Tunnel.
- The initial architecture should preserve room for service separation without forcing distributed-system complexity on day one.
- The repository owner will review and merge pull requests created with Codex assistance.

## Goals For The First Foundation Phase
- Clarify the product and technical direction in docs.
- Establish repo conventions that make future issue-driven work smoother.
- Prepare for frontend and backend scaffolding without generating unnecessary code too early.
- Create enough CI and automation structure to keep the repo organized as code arrives.

## Recommended Delivery Phases
### Phase 0: Planning And Repo Foundation
- Finalize product, frontend, backend, and delivery documents.
- Add detailed extracted backend reference docs under `docs/backend-designs/`.
- Add deployment assets under `infra/`.
- Establish repo guardrails in `AGENTS.md`.
- Add Codex repository skills for planning and implementation workflow.
- Add root repo hygiene files such as `.gitignore`, `.editorconfig`, and GitHub workflows.
- Add lightweight linting and formatting conventions for future frontend work.

### Phase 1: Frontend Skeleton
- Create `apps/web` with React 18, TypeScript, and Vite.
- Add routing, layout shell, API client boundary, Zustand store setup, MUI theme, and Tailwind integration.
- Implement the first search page with postcode and keyword inputs, loading state, empty state, and results list placeholders.
- Add basic linting, testing, and build scripts for the frontend.
- Use the detailed source of truth in `docs/frontend-designs/` for structure, coding style, state, styling, and testing conventions.
- Prepare the Cloudflare Pages deployment workflow and app configuration.

### Phase 2: Backend Skeleton
- Create `services/api` as a .NET 10 solution.
- Use `.slnx` and do not use `.slnf`.
- Introduce the API gateway, the custom identity API, the job search API, and a lightweight AI API.
- Add `Directory.Build.props`, `Directory.Build.targets`, and `Directory.Packages.props`.
- Add shared local Docker development for PostgreSQL, pgweb, RabbitMQ, Redis, and RedisInsight.
- Establish auth boundary, Snowflake-based long IDs, EF Core migrations and seeding, event bus wiring, configuration loading, health checks, logging, and testing strategy.
- Keep production Dockerfiles and Mac mini deployment assets under `infra/`, separate from local dev Compose.

### Phase 3: MVP Job Search Flow
- Implement public source integration for job discovery.
- Normalize and return search results through the backend.
- Connect frontend search form to the gateway.
- Add observability, rate limiting where needed, and persistence decisions only if they add clear value.

### Phase 4: Post-MVP Platform Enhancements
- Scheduled collection
- Storage and deduplication
- Saved searches and tracking
- Resume-aware scoring
- AI-assisted workflows
- Dashboard and analytics

## Repository Shape To Grow Into
```text
apps/
  web/
docs/
  backend-designs.md
  frontend-designs.md
  plans.md
  product-requirements-document.md
services/
  api/
.codex/
  skills/
.github/
  workflows/
```

## Delivery Principles
- Prefer vertical slices over broad platform-first buildout.
- Keep each GitHub issue small enough to implement, test, and review in one PR.
- Avoid creating infrastructure pieces that are not yet exercised by a real feature.
- Treat docs as active architecture, not passive notes.
- Preserve optionality for later mobile expansion by keeping API contracts and UI states disciplined.

## Proposed Early GitHub Issue Sequence
1. Initialize repository plumbing
2. Scaffold frontend app shell
3. Define frontend design tokens and app layout
4. Scaffold backend solution and gateway
5. Add Docker Compose for local dependencies
6. Define auth strategy and configuration model
7. Implement job search contract end to end
8. Add frontend search experience
9. Integrate first public job source
10. Add CI quality gates for code paths that now exist

## Risks To Manage Early
- Overcommitting to microservices before the first feature lands
- Letting frontend and backend contracts drift without a documented API boundary
- Introducing RabbitMQ or Redis before a concrete use case exists
- Creating AI features before the underlying product workflow is stable
- Depending on unstable or restrictive public job data sources without fallback planning

## Success Criteria For The Planning Phase
- Product direction is written down and reviewable.
- Frontend and backend designs are clear enough to guide scaffolding.
- Repo automation supports disciplined future issue work.
- The next issue can focus on implementation instead of re-deciding architecture.
- Frontend architecture, coding style, and delivery guidance are detailed enough to scaffold `apps/web` without re-arguing structure.

## Notes For Future Codex Work
- Read this file first for sequencing context.
- Then read `docs/development/overview.md` for the practical coding roadmap and backlog from the current repo baseline.
- Confirm whether a task belongs to foundation, frontend, backend, or product.
- Keep issue branches focused on one phase outcome where possible.
- Record deviations from these assumptions in the PR summary and, when durable, in the relevant design doc.
