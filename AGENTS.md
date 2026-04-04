# AGENTS.md

## Project
Firefly Signal

Firefly Signal is a personal career intelligence platform.
The first release focuses on UK job discovery by postcode and keyword, with a longer-term roadmap that includes job tracking, resume-aware filtering, AI recommendations, cover letters, interview preparation, and analytics.

This repository is a monorepo with product, frontend, backend, and automation work living together.

## Repository Intent
- Keep this repo easy for a single maintainer to understand and evolve.
- Prefer small, focused changes that move one concern forward at a time.
- Start with a web-first product and preserve a path to Android and iOS later.
- Treat automation, AI assistance, and contributor guidance as first-class repository assets.

## Current Structure
- `apps/`
  - `web/` for the React frontend
- `services/`
  - `api/` for backend services and the API gateway solution
- `docs/`
  - planning, product, frontend, and backend design documents
- `infra/`
  - production Dockerfiles and runtime deployment assets
- `.codex/skills/`
  - repository-specific Codex skills for planning, frontend work, backend work, and delivery work
- `.github/`
  - workflows and future GitHub automation

## General Principles
- Prefer small, focused changes.
- Follow existing patterns in the repository before introducing new abstractions.
- Do not perform unrelated refactors.
- Do not rename files, folders, symbols, or APIs unless required by the task.
- Do not add dependencies unless necessary and justified in the PR summary.
- Keep the codebase easy for a single maintainer to understand.
- Favor readability and maintainability over cleverness.
- When making assumptions, document them in the PR summary, task notes, or relevant design doc.
- Update the docs when architecture or delivery direction changes materially.

## Product Context
The current primary user is the repository owner.
The product is initially optimized for personal use, not multi-tenant scale.

Current MVP direction:
- User enters a UK postcode
- User enters a job keyword
- System searches for relevant jobs using public apis
- System displays results clearly
- System supports future persistence and analysis

Future direction:
- Scheduled job collection
- Job storage and deduplication
- Resume upload
- User preference profiling
- AI-based filtering and categorization
- Resume improvement suggestions
- Cover letter generation
- Interview preparation support
- Dashboard and analytics

## Technology Direction
### Frontend
- React 18
- TypeScript
- Vite
- Client-side rendered web app
- Zustand for client state
- MUI for component primitives
- Tailwind CSS for utility styling and design tokens
- Web-first UX with a future path to shared mobile-friendly patterns

### Backend
- .NET 10
- Microservice-oriented backend
- API gateway in front of backend services
- Entity Framework Core
- PostgreSQL
- RabbitMQ for asynchronous workflows
- Redis only where it adds clear value
- Google OAuth via Gmail identity
- JWT for API authentication and session propagation
- Use `.slnx` solution files
- Do not use `.slnf`
- Do not use .NET Aspire

### Infrastructure
- Source control: GitHub
- Frontend hosting: Cloudflare
- Backend runtime: Docker on a Mac mini
- Backend exposure: Cloudflare Tunnel
- Prefer local-first and low-ops infrastructure choices when they fit the personal-use scope
- Frontend deployment target: Cloudflare Pages
- Backend image registry: Docker Hub

## Architectural Guardrails
- Keep boundaries explicit between frontend, gateway, and backend services.
- Default to a modular monolith feel in developer experience even if services are deployed separately.
- Introduce microservices only where the boundary is clear and useful.
- Prefer synchronous HTTP for straightforward request-response flows.
- Use RabbitMQ only for workflows that genuinely benefit from async decoupling.
- Use Redis only for caching, short-lived coordination, or rate-limiting concerns that justify the extra moving part.
- Avoid premature platform complexity.

## Frontend Expectations
- Build responsive layouts that work well on desktop first and remain clean on mobile widths.
- Keep routing, state, and API integration simple in the first iteration.
- Prefer feature-oriented folders once the app exists.
- Reuse MUI primitives intentionally and use Tailwind for composition, spacing, and tokens rather than styling chaos.
- Keep accessibility, loading states, empty states, and error states in scope from the start.
- Keep route entry files thin and move feature behavior into feature-owned folders.
- Prefer local component or feature-hook state before introducing Zustand.
- Keep frontend API access behind small typed client modules.
- Preserve UX and state definitions that future Android and iOS clients can conceptually reuse even if UI code differs.

## Backend Expectations
- Follow Clean Architecture boundaries where they clarify responsibilities.
- Keep service contracts explicit and versionable.
- Centralize cross-cutting concerns such as auth, logging, and correlation IDs.
- Prefer vertical slices inside a service over overly generic shared abstractions.
- Keep persistence concerns inside infrastructure layers.
- Design for local development in Docker from the beginning.
- Use `Directory.Build.props`, `Directory.Build.targets`, and `Directory.Packages.props` to keep backend conventions centralized.
- Prefer a shared migration helper pattern similar to `MigrateDbContextExtensions`.
- Prefer RabbitMQ-backed integration events over in-process orchestration when async boundaries are justified.
- Use Dockerfiles per API and Docker Compose for local infrastructure such as PostgreSQL, pgweb, Redis, RedisInsight, and RabbitMQ.

## AI Coding Workflow
The expected delivery loop for future GitHub issues is:
1. Review the issue and relevant docs before editing code.
2. Create a focused branch for the issue.
3. Implement the smallest change that satisfies the issue.
4. Run the relevant tests and checks.
5. Rebase the issue branch onto the latest target branch before opening the PR.
6. Summarize assumptions, risks, and validation.
7. Open a PR for review and squash merge by the repository owner.

When Codex works in this repo:
- Read `docs/plans.md` and the relevant design docs before larger changes.
- Check whether the task affects product scope, frontend design, or backend design, and update docs if needed.
- Avoid broad speculative scaffolding unless the task explicitly asks for it.
- Keep PRs reviewable by one person.
- Use branch names in the form `issue-<number>-<descriptive-title>`.
- Keep GitHub issue titles short and descriptive, without type prefixes.
- Use PR titles in the form `<type>: <description> (#<issue-number>)`, for example `feat: add postcode search form (#12)`.
- Add `Closes #<issue-number>` to the PR body so GitHub links and closes the issue correctly.
- Prefer conventional PR types such as `feat`, `fix`, `docs`, `refactor`, `test`, `build`, and `chore`.

## Documentation Rules
- `docs/product-requirements-document.md` is the product source of truth.
- `docs/frontend-designs.md` describes frontend architecture, UX patterns, and delivery direction.
- `docs/frontend-designs/` contains the detailed frontend coding style and architecture source of truth for this repo.
- `docs/backend-designs.md` describes service boundaries, integration patterns, and operational design.
- `docs/backend-designs/` contains the detailed backend coding style and infrastructure source of truth for this repo.
- `docs/plans.md` tracks phased delivery and sequencing decisions.
- `infra/` contains production Dockerfiles and runtime deployment assets.
- If implementation diverges from these docs, update the docs in the same change when reasonable.

## Out of Scope for Early Work
- Native mobile apps
- Heavy multi-tenant infrastructure
- Over-engineered event choreography
- Premature AI orchestration layers
- Broad shared libraries before repetition proves they are needed
