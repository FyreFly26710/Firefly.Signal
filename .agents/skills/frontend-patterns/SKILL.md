---
name: frontend-patterns
description: Firefly Signal frontend development patterns for apps/web. Covers folder structure, component composition, TanStack Query data fetching, Zustand state, form handling, and coding conventions specific to this repo.
---

# Frontend Patterns — Firefly Signal

This skill defines the frontend implementation patterns for `apps/web`. It replaces generic React advice with Firefly-specific conventions. Follow this whenever you are adding, changing, or reviewing frontend code.

## Folder Structure

```
apps/web/src/
  api/                      ← HTTP transport layer — one folder per backend resource
    auth/                   ← auth.api.ts, auth.types.ts
    jobs/                   ← jobs.api.ts, jobs.types.ts
    job-search/             ← job-search.api.ts, user-job-state.api.ts
    profile/                ← profile.api.ts, profile.types.ts
  app/                      ← App bootstrap: AppRoot, AppProviders, AppRouter, RouteLoadingScreen, theme
  components/               ← Shared pure UI: AppHeader, SearchInput, SectionCard
  features/                 ← Feature modules — primary code boundary
    auth/
      components/           ← Route guards (ProtectedRoute, AdminRoute), form components
      store/                ← session.store.ts — Zustand store for auth session
      views/                ← LoginView
    jobs/
      components/           ← Job cards, panels, editor sections
      hooks/                ← useJobDetail — TanStack Query data hooks
      mappers/              ← Response → view model mappers
      types/                ← Feature-specific TypeScript types
      views/                ← JobDetailView, JobsListView, ManageJobView
    profile/
      views/                ← ProfileView
    search/
      components/           ← SearchForm, SearchResults, SearchResultsToolbar
      hooks/                ← useJobSearch (TanStack Query), useJobState (optimistic)
      lib/                  ← search-query.ts — pure URL/criteria helpers
      mappers/              ← search.mappers.ts — response → view model
      types/                ← search.types.ts
      views/                ← SearchLandingView, SearchResultsView
    workspace/
      components/           ← Workspace panels and cards
      views/                ← WorkspaceView
  lib/
    async/                  ← useAsyncTask (for mutation-style imperatives), async-state
    auth/                   ← session-storage helpers
    http/                   ← fetch client (client.ts), ApiError
    env.ts                  ← VITE_ environment variable access
  routes/                   ← Thin route wrappers — extract URL params, render the view
  test/
    render.tsx              ← renderWithProviders, renderHookWithProviders
    setupTests.ts           ← @testing-library/jest-dom setup
```

### Rules

- Feature code belongs inside its feature folder. Do not reach across features.
- `src/api/` is the HTTP transport layer. API functions return raw DTOs.
- `src/routes/` files are thin — they extract URL params and render one feature view. No logic.
- `src/components/` is for shared UI that no single feature owns.
- `src/lib/` is for framework-agnostic utilities (no business logic).
- Zustand stores belong colocated with the feature that owns them (`features/auth/store/`).

## Data Fetching — TanStack Query

Use TanStack Query for all server-state reads. Do not reach directly from a view into `src/api/` for queries — wrap them in a feature hook first.

### Query hook pattern

```typescript
// src/features/jobs/hooks/useJobDetail.ts
import { useQuery } from "@tanstack/react-query";
import { getJobById } from "@/api/jobs/jobs.api";
import { mapJobDetail } from "@/features/jobs/mappers/job-detail.mappers";

export function useJobDetail(jobId: number | null) {
  return useQuery({
    queryKey: ["jobs", jobId],
    queryFn: () => getJobById(jobId!).then(mapJobDetail),
    enabled: jobId !== null
  });
}
```

### View consuming a query hook

```typescript
// src/features/jobs/views/JobDetailView.tsx
export function JobDetailView({ jobId }: { jobId: string | undefined }) {
  const numericId = jobId && !Number.isNaN(Number(jobId)) ? Number(jobId) : null;
  const { data, isPending, isError, error } = useJobDetail(numericId);

  if (numericId === null) return <JobDetailNotFound />;

  return (
    <>
      {isPending && <LoadingPanel />}
      {isError && <Alert severity="error">{error.message}</Alert>}
      {data && <JobDetailHeroCard job={data} />}
    </>
  );
}
```

### Query key conventions

Stable, serialisable query keys:

