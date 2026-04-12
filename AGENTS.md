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
  - repository-specific Codex skills for planning, GitHub issue delivery, frontend work, backend work, and delivery work
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
2. Refine the issue with the developer before implementation starts.
3. Create or continue a focused branch for the issue.
4. Implement the smallest change that satisfies the refined issue.
5. Run the relevant tests and checks.
6. Rebase the issue branch onto the latest target branch before opening the PR.
7. Summarize assumptions, risks, and validation.
8. Open a PR for review and squash merge by the repository owner.

When Codex works in this repo:
- Treat the GitHub issue as the source of truth for the requested change when the task comes from GitHub.
- Expect implementation issues to use the `task` issue template with required `Description` plus optional `Goal`, `Scope`, `Acceptance Criteria`, `Constraints`, and `Context`.
- Stop and surface ambiguity when the issue goal, scope, or constraints are not clear enough to implement safely.
- Update the source GitHub issue with visible status comments during issue-driven work:
  - mark it `in progress` when work starts
  - mark it `blocked` if progress stops on an unresolved dependency or clarification
  - mark it `ready for review` when implementation and validation are complete
- Keep the source GitHub issue labels aligned with the same state:
  - ensure `codex`, `co-op`, and `in-progress` are present when active work starts
  - swap to `blocked` if the issue becomes blocked
  - swap to `ready-for-review` when the work is complete
- Keep `in-progress`, `blocked`, and `ready-for-review` mutually exclusive so the current issue state is obvious from the issue list.
- Treat issue comments as the required status signal in the current manual workflow, with labels kept in sync as a filterable mirror of that state.
- When a developer asks Codex to pick up an issue, do not jump straight into coding.
- Read the relevant docs first, then return with a refinement summary that proposes `Goal`, `Scope`, and `Acceptance Criteria`.
- Use comments for the refinement conversation when that helps collaboration.
- Once the refinement is agreed, update the issue body so the final task definition remains in one canonical place.
- Use `firefly-github-delivery` as the orchestration skill for issue-driven work.
- Use `firefly-planning` when the issue needs refinement, decomposition, or sequencing before coding.
- Use `firefly-frontend-delivery` or `firefly-backend-delivery` for area-specific implementation guidance once the touched area is clear.
- Reserve `firefly-delivery` for direct coding tasks that are not primarily driven by a GitHub issue.
- Read `docs/plans.md` and the relevant design docs before larger changes.
- Check whether the task affects product scope, frontend design, or backend design, and update docs if needed.
- Avoid broad speculative scaffolding unless the task explicitly asks for it.
- Keep PRs reviewable by one person.
- Treat the source GitHub issue number as the canonical work item ID throughout the workflow.
- Do not use the pull request number as a substitute for the issue number in commit messages, PR titles, PR bodies, or status summaries.
- Use branch names in the form `issue-<number>-<descriptive-title>`.
- Keep GitHub issue titles short and descriptive, without type prefixes.
- Use PR titles in the form `<type>(<scope>): <description> (#<issue-number>)`, for example `fix(auth): handle null token validation (#12)`.
- Add `Closes #<issue-number>` to the PR body so GitHub links and closes the issue correctly.
- Prefer PR types from this list: `feat`, `fix`, `refactor`, `test`, `chore`, `agent`.

Issue mode guidance:
- Treat all current issue work as `co-op`.
- `co-op` means you and Codex work the issue together, with the GitHub issue providing the shared source of truth.
- Do not use `agent-only` in the current workflow.
- `agent-only` is a future delivery mode to revisit only after the co-op workflow is stable and proven.

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

## Skills

Skills define how an AI agent should approach a specific type of work in this repo. Both Claude Code and Codex use these. Codex resolves skills by name via `.codex/skills/`; Claude Code reads this section directly.

---

### firefly-planning

> Plan work in the Firefly Signal repository. Use when turning product ideas, architecture decisions, or issue outlines into concrete documentation, phased delivery plans, or implementation sequencing.

Read `AGENTS.md`, `docs/plans.md`, and the relevant design document before proposing substantial work.

