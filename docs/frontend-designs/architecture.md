# Frontend Architecture

This document defines the concrete frontend architecture for Firefly Signal.
It turns the existing frontend direction into explicit operating rules so future issues can extend the app without re-deciding structure each time.

## 1. Architecture Goals

- keep the web app easy for one maintainer to change confidently
- support MVP delivery without locking the repo into premature abstractions
- keep feature behavior discoverable and close to the feature that owns it
- preserve reusable contracts, state models, and UX states for future mobile clients

## 2. Current Implementation Summary

The current frontend implementation now follows this practical structure:

- `routes/`
  Thin `Page` files that act as route entries only.
- `features/<feature>/views/`
  `View` components that own orchestration, routing coordination, async hooks, and screen composition.
- `features/<feature>/components/`
  Dumb feature-specific presentational components and sections.
- `components/`
  Dumb shared presentational primitives that are proven cross-feature.
- `api/`
  Shared backend request modules and DTO contracts.
- `lib/`
  Shared technical helpers such as HTTP primitives and async-state utilities.

This is the preferred shape for future frontend work unless a task explicitly calls for a different structure.

## 3. Core Boundary Model

The frontend should be read through four main boundaries:

- `app/`
  App-wide composition such as providers, router setup, theme setup, and shell wiring.
- `features/`
  The main implementation area. Each feature owns its `View`, feature-specific components, local types, mappers, and hooks.
- `components/`
  Shared presentational UI that is reused across features and remains business-light.
- `api/`
  Shared backend-facing request functions and DTO contracts.
- `lib/`
  Small shared technical helpers such as the HTTP client, environment helpers, date helpers, and error normalization.

Supporting boundaries:

- `routes/`
  Thin route-entry files that compose feature-owned modules.
- `store/`
  Global Zustand stores only.
- `styles/`
  Global tokens, Tailwind entrypoints, and app-level visual definitions.
- `test/`
  Shared test helpers and setup.

## 4. Folder And Ownership Rules

Use this structure as the default shape:

```text
apps/web/src/
  app/
  api/
  components/
  features/
  lib/
  routes/
  store/
  styles/
  test/
```

Ownership rules:

- `app/` owns startup and cross-app composition, not feature behavior.
- `api/` owns shared backend request functions and DTOs.
- `features/` owns business-facing UI and should be the default home for new work.
- `features/<feature>/views/` owns screen-level orchestration for that feature.
- `features/<feature>/components/` owns dumb feature-specific UI.
- `components/` should contain only shared UI that has already proven reusable and was explicitly promoted.
- `lib/` should stay technical and boring, not become a second feature layer.
- `store/` should stay small because most state should remain local or feature-scoped.

Do not move code into `components/`, `lib/`, or `store/` just to make the tree look tidy.

## 5. Page And View Rules

The top-level UI hierarchy should be:

- `Page`
- `View`
- low-level components

Use these meanings:

- `Page`
  Thin route-entry file that passes route params, search params, or route context down to one `View`.
- `View`
  Smart screen-level component that owns orchestration, API or hook usage, and composition of lower-level pieces.
- low-level components
  Dumb presentational components that receive props and callbacks and may compose other dumb components.

Low-level components may include other low-level components freely.
The rule is not about rendering depth. The rule is that business orchestration stays in the `View`.

## 6. Route Composition Rules

Routes represent user-visible screens, not implementation layers.

Route files should:

- read params, query values, or navigation context
- compose feature containers or shared layout pieces
- stay thin and easy to scan

Route files should not:

- own complex request orchestration
- own large form state
- become a dumping ground for shared helpers

When a route starts carrying substantial behavior, move that behavior into the relevant feature folder and keep the route as a composition entry point.

## 7. Feature Slice Rules

Each feature owns the code that explains how that feature works.

Typical feature shape:

```text
features/
  search/
    views/
    components/
    hooks/
    mappers/
    types/
```

Add only the subfolders a feature actually needs.

A feature may own:

- one or more `View` components
- feature-specific presentational pieces
- UI-facing types specific to that feature
- mappers from backend DTOs to UI models
- custom hooks that coordinate feature logic
- feature-level validation helpers

A feature should not introduce its own global store unless the state is genuinely shared across the app.

## 8. Feature Isolation Rule

Features should not reference code inside another feature.

Do not import:

- components from another feature
- hooks from another feature
- mappers from another feature
- types from another feature
- view models from another feature

If another feature needs similar behavior or UI, it should create its own code by default.
Promote code into a shared layer only when explicitly instructed or when a deliberate shared decision is made.

Preferred shared destinations:

- `components/` for shared dumb UI
- `api/` for shared DTOs and request functions
- `lib/` for shared technical helpers

This repo should prefer duplicated clarity over accidental coupling between features.

## 9. Component Architecture Rules

Use this split:

- page
  route entry and route-level composition
- view
  feature-owned orchestration, request state, API or hook usage, and composition
- presentational component
  mostly stateless UI focused on rendering and local interaction
- shared UI component
  repeated presentational building block promoted out of a feature

Promotion rule for shared UI:

Move a component into `src/components/` only when all of these are true:

- the same pattern has repeated in more than one feature or route
- the abstraction reduces noise rather than hiding intent
- the component API stays small and explicit
- the component does not pull feature-specific business rules with it
- the promotion is a deliberate shared decision, not an opportunistic shortcut

