# Development Docs Overview

This folder turns the repository's product, frontend, backend, and delivery docs into a practical guide for future coding work.
It exists to answer a simple question:

What should we build next, in what order, and how do we keep those changes small and reviewable?

## Why This Folder Exists

The source-of-truth docs define the product direction and architecture:
- `docs/product-requirements-document.md`
- `docs/frontend-designs.md`
- `docs/backend-designs.md`
- `docs/plans.md`

Those documents explain what Firefly Signal is and how the system should be shaped.
This folder adds the development lens:
- the current repo baseline
- the recommended coding sequence from here
- the active backlog and todo list for future issues

## Current Baseline

The repository already includes:
- a React web app with public search, login, protected routes, and admin job-management UI
- a .NET backend with gateway, identity, job search, and AI APIs
- local Docker Compose support for backend dependencies
- repo conventions for GitHub issue-driven delivery

That means future planning should not treat the repo as an empty scaffold.
The next issues should connect and harden what already exists around the real MVP workflow.

## How To Use These Docs

Read in this order before starting a larger implementation issue:

1. `docs/product-requirements-document.md`
2. `docs/plans.md`
3. `docs/backend-designs/data-model-plan.md`
4. `docs/development/roadmap.md`
5. `docs/development/todo.md`
6. the relevant area design doc:
   - `docs/frontend-designs.md`
   - `docs/backend-designs.md`
   - `docs/github-delivery/overview.md`

## Planning Rules For Future Coding

- Keep each issue small enough for one branch and one reviewable PR.
- Prefer vertical slices over broad platform work.
- Use the current repo state as the starting point, not the original empty-repo vision.
- Prioritize the real personal-use workflow: persisted job catalog, search, workflow state, profile, documents, and bounded AI support.
- Update these docs when implementation changes the recommended order or introduces a durable new constraint.

## Folder Contents

- `roadmap.md`
  Recommended development sequence from the current baseline.
- `todo.md`
  Prioritized working backlog and candidate future issues.

## Current Recommendation

The most important planning principle for this repo is:

Strengthen the real single-user end-to-end product loop before expanding into advanced user-management or mobile-platform work.
