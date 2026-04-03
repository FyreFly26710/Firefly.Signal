# Frontend Design

## Objectives
- Deliver a fast, clear web-first experience for job discovery.
- Keep the first client simple: React 18, TypeScript, Vite, client-side rendering only.
- Build UI patterns that can inform later Android and iOS expansion without pretending to solve mobile-native architecture today.

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
    components/
    features/
      search/
    lib/
    routes/
    store/
    styles/
```

Recommended meaning:
- `app/` for providers, app shell, theme, and router setup
- `components/` for broadly reusable presentational pieces
- `features/` for feature-owned UI, state, and API interactions
- `lib/` for API clients and utilities
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
- Keep API access behind a small client layer in `lib/`.
- Normalize backend responses only where the UI truly benefits.
- Keep request status handling explicit.
- Add typed request and response models close to the consuming feature.

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
- Unit-test utility functions and feature logic where worthwhile.
- Add component tests for key user flows such as search submission and state transitions.
- Add a small number of end-to-end tests once the first real flow exists.

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
