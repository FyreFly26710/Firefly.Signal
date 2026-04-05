# Development Docs Overview

This folder turns the repository's product, frontend, backend, and delivery docs into a practical guide for future coding work.
It exists to answer a simple question:

What should we build next, in what order, and how do we keep those changes small and reviewable?

## Why This Folder Exists

The source-of-truth docs already define the product direction and architecture:
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

At the time this folder was created, the repository already includes:
- a React web app with a search experience and supporting tests
- a .NET backend with gateway, identity, job search, and AI APIs
- local Docker Compose support for backend dependencies
- repo conventions for GitHub issue-driven delivery

That means future planning should not treat the repo as an empty scaffold.
The next issues should refine and connect what already exists.

## How To Use These Docs

Read in this order before starting a larger implementation issue:

1. `docs/product-requirements-document.md`
2. `docs/plans.md`
3. `docs/development/roadmap.md`
4. `docs/development/todo.md`
5. the relevant area design doc:
   - `docs/frontend-designs.md`
   - `docs/backend-designs.md`
   - `docs/github-delivery/overview.md`

## Planning Rules For Future Coding

- Keep each issue small enough for one branch and one reviewable PR.
- Prefer vertical slices over broad platform work.
- Use the current repo state as the starting point, not the original empty-repo vision.
- Prioritize work that improves the real search loop before adding optional platform complexity.
- Update these docs when implementation changes the recommended order or introduces a durable new constraint.

## Folder Contents

- `roadmap.md`
  Recommended development sequence from the current baseline.
- `todo.md`
  Candidate issues and follow-up work, grouped by priority and area.

## Current Recommendation

The most important planning principle for this repo is:

Strengthen the first useful job-search loop end to end before expanding into richer persistence, AI workflows, or automation layers.
