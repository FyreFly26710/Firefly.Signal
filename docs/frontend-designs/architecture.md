# Frontend Architecture

This document defines the concrete frontend architecture for Firefly Signal.
It turns the existing frontend direction into explicit operating rules so future issues can extend the app without re-deciding structure each time.

## 1. Architecture Goals

- keep the web app easy for one maintainer to change confidently
- support MVP delivery without locking the repo into premature abstractions
- keep feature behavior discoverable and close to the feature that owns it
- preserve reusable contracts, state models, and UX states for future mobile clients

## 2. Core Boundary Model

The frontend should be read through four main boundaries:

- `app/`
  App-wide composition such as providers, router setup, theme setup, and shell wiring.
- `features/`
  The main implementation area. Each feature owns its UI behavior, feature-specific types, API mapping, and local orchestration.
- `components/`
  Shared presentational UI that is reused across features and remains business-light.
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

## 3. Folder And Ownership Rules

Use this structure as the default shape:

```text
apps/web/src/
  app/
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
- `features/` owns business-facing UI and should be the default home for new work.
- `components/` should contain only shared UI that has already proven reusable.
- `lib/` should stay technical and boring, not become a second feature layer.
- `store/` should stay small because most state should remain local or feature-scoped.

Do not move code into `components/`, `lib/`, or `store/` just to make the tree look tidy.

## 4. Route Composition Rules

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

## 5. Feature Slice Rules

Each feature owns the code that explains how that feature works.

Typical feature shape:

```text
features/
  search/
    api/
    components/
    hooks/
    mappers/
    types/
```

Add only the subfolders a feature actually needs.

A feature may own:

- feature containers and presentational pieces
- request and response types specific to that feature
- typed feature API modules
- mappers from backend DTOs to UI models
- custom hooks that coordinate feature logic
- feature-level validation helpers

A feature should not introduce its own global store unless the state is genuinely shared across the app.

## 6. Component Architecture Rules

Use this split:

- route component
  screen entry and route-level composition
- feature container
  feature-owned orchestration, request state, and composition
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

Do not build a broad internal component library before real repetition proves the need.

## 7. Hook Creation Rules

Create a custom hook when it improves clarity by grouping reusable behavior or feature orchestration.

A feature hook is a good fit when it owns:

- async request lifecycle for one feature flow
- derived feature state that would otherwise bloat a component
- feature-level action handlers that are easier to read outside JSX
- synchronization with routing or external APIs

Keep logic in the component instead when:

- the behavior is short and local to one component
- extraction would create indirection without reuse or clarity

Do not use hooks as a place to hide global mutable state or to recreate service layers inside React.

## 8. State And Session Rules

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

## 9. API Architecture Rules

Frontend API access should be explicit, typed, and easy to trace.

Use this split:

- `lib/http/` or similar shared client code for low-level HTTP concerns
- `features/<feature>/api/` for feature-facing request functions
- `features/<feature>/mappers/` for mapping backend DTOs to UI-oriented models when needed

API rules:

- one small API module per feature area
- keep request building and response parsing in the API layer
- keep UI-specific mapping close to the consuming feature
- normalize backend errors into a stable frontend error shape where useful
- avoid leaking raw backend DTOs widely through the UI

Do not introduce a large frontend data framework unless the app clearly proves the need.

## 10. Request And UI State Rules

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

## 11. Naming Rules

Use these file-name conventions:

- React components: `PascalCase.tsx`
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

## 12. Styling And Design System Rules

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

## 13. Testing Architecture Rules

Testing should follow the architecture rather than fight it.

Use:

- unit tests for pure helpers, mappers, validation, and small feature logic
- component tests for key user behavior and explicit UI states
- narrow E2E coverage for the most important end-to-end flows

Testing priorities:

- search input validation
- request-state transitions
- success, empty, and error rendering
- route-to-feature composition where behavior matters
- auth guard behavior once protected routes expand

Avoid snapshot-heavy suites that provide low signal for a single-maintainer workflow.

## 14. Mobile Portability Rule

The web app does not need shared mobile UI, but it should preserve reusable intent for future Android and iOS clients:

- API contracts
- domain naming
- state models
- validation rules
- UI state definitions

Preserve those assets without pretending the same UI components will be reused across platforms.

## 15. Practical Decision Rule

When choosing where new frontend code belongs, prefer this order of questions:

1. Is this app-wide composition
2. Is this feature-owned behavior
3. Is this proven shared UI
4. Is this low-level shared technical support
5. Does this state truly need to be global

If the answer is unclear, keep the code closer to the feature first.
The repo should bias toward explicit feature ownership over abstract neatness.
