---
name: frontend-tdd-workflow
description: Use this skill when writing frontend features, fixing frontend bugs, or refactoring apps/web. Enforces Firefly Signal's frontend TDD workflow: colocated Vitest for logic, hooks, and views; Playwright smoke flows for critical browser paths.
---

# Frontend TDD Workflow — Firefly Signal

This skill defines the frontend testing workflow for `apps/web`. Use it with `frontend-patterns` and `frontend-design` whenever you are changing frontend behavior.

## When to Activate

- Writing new frontend features in `apps/web`
- Fixing frontend bugs
- Refactoring views, hooks, route behavior, or shared UI
- Adding or expanding frontend test coverage
- Changing browser-visible behavior that may need Playwright validation

## Test Placement Rules

| Layer | File location | Runner |
|---|---|---|
| Pure logic, mappers, helpers | Colocated `*.test.ts` | Vitest |
| Zustand store transitions | Colocated `*.test.ts` | Vitest |
| Hooks (TanStack Query, custom) | Colocated `*.test.tsx` | Vitest |
| View rendering and interactions | Colocated `*.test.tsx` | Vitest |
| Route smoke, auth redirects, critical flows | `tests/e2e/*.spec.ts` | Playwright |

Vitest picks up `src/**/*.test.{ts,tsx}` only. Playwright specs live under `tests/e2e/` and are excluded from the Vitest run.

## TDD Workflow

### Step 1: State the behavior

Write one or two sentences describing what will change and from whose perspective.

> "As a public user, I can search for jobs by keyword and see paginated results."

### Step 2: Choose the lowest owning layer

1. If the behavior is pure logic (mapper, normalizer, URL helper) → `*.test.ts`
2. If the behavior is a Zustand store transition → `*.test.ts`
3. If the behavior is a TanStack Query hook or custom hook → `*.test.tsx` using `renderHookWithProviders`
4. If the behavior is view rendering, form interaction, or error state → `*.test.tsx` using `renderWithProviders`
5. If the behavior requires real routing, auth redirects, or a multi-page flow → Playwright under `tests/e2e/`

### Step 3: Write the failing test

Write the smallest test that expresses the missing behavior. Run it first to confirm it fails.

### Step 4: Run the narrowest command

```bash
npm test -- SearchForm
npm test -- useJobSearch
npm test -- JobDetailView
npm run test:e2e -- --grep "Search landing"
```

The first run should fail for the new behavior.

### Step 5: Implement the minimum passing code

Write only what makes the test green. Follow `frontend-patterns` for structure decisions.

### Step 6: Refactor with tests green

Clean up naming, remove duplication, simplify helpers. Keep tests green throughout.

### Step 7: Verify the frontend slice

```bash
npm run lint
npm test
npm run build
```

If browser behavior changed, also run:

```bash
npm run test:e2e
```

## Test Utilities

### renderWithProviders — view and component tests

Located at `src/test/render.tsx`. Wraps with a fresh `QueryClient` (retry: false), `AppProviders` (theme + session), and `MemoryRouter`.

```typescript
import { renderWithProviders } from "@/test/render";

renderWithProviders(<JobDetailView jobId="42" />);
renderWithProviders(<ProfileView />, { route: "/app/profile" });
renderWithProviders(<WorkspaceView />, { hydrateSessionOnMount: false });
```

### renderHookWithProviders — TanStack Query and custom hook tests

Also in `src/test/render.tsx`. Same providers as `renderWithProviders` but uses `renderHook`.

```typescript
import { renderHookWithProviders } from "@/test/render";

const { result } = renderHookWithProviders(() =>
  useJobSearch({ keyword: "designer", postcode: "EC2A", pageIndex: 0, pageSize: 20 })
);

await waitFor(() => {
  expect(result.current.status).toBe("success");
});
```

### Test QueryClient behavior

The test `QueryClient` in `createTestQueryClient()` has:
- `retry: false` — errors surface immediately, no retry delays
- `staleTime: 0` — no caching between tests
- `gcTime: 0` — query cache is cleared immediately

This is why query-level `retry` must NOT be set on individual hooks — it would override `retry: false` and cause tests to hang. Let the `QueryClient` govern retry behavior.

### Mocking API modules

Mock at the module level with `vi.mock`. Return typed mock data.

```typescript
vi.mock("@/api/jobs/jobs.api", () => ({
  getJobById: vi.fn()
}));

vi.mocked(getJobById).mockResolvedValueOnce({ id: 42, title: "Senior Engineer", ... });
```

### Seeding session state in tests

Use `useSessionStore.setState(...)` directly. Do not invoke `signIn`.

