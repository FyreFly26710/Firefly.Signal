---
name: firefly-frontend-architecture
description: Design or refine frontend architecture in the Firefly Signal repository. Use when Codex is planning React app structure, feature boundaries, state strategy, routing, styling, testing, or future mobile-friendly frontend conventions for this repo.
---

# Firefly Frontend Architecture

Read these files before making frontend architecture recommendations:
- `AGENTS.md`
- `docs/plans.md`
- `docs/frontend-designs.md`
- `docs/frontend-designs/overview.md`
- `docs/frontend-designs/solution-structure.md`
- `docs/frontend-designs/state-routing-and-api.md`
- `docs/frontend-designs/styling-and-design-system.md`

Preserve the repo's frontend direction:
- React 18
- TypeScript
- Vite
- client-side rendered web app
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