Do not build a broad internal component library before real repetition proves the need.

Readability rule for larger components:

- if a component grows large enough to feel hard to scan, roughly more than 70 lines is a useful warning sign
- and it can be clearly split into smaller presentational sections
- split it into subcomponents for readability even if those subcomponents are not reused elsewhere
- keep those subcomponents in the same file by default when they are private to that one component

Prefer one exported component per file, with same-file private subcomponents when that keeps the structure readable without creating file sprawl.

## 10. Hook Creation Rules

Create a custom hook when it improves clarity by grouping reusable behavior or feature orchestration.

A feature hook is a good fit when it owns:

- async request lifecycle for one feature flow
- derived feature state that would otherwise bloat a component
- feature-level action handlers that are easier to read outside JSX
- synchronization with routing or external APIs

Views should usually call hooks.
Low-level components should usually not call feature orchestration hooks.

Keep logic in the component instead when:

- the behavior is short and local to one component
- extraction would create indirection without reuse or clarity

Do not use hooks as a place to hide global mutable state or to recreate service layers inside React.

## 11. State And Session Rules

State should live in the smallest reasonable place.

Preferred order:

1. local component state
2. feature hook state
3. Zustand store when the state is genuinely shared

Use Zustand for:

- auth and session state
- persisted user preferences
- cross-route filters or UI state that truly spans screens
- app-wide state that multiple features must observe

Avoid Zustand for:

- one form's local fields
- one route's temporary UI toggles
- transient state that is only used by one feature container

Session rule:

Auth and session concerns should stay centralized in app-level or store-level code.
Feature code should consume session state through clear selectors or typed access points rather than parsing tokens or duplicating auth logic.

## 12. API Architecture Rules

Frontend API access should be explicit, typed, and easy to trace.

Use this split:

- `lib/http/` for low-level HTTP concerns
- `api/<domain>/` for shared request functions and DTO contracts
- `features/<feature>/mappers/` for mapping backend DTOs to UI-oriented models when needed
- `features/<feature>/types/` for feature-local UI types and view models

API rules:

- one shared API module per backend capability area
- keep request building and response parsing in the shared API layer
- keep UI-specific mapping close to the consuming feature by default
- normalize backend errors into a stable frontend error shape where useful
- avoid leaking raw backend DTOs widely through the UI

If multiple features use the same endpoint:

- DTOs stay shared in `api/`
- request functions stay shared in `api/`
- feature-local mappers, hooks, and UI models stay local unless explicitly promoted

Do not introduce a large frontend data framework unless the app clearly proves the need.

## 13. Request And UI State Rules

Every user-visible async flow should model explicit UI states:

- `idle`
- `loading`
- `success`
- `empty`
- `error`

Do not infer major user-facing states from scattered booleans when a small explicit status model is clearer.

Validation and failure rules:

- keep basic form validation near the feature that owns the form
- define a stable frontend error shape for API failures
- make retry paths obvious when retry is appropriate
- preserve submitted values where that helps the user recover quickly
- prefer shared async-state utilities in `lib/` for repeated async state patterns rather than ad hoc per-feature boilerplate

## 14. Naming Rules

Use these file-name conventions:

- React components: `PascalCase.tsx`
- views: `XView.tsx`
- hooks: `useX.ts`
- stores: `x.store.ts`
- API modules: `x.api.ts`
- mappers: `x.mappers.ts`
- types: `x.types.ts`
- tests: `*.test.ts` or `*.test.tsx`

Naming rules:

- prefer feature-oriented names over generic utility names
- name handlers after user intent
- keep component APIs value-oriented and explicit
- avoid giant options bags and generic `data` props when a specific prop shape is clearer

## 15. Styling And Design System Rules

The styling architecture remains:

- MUI for accessible primitives
- Tailwind for layout, spacing, and utility composition
- a small token layer for color, typography, spacing, radius, and elevation

Styling rules:

- keep tokens centralized
- avoid default-library-looking screens
- keep responsive rules intentional
- preserve accessibility, contrast, and focus states

Shared styled components should follow the same promotion rule as shared UI components: promote only after real repetition.

## 16. Testing Architecture Rules

The current UI tests have been intentionally removed and will be reintroduced later.

When frontend tests return, they should follow the architecture rather than fight it:

- test view orchestration where behavior matters
- test shared utilities and feature mappers where useful
- avoid forcing low-level presentational components to carry most of the testing burden
- avoid snapshot-heavy suites that provide low signal for a single-maintainer workflow

## 17. Mobile Portability Rule

The web app does not need shared mobile UI, but it should preserve reusable intent for future Android and iOS clients:

- API contracts
- domain naming
- state models
- validation rules
- UI state definitions

Preserve those assets without pretending the same UI components will be reused across platforms.

## 18. Practical Decision Rule

When choosing where new frontend code belongs, prefer this order of questions:

1. Is this app-wide composition
2. Is this shared backend contract code
3. Is this feature-owned view or feature-owned UI
4. Is this proven shared UI
5. Is this low-level shared technical support
6. Does this state truly need to be global

If the answer is unclear, keep the code closer to the feature first.
The repo should bias toward explicit feature ownership over abstract neatness.
