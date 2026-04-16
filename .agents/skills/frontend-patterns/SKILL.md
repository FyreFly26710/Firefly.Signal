---
name: frontend-patterns
description: Firefly Signal frontend architecture and React implementation patterns for routes, features, API boundaries, state, and UI composition.
origin: ECC
---

# Frontend Patterns

Use this skill for frontend architecture and implementation work inside `apps/web/`.

## When To Use
- Adding or restructuring React routes, views, components, hooks, or API clients.
- Deciding where frontend code should live.
- Extending shared UI, feature state, or async flows.
- Reviewing whether a frontend change matches the repo's intended shape.

## Current Firefly Frontend Shape

```text
apps/web/src/
  api/
  app/
  components/
  features/
  lib/
  routes/
  store/
  styles/
  test/
```

## Ownership Rules
- `routes/` contains thin page-entry components only.
- `features/` owns feature behavior, views, hooks, mappers, local types, and feature-specific components.
- `api/` owns typed backend request functions and transport types shared across features.
- `components/` is for broadly reusable presentational pieces.
- `lib/` is for technical utilities such as HTTP, auth helpers, and async primitives.
- `store/` is for truly shared Zustand state, not routine local form state.
- `styles/` owns global CSS and Tailwind entrypoints.

## Firefly Component Pattern
Prefer `Page -> View -> Components`.

- `Page`
  Route entry that wires params, auth gates, and high-level layout only.
- `View`
  Smart orchestration layer for a feature surface.
- `Components`
  Presentational pieces that receive explicit props and stay easy to reuse or test.

## State Rules
- Start with local component state.
- Move to a feature hook when the logic is reused inside the same feature.
- Use Zustand only for cross-route or session-level state.
- Do not move isolated form state into global stores by default.

## Feature Boundary Rules
- A feature should not directly import another feature's components, hooks, mappers, or local types.
- If a pattern is truly shared, promote it deliberately into `components/`, `api/`, or `lib/`.
- Keep mapping code close to the feature that consumes it.

## API And Async Rules
- Keep backend access behind small typed client modules in `src/api/`.
- Keep feature-specific orchestration in the feature, not inside the low-level client.
- Treat loading, empty, success, permission, and error states as first-class UI states.
- Make async transitions explicit instead of hiding them in generic abstractions too early.

## Styling Rules
- Use MUI for accessible primitives.
- Use Tailwind for layout, spacing, responsive behavior, and token-driven composition.
- Preserve a coherent visual language across public and protected surfaces.
- Reach for the `frontend-design` mindset when the task is primarily about visual quality.

## Review Heuristics
- Routes stay thin.
- Feature ownership remains obvious.
- Shared code is shared deliberately, not prematurely.
- API contracts are typed and easy to trace.
- The UI remains responsive, accessible, and explicit about state.
