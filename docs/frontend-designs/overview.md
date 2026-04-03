# Frontend Designs Overview

This folder is the detailed frontend source of truth for Firefly Signal.
It is intentionally self-contained so future frontend scaffolding can follow one consistent style instead of re-deciding structure feature by feature.

## Goals
- Keep the first web app simple, fast, and easy to review.
- Establish a frontend style that scales to more features without turning into a framework maze.
- Preserve a path to Android and iOS later by keeping feature contracts, state shape, and UI intent disciplined.
- Give future GitHub issues concrete conventions for frontend delivery.

## Frontend Direction In One Page
- Use React 18 with TypeScript and Vite.
- Keep the app client-side rendered for the first version.
- Use a feature-based folder structure.
- Use React Router for route structure once the app exists.
- Use Zustand only for shared client state that genuinely crosses component or route boundaries.
- Use MUI for accessible component primitives.
- Use Tailwind CSS for layout, spacing, and token-driven styling.
- Keep route entry files thin and move implementation into feature folders.
- Keep API access behind small typed client modules.
- Optimize for web first while keeping patterns portable to future mobile clients.

## Read Order
1. `solution-structure.md`
2. `coding-style.md`
3. `state-routing-and-api.md`
4. `styling-and-design-system.md`
5. `testing-and-quality.md`

## What These Docs Optimize For
- fast MVP delivery
- feature ownership
- consistent React and TypeScript style
- predictable UI states
- explicit API boundaries
- frontend changes that remain understandable for a single maintainer

## What These Docs Explicitly Avoid
- premature component abstraction
- global state for everything
- design-system sprawl before repeated patterns exist
- router, store, or API wrappers that hide normal React behavior
- web choices that make future mobile expansion harder than necessary
