# Frontend Coding Style

This document defines the expected coding style for frontend implementation in Firefly Signal.

## 1. General Rules

- Prefer function components and hooks.
- Keep files focused and easy to scan.
- Prefer explicit props and return types where they improve clarity.
- Avoid clever generic abstractions unless the pattern has clearly repeated.
- Keep behavior close to the feature that owns it.
- Prefer readable code over DRY for one-off UI cases.

## 2. TypeScript Style

- Use `type` for component props and UI-shaped objects unless `interface` adds clear value.
- Keep API response types close to the consuming feature or API client.
- Prefer discriminated unions for UI status when state has meaningful modes.
- Avoid `any`.
- Avoid broad optional property bags when a smaller explicit type is clearer.

Example:

```ts
export type SearchStatus = "idle" | "loading" | "success" | "empty" | "error";

export type JobCardViewModel = {
  id: number;
  title: string;
  company: string;
  location: string;
  summary: string;
  sourceName: string;
  isRemote: boolean;
  postedAtUtc: string;
};
```

## 3. React Style

- Route files should be thin.
- Feature components may coordinate data loading, state, and composition.
- Presentational components should stay mostly stateless.
- Prefer plain React state for local form and UI concerns.
- Use effects sparingly and only for real external synchronization.
- Keep event handlers named after user intent.

Example naming:
- `handleSubmit`
- `handlePostcodeChange`
- `handleRetryClick`

## 4. Component Boundaries

Use this rough split:
- route component: page entry and route-level composition
- feature container: feature-owned orchestration
- reusable UI component: repeated presentational building block

Do not extract a shared component just because two files happen to look similar once.

## 5. File Naming

- React component files: `PascalCase.tsx`
- hooks: `useX.ts`
- stores: `x.store.ts`
- API modules: `x.api.ts`
- mappers: `x.mappers.ts`
- schema or type files: `x.types.ts`
- test files: `*.test.ts` or `*.test.tsx`

## 6. Imports

- Keep imports grouped by external packages, internal modules, then styles when needed.
- Prefer repo aliases once they are configured.
- Avoid deep relative traversal when an alias makes intent clearer.

## 7. Props And Component APIs

- Keep props small and explicit.
- Prefer value-oriented props over dumping feature state into a presentational component.
- Prefer a small number of clear callbacks over exposing internal implementation details.

Bad direction:
- giant `options` bags
- generic `data` props with unknown shape
- components that mutate external state in hidden ways

## 8. State Shape

- Store UI state in the smallest reasonable place.
- Keep server-derived data separate from purely local UI preferences.
- Prefer explicit status fields over inferring loading and error from multiple booleans.

Example:

```ts
type SearchState = {
  status: "idle" | "loading" | "success" | "empty" | "error";
  errorMessage: string | null;
  results: JobCardViewModel[];
};
```

## 9. Accessibility And UX States

Every feature that loads data should explicitly consider:
- loading
- empty
- error
- success

Forms should:
- keep visible labels
- surface validation clearly
- preserve keyboard flow

## 10. Styling Discipline

- Use MUI for accessible primitive controls.
- Use Tailwind for layout, spacing, sizing, and visual composition.
- Keep styling tokens centralized.
- Avoid ad hoc color and spacing values spread across the app.

## 11. Frontend Rule Set

- Thin routes
- Feature-owned implementation
- Local state first
- Zustand only when state is meaningfully shared
- Typed API boundaries
- Explicit UI states
- Reuse only after repetition is real