Keep planning output practical:
- prefer phased delivery over exhaustive speculation
- tie architecture choices back to the personal-use MVP
- preserve room for later mobile expansion without designing a mobile platform now

When planning backend work:
- keep the gateway plus job-search-service start point in mind
- avoid introducing RabbitMQ or Redis without a concrete flow that needs them
- prefer local Docker development and simple Mac mini deployment assumptions

When planning frontend work:
- keep the first release centered on a single search flow
- preserve React 18, TypeScript, Vite, Zustand, MUI, and Tailwind as the intended stack
- prioritize loading, error, and empty states

When planning implementation:
- propose small GitHub issues that fit into one branch and one reviewable PR
- call out assumptions and unresolved questions explicitly
- update docs if the recommendation materially changes repository direction

---

### firefly-delivery

> Deliver direct implementation work in this repo. Use for coding tasks not primarily driven by a GitHub issue workflow. For issue-driven work use `firefly-github-delivery` instead.

Start by reading:
- `AGENTS.md`
- `docs/plans.md`
- the relevant design document for the touched area

When implementing:
- keep changes focused on the requested task
- do not broaden scope into unrelated platform work
- follow existing repo patterns before introducing new abstractions
- keep the codebase understandable for a single maintainer

For frontend tasks:
- preserve the intended stack: React 18, TypeScript, Vite, Zustand, MUI, Tailwind
- keep API calls behind a small client boundary
- treat loading, empty, and error states as required behavior

For backend tasks:
- preserve the gateway and service boundary
- prefer explicit contracts and vertical slices
- avoid adding distributed complexity unless the issue needs it

Before finishing:
- run the relevant checks that exist
- summarize assumptions
- note any docs that should be updated with the change
- keep summaries readable for a human reviewer scanning quickly

---

### firefly-github-delivery

> Orchestrate GitHub issue-driven delivery. Use when reading a GitHub issue, planning the work, creating the issue branch, coordinating implementation, validating changes, and preparing the pull request.

Read these before acting:
- `AGENTS.md`
- `docs/plans.md`
- `docs/github-delivery/overview.md`
- `docs/github-delivery/naming-conventions.md`
- `docs/github-delivery/templates-and-management.md`
- `docs/github-delivery/manual-codex-flow.md`

The GitHub issue is the source of truth for the requested change. Prefer the connected GitHub app/tools first for issue, PR, and metadata access. Use local `gh` commands as a helper or fallback.

**Orchestration rules:**
- Start by reading the issue carefully and restating its goal, scope, acceptance criteria, constraints, and useful context.
- Read the relevant repo docs for the touched area before proposing implementation.
- Treat ambiguity in the issue as a blocker to clarify, not an invitation to invent scope.
- When picked up, do not begin coding in the same turn. Return a refinement summary that narrows the issue into concrete `Goal`, `Scope`, and `Acceptance Criteria`.
- Use issue comments for the refinement discussion when GitHub updates are part of the workflow.
- Once the developer agrees on the refinement, update the issue body so it remains the source of truth.
- Before substantive implementation, post a status comment (picked up, branch name, `in progress`) and apply labels `codex`, `co-op`, `in-progress`; remove `blocked` or `ready-for-review` if present.
- Use `firefly-planning` when the issue needs refinement or decomposition before coding.
- Use `firefly-frontend-delivery` or `firefly-backend-delivery` for area-specific implementation.
- Keep one issue mapped to one focused branch and one reviewable PR.
- When blocked, post a comment marking the issue `blocked` and update labels accordingly.
- Before handing back for review, post a comment marking `ready for review` and summarize validation, assumptions, and risks. Update labels to `ready-for-review`.

**Issue status transitions:**
- `in progress` — when active work starts
- `blocked` — when progress stops on an unresolved dependency
- `ready for review` — when implementation and validation are complete
- Keep `in-progress`, `blocked`, and `ready-for-review` mutually exclusive.

**Naming rules:**
- Branch: `issue-<number>-<descriptive-title>` (lowercase kebab-case)
- PR title: `<type>(<scope>): <description> (#<issue-number>)`
- PR body: must include `Closes #<issue-number>`
- Never use the PR number in place of the issue number
- PR types: `feat`, `fix`, `refactor`, `test`, `chore`, `agent`

