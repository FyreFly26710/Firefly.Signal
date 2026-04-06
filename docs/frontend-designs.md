# Frontend Design

## Objectives
- Deliver a fast, clear web-first experience for job discovery.
- Keep the first client simple: React 18, TypeScript, Vite, client-side rendering only.
- Build UI patterns that can inform later Android and iOS expansion without pretending to solve mobile-native architecture today.

## Detailed Reference Docs
Use the files in `docs/frontend-designs/` as the detailed frontend source of truth for implementation style:
- `architecture.md`
- `overview.md`
- `coding-style.md`
- `solution-structure.md`
- `state-routing-and-api.md`
- `styling-and-design-system.md`
- `testing-and-quality.md`

## Core Stack
- React 18
- TypeScript
- Vite
- Zustand
- MUI
- Tailwind CSS

## Frontend Principles
- Optimize for clear task completion over feature density.
- Make search, loading, error, and empty states first-class.
- Keep state localized unless it is shared across routes or sessions.
- Use MUI for accessible primitives and Tailwind for composition, spacing, and token-driven styling.
- Avoid over-abstracting UI before repeated patterns emerge.
- Prefer `Page -> View -> dumb components` for screen structure.
- Do not let one feature reference another feature directly; promote to shared only by deliberate decision.

## Initial App Scope
The first frontend slice should support:
- a landing/search page
- postcode input
- keyword input
- search submission
- loading feedback
- results list
- empty state
- error state

Future capabilities such as saved jobs, dashboards, AI recommendations, and document workflows should stay out of the first implementation unless explicitly planned.

## Information Architecture
Recommended early route shape:
- `/`
  - search experience and results
- future routes
  - `/jobs/:id`
  - `/saved`
  - `/profile`
  - `/insights`

The first iteration can begin with a single route if that reduces noise.

## Suggested Frontend Structure
```text
apps/web/
  src/
    app/
    api/
    components/
    features/
      search/
    lib/
    routes/
    store/
    styles/
```

The concrete architectural operating rules for those folders live in `docs/frontend-designs/architecture.md`.

Recommended meaning:
- `app/` for providers, app shell, theme, and router setup
- `api/` for shared backend request functions and DTO contracts
- `components/` for broadly reusable presentational pieces
- `features/` for feature-owned views, components, hooks, mappers, and local types
- `lib/` for technical utilities such as HTTP primitives and async-state helpers
- `routes/` for route entry components
- `store/` for global Zustand stores only
- `styles/` for Tailwind entrypoints and theme-level CSS

## State Management Guidance
Use Zustand for:
- shared search parameters if they span multiple components
- lightweight UI preferences
- future auth/session state if retained client-side

Avoid putting every form field into global state by default.
Prefer component state for isolated form handling.

## API Integration Guidance
- Keep shared backend request functions and DTOs under `src/api/`.
- Keep feature-specific models, mappers, and hooks under the consuming feature by default.
- Normalize backend responses only where the UI truly benefits.
- Keep request status handling explicit.

## Design System Direction
### MUI
Use MUI for:
- accessible controls
- form inputs
- feedback components
- dialog and menu primitives later

### Tailwind
Use Tailwind for:
- layout composition
- spacing
- responsive adjustments
- utility-driven refinements around MUI components

### Theme Strategy
- Define a small set of design tokens early.
- Use a coherent visual identity rather than default library styling.
- Keep typography, spacing, and color choices intentional and easy to evolve.

## UX Guidance For MVP
- Prioritize speed to first meaningful result.
- Keep the main form visible and easy to retry.
- Present job cards with the minimum useful decision data.
- Show source, location, and freshness when available.
- Handle bad postcode input clearly.
- Make no-results states helpful instead of dead ends.

## Responsive Strategy
- Design for desktop first, because that is the initial primary usage mode.
- Ensure the search experience remains comfortable on mobile widths.
- Prefer stacked layouts and progressive disclosure on smaller screens.
- Keep touch targets and form spacing mobile-safe from the start.

## Accessibility Guidance
- Use semantic landmarks.
- Preserve visible labels for key form controls.
- Keep keyboard navigation working across search and results.
- Ensure loading and error states are screen-reader friendly.

## Testing Guidance
- The current UI test files have been intentionally removed and will be reintroduced later.
- When tests return, prioritize view-level behavior, shared utilities, and important feature logic over low-signal component snapshots.

## Phased Frontend Implementation
### Phase 1
- Scaffold Vite app
- Add MUI, Tailwind, Zustand, routing, and theme setup
- Create app shell and search page skeleton

### Phase 2
- Build the search form and results UI
- Add API client integration
- Add error, loading, and empty states

### Phase 3
- Refine responsiveness and accessibility
- Add saved state or richer client behavior only if the product requires it

## Current Recommendation
Start with one feature slice, one page, one route, and one API client boundary.
That will keep the first frontend issue sequence fast and understandable while leaving room to grow cleanly.

Current repo frontend direction:
- `docs/frontend-designs/architecture.md` is the concrete architecture contract for future frontend work
- `routes/` should stay as thin `Page` files that hand off to feature `View`s
- `View`s are the smart orchestration layer
- low-level components should stay dumb and may compose other dumb components
- `src/api/` owns shared backend request functions and DTOs
- features should not reference another feature directly for components, hooks, models, mappers, or types
- promotion into `src/components/`, `src/api/`, or `src/lib/` should happen only by explicit decision
- feature-based structure inside `apps/web/src`
- route entries stay thin and compose feature-owned UI
- Zustand is reserved for cross-route or session-level state
- MUI provides accessible primitives and Tailwind handles layout, spacing, and token-driven styling
- frontend code should stay portable enough that future Android and iOS work can reuse contracts, state models, and UX decisions even if UI implementation changes