```typescript
queryKey: ["jobs", jobId]                              // single entity
queryKey: ["job-search", { keyword, postcode, pageIndex, pageSize }]  // parameterised list
queryKey: ["profile", "current"]                       // current-user scoped
```

### QueryClient setup

- Production client lives in `src/app/AppRoot.tsx` with `staleTime: 30_000` and `retry: 1`.
- Test clients live in `src/test/render.tsx` via `createTestQueryClient()` with `retry: false` and `staleTime: 0`.
- `AppProviders` does NOT own a `QueryClient` — it handles session hydration + MUI theme only.
- This separation ensures test `QueryClient` settings are not overridden by production defaults.

## State Management — Zustand

Use Zustand for client-owned global state (auth session, UI state that spans multiple routes). Do not put server state in Zustand — that belongs in TanStack Query.

```typescript
// src/features/auth/store/session.store.ts
export const useSessionStore = create<SessionStore>((set) => ({
  user: null,
  isAuthenticated: false,
  signIn: async ({ userAccount, password }) => { ... },
  signOut: () => { ... },
  hydrate: async () => { ... }
}));
```

## Mutations — useAsyncTask

For write operations (form submits, imports, deletes) that are not reads, use `useAsyncTask` from `src/lib/async/useAsyncTask.ts`. It handles race conditions, loading state, and error capture.

```typescript
const { status, errorMessage, execute } = useAsyncTask(submitProfile);
```

Do not use `useAsyncTask` for data queries — use TanStack Query instead.

## Route Modules

Route files in `src/routes/` extract URL params and render one view. Nothing else.

```typescript
// src/routes/AppJobDetailPage.tsx
import { useParams } from "react-router-dom";
import { ManageJobView } from "@/features/jobs/views/ManageJobView";

export function AppJobDetailPage() {
  const { jobId } = useParams<{ jobId: string }>();
  return <ManageJobView jobId={jobId} />;
}
```

## Component Composition

Prefer composition. Views are assembled from smaller, single-responsibility components.

```typescript
// Good: composed from focused pieces
export function JobDetailView({ jobId }) {
  const { data } = useJobDetail(numericId);
  return (
    <>
      <JobDetailHeroCard job={data} />
      <JobDetailContentPanel title="About the role">
        {paragraphs.map(p => <p key={p}>{p}</p>)}
      </JobDetailContentPanel>
    </>
  );
}
```

## Lazy Loading

Pages are lazy-loaded in `AppRouter`. Wrap each lazy page with `Suspense` via the `withRouteFallback` helper already in `src/app/AppRouter.tsx`.

```typescript
const JobDetailPage = lazy(() =>
  import("@/routes/JobDetailPage").then((m) => ({ default: m.JobDetailPage }))
);

{ path: "/jobs/:jobId", element: withRouteFallback(<JobDetailPage />) }
```

## HTTP Client

Use the typed helpers in `src/lib/http/client.ts` for all API calls. Do not use raw `fetch` in feature code.

```typescript
import { getJson, postJson, putJson, deleteRequest } from "@/lib/http/client";

export async function getJobById(id: number): Promise<JobDetailResponseDto> {
  return getJson<JobDetailResponseDto>(`/api/jobs/${id}`);
}
```

The client automatically:
- attaches the `Authorization: Bearer` header from session storage
- throws `ApiError` with `status` and `message` on non-OK responses
- handles `Content-Type: application/json` for JSON bodies

## Error Handling

`ApiError` from `src/lib/http/api-error.ts` carries the HTTP status code. Check it in views to distinguish 404 from other failures.

```typescript
const isNotFound = isError && error instanceof ApiError && error.status === 404;
```

## Styling

The app uses Tailwind CSS (v4) for layout and custom design tokens, and MUI v7 for form controls and feedback components (Alert, TextField, Button, etc.).

- Use Tailwind for layout, spacing, typography, and colour tokens.
- Use MUI for interactive form controls and feedback elements.
- Do not mix class-based styling with MUI's `sx` prop for the same concern.

## What Not To Do

- Do not call `src/api/` directly from a view — go through a feature hook.
- Do not add `axios` — the fetch client already handles auth, errors, and all HTTP verbs.
- Do not use Suspense for data fetching with `use()` — TanStack Query handles loading/error states more cleanly and is easier to test.
- Do not put server state (lists, entities) into Zustand.
- Do not put route params, loading state, or business logic inside `src/routes/`.
- Do not create a new provider setup in test files — use `renderWithProviders`.