**Before opening the PR:**
- confirm branch name matches issue number and title
- confirm PR title and body formats
- confirm commit messages use the issue number, not the PR number
- summarize validation, assumptions, and risks for human review

---

### firefly-frontend-architecture

> Design or refine frontend architecture. Use when planning React app structure, feature boundaries, state strategy, routing, styling, testing, or future mobile-friendly frontend conventions.

Read these files before making recommendations:
- `AGENTS.md`
- `docs/plans.md`
- `docs/frontend-designs.md`
- `docs/frontend-designs/architecture.md`
- `docs/frontend-designs/overview.md`
- `docs/frontend-designs/solution-structure.md`
- `docs/frontend-designs/state-routing-and-api.md`
- `docs/frontend-designs/styling-and-design-system.md`

Preserve the repo's frontend direction:
- React 18, TypeScript, Vite, client-side rendered
- feature-based structure
- Zustand only for genuinely shared client state
- MUI for accessible primitives
- Tailwind CSS for layout, spacing, and token-driven styling
- web first, with a future path to Android and iOS

When proposing structure:
- keep route files thin
- keep implementation inside feature folders
- keep API access behind small typed client modules
- prefer local state before introducing global stores
- preserve explicit loading, empty, error, and success UI states

When proposing issue breakdowns:
- keep issues vertical and reviewable
- separate app shell, search feature, auth, styling, and quality concerns cleanly
- call out assumptions and out-of-scope abstractions

---

### firefly-frontend-delivery

> Implement frontend changes. Use when editing the React web app, frontend tests, styling, routing, state, or frontend build configuration.

Start by reading:
- `AGENTS.md`
- `docs/plans.md`
- `docs/frontend-designs.md`
- `docs/frontend-designs/architecture.md`
- the relevant file in `docs/frontend-designs/`

Implementation rules:
- keep route files thin
- keep feature behavior inside feature folders
- prefer local state before Zustand
- keep API modules small and typed
- use MUI for accessible primitives
- use Tailwind for layout, spacing, and tokens
- keep loading, empty, error, and success states explicit

When touching styling:
- preserve token-driven styling
- avoid default-library-looking screens
- keep responsive behavior intentional and mobile-safe

When touching tests:
- prefer behavior-focused tests
- keep unit and component tests close to the feature that owns the behavior
- avoid low-signal snapshot sprawl

Before finishing:
- run the relevant frontend checks that exist
- summarize assumptions
- note any frontend design docs that changed or should change

---

### firefly-backend-architecture

> Design or refine backend architecture. Use when planning backend service boundaries, solution structure, messaging, migrations, identity, Docker, testing, or .NET project organization.

Read these files before making recommendations:
- `AGENTS.md`
- `docs/plans.md`
- `docs/backend-designs.md`
- `docs/backend-designs/overview.md`
- `docs/backend-designs/solution-structure.md`
- `docs/backend-designs/messaging-migrations-and-outbox.md`
- `docs/backend-designs/identity-api-direction.md`

Preserve the repo's backend direction:
- `.NET 10`, `.slnx` only — no `.slnf`, no Aspire
- thin gateway
- custom identity API with Google OAuth and JWT
- PostgreSQL with EF Core
- RabbitMQ for async integration when justified
- Redis only when it clearly adds value
- Dockerfiles per API, Docker Compose for local infrastructure

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

---

### firefly-backend-delivery

> Implement backend changes. Use when editing .NET APIs, shared backend libraries, Docker files, Compose files, backend tests, or backend build configuration.

Start by reading:
- `AGENTS.md`
- `docs/plans.md`
- `docs/backend-designs.md`
- the relevant file in `docs/backend-designs/`

Implementation rules:
- keep `Program.cs` thin; push DI into extension methods
- use minimal APIs with explicit route groups
- keep DTOs and typed results explicit
- centralize backend package and build conventions with `Directory.Build.*`
- preserve `.slnx` usage — do not introduce `.slnf` or Aspire

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