```typescript
useSessionStore.setState({
  user: { userAccount: "admin", displayName: "Admin", role: "admin", email: "" },
  isAuthenticated: true
});
```

Reset between tests with `useSessionStore.getState().reset()` in `beforeEach`.

## Examples from This Repo

### Pure logic test

```typescript
// src/features/search/lib/search-query.test.ts
import { createSearchPath } from "@/features/search/lib/search-query";

describe("search-query", () => {
  it("builds the search path from normalized criteria", () => {
    expect(
      createSearchPath({ keyword: "designer", postcode: "EC2A", pageIndex: 0, pageSize: 20 })
    ).toContain("/search");
  });
});
```

### Zustand store test

```typescript
// src/features/auth/store/session.store.test.ts
import { useSessionStore } from "@/features/auth/store/session.store";

describe("useSessionStore", () => {
  beforeEach(() => {
    window.sessionStorage.clear();
    useSessionStore.getState().reset();
    vi.clearAllMocks();
  });

  it("signs in and updates the store", async () => {
    vi.mocked(login).mockResolvedValueOnce({ accessToken: "tok", user: { ... } });
    const user = await useSessionStore.getState().signIn({ userAccount: "ada", password: "secret" });
    expect(useSessionStore.getState().isAuthenticated).toBe(true);
  });
});
```

### Hook test (TanStack Query)

```typescript
// src/features/search/hooks/useJobSearch.test.tsx
import { waitFor } from "@testing-library/react";
import { renderHookWithProviders } from "@/test/render";

it("surfaces API failures as an error state", async () => {
  vi.mocked(getJobsPage).mockRejectedValueOnce(new Error("Service unavailable."));

  const { result } = renderHookWithProviders(() =>
    useJobSearch({ keyword: "engineer", postcode: "M1", pageIndex: 0, pageSize: 20 })
  );

  await waitFor(() => {
    expect(result.current.status).toBe("error");
  });

  expect(result.current.errorMessage).toBe("Service unavailable.");
});
```

### View test

```typescript
// src/features/jobs/views/JobDetailView.test.tsx
import { renderWithProviders } from "@/test/render";

it("renders job details from the API", async () => {
  vi.mocked(getJobById).mockResolvedValueOnce({ id: 42, title: "Senior Product Designer", ... });

  renderWithProviders(<JobDetailView jobId="42" />);

  expect(screen.getByText("Fetching the latest job details...")).toBeInTheDocument();

  await screen.findByRole("heading", { name: "Senior Product Designer" });
});
```

### Async view test with user interaction

```typescript
// src/features/profile/views/ProfileView.test.tsx
it("loads the current profile and saves edits", async () => {
  const user = userEvent.setup();
  vi.mocked(getCurrentProfile).mockResolvedValueOnce({ preferredTitle: "Senior Developer", ... });
  vi.mocked(upsertCurrentProfile).mockResolvedValueOnce({ preferredTitle: "Principal Engineer", ... });

  renderWithProviders(<ProfileView />, { route: "/app/profile" });

  expect(await screen.findByDisplayValue("Senior Developer")).toBeInTheDocument();

  await user.clear(screen.getByLabelText("Preferred title"));
  await user.type(screen.getByLabelText("Preferred title"), "Principal Engineer");
  await user.click(screen.getByRole("button", { name: "Save profile" }));

  await waitFor(() => {
    expect(screen.getByText("Profile saved.")).toBeInTheDocument();
  });
});
```

### E2E smoke test

```typescript
// tests/e2e/search-landing.spec.ts
import { expect, test } from "@playwright/test";

test.describe("Search landing", () => {
  test("shows the public entry points", async ({ page }) => {
    await page.goto("/");
    await expect(page.getByRole("link", { name: "Discover" })).toBeVisible();
  });
});
```

## What Not To Do

- Do not call `renderHook` from `@testing-library/react` directly for TanStack Query hooks — use `renderHookWithProviders` so the `QueryClient` wrapper is in place.
- Do not rebuild providers manually in test files — `renderWithProviders` already handles `QueryClient`, theme, session, and router.
- Do not set `retry` on individual queries or mutations — let the `QueryClient` default govern (retry: 1 in production, retry: false in tests).
- Do not assert implementation details (internal state, CSS class names, private refs).
- Do not default to Playwright for behavior that a Vitest view test covers just as well.
- Do not add tests for behavior you did not change.

## Success Criteria

Frontend work is done when:

- the first test was written before the implementation
- the test sits at the correct ownership layer (pure logic / hook / view / E2E)
- changed behavior is covered by focused Vitest or Playwright tests
- `npm run lint`, `npm test`, and `npm run build` all pass
- the implementation follows the structure in `frontend-patterns`
