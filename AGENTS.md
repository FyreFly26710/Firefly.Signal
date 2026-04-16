# AGENTS.md

## Project
Firefly Signal

Firefly Signal is a personal career intelligence platform focused on UK software-development job discovery and workflow management.
It is a monorepo containing the React web app, .NET backend services, infrastructure assets, and project documentation.

## Repository Intent
- Keep the repository understandable for a single maintainer.
- Prefer small, focused changes over broad reshaping.
- Preserve a web-first MVP with a clean path to later mobile clients.
- Keep architecture and workflow guidance reusable across both Codex and Claude.

## Canonical Guidance Boundaries
- `AGENTS.md` is the shared repository operating contract.
- `.agents/skills/` is the shared home for architecture, implementation-pattern, and workflow skills.
- `.codex/skills/` contains thin Codex wrappers that point at the shared skill bodies.
- `CLAUDE.md` is a thin Claude entrypoint into the same shared guidance.
- `docs/` is for product scope, project planning, and explaining the current system. Docs are not the implementation-pattern source of truth.

## General Rules
- Prefer small, reviewable changes.
- Follow existing repo patterns before introducing new abstractions.
- Do not perform unrelated refactors.
- Do not rename files, folders, symbols, or APIs unless the task requires it.
- Do not add dependencies unless the change clearly needs them.
- Keep contracts explicit across frontend, gateway, and backend boundaries.
- Record important assumptions in issue notes, PR summaries, or planning docs.
- Keep skills self-contained. Do not make skills rely on docs unless a task explicitly needs project planning context.

## Product Context
- Primary user: repository owner.
- MVP focus: UK developer-job discovery, stored job catalog, search, workflow state, profile material, and bounded AI assistance.
- Early non-goals: native mobile apps, broad multi-tenant infrastructure, and premature platform complexity.

## Technology Snapshot
- Frontend: React 18, TypeScript, Vite, Zustand, MUI, Tailwind CSS.
- Backend: .NET 10, EF Core, PostgreSQL, RabbitMQ when justified, selective Redis, JWT auth.
- Infrastructure: Cloudflare Pages for frontend, Docker on a Mac mini for backend, Cloudflare Tunnel for exposure.

## Working Model
- Read the PRD and planning docs before larger or ambiguous changes.
- Ask for confirmation before broadening scope, rewriting issue or PR content substantially, or making non-obvious architectural changes.
- Keep route, API, and service contracts explicit and typed.
- Prefer `red -> implement -> green` when behavior is changing and tests are in scope.
- If the task explicitly excludes tests, or the repo does not have meaningful coverage for the area, do not invent test work just to satisfy process language.
- Validate changes with the relevant commands or manual checks that actually exist.

## Issue-Driven Work
- Treat the GitHub issue as the source of truth when the task comes from GitHub.
- Refine unclear issues into concrete `Goal`, `Scope`, and `Acceptance Criteria` before coding.
- Keep one issue mapped to one focused branch and one reviewable PR whenever possible.
- Branches should use `issue-<number>-<descriptive-title>`.
- PR titles should use `<type>(<scope>): <description> (#<issue-number>)`.
- PR bodies should include `Closes #<issue-number>`.
- Do not use the PR number in place of the source issue number in commit messages, PR titles, PR bodies, or summaries.

## Shared Skills
Use the shared skills in `.agents/skills/` when their area matches the task.

- `documentation-lookup`
  Live library and framework documentation lookup.
- `frontend-design`
  Design-led frontend work where visual direction matters.
- `frontend-patterns`
  Firefly frontend structure, React patterns, API boundaries, and UI implementation rules.
- `backend-patterns`
  Firefly backend structure, service layout, contracts, persistence, and eventing rules.
- `git-issue`
  GitHub issue shaping, refinement, status handling, and branch naming.
- `git-pr`
  Pull request preparation, title/body rules, validation summary, and review handoff.

## Planning Docs
Keep these current when the product direction or delivery sequencing changes materially.

- `docs/product-requirements-document.md`
- `docs/plans.md`
- `docs/frontend-designs.md`
- `docs/backend-designs.md`
- `docs/development/overview.md`
- `docs/development/roadmap.md`
- `docs/development/todo.md`
