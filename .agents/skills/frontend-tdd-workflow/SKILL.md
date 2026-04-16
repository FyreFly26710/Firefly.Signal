---
name: frontend-tdd-workflow
description: Use this skill when writing frontend features, fixing frontend bugs, or refactoring apps/web. Enforces Firefly Signal's frontend TDD workflow with colocated Vitest coverage and focused Playwright smoke flows where browser behavior matters.
---

# Frontend TDD Workflow

This skill defines the frontend testing workflow for Firefly Signal under `apps/web/`.

Use it together with `frontend-patterns`, `frontend-design`, and `frontend-e2e-testing` when changing frontend behavior.

## When to Activate

- Writing new frontend features in `apps/web`
- Fixing frontend bugs
- Refactoring view, hook, route, or shared UI behavior
- Adding tests or expanding existing frontend coverage
- Changing browser-visible behavior that may need Playwright validation

## Firefly Rules

### 1. Tests Before Code

Always start with a failing test at the lowest layer that owns the behavior.

Use:

- colocated `*.test.ts` for pure logic, mappers, stores, and helpers
- colocated `*.test.tsx` for hooks, components, and views
- Playwright specs in `tests/e2e/` for critical browser flows

### 2. Follow The Existing Repo Split

The current frontend standard is:

- feature behavior lives under `apps/web/src/features/<feature>/`
- shared UI lives under `apps/web/src/components/`
- shared test helpers live under `apps/web/src/test/`
- browser tests live under `apps/web/tests/e2e/`

Do not create separate `integration` or `acceptance` folder layers for normal frontend work unless the task explicitly needs them.

### 3. Coverage Means Behavior, Not Metrics Theater

The repo does not currently standardize on a numeric frontend coverage threshold.
Do not invent one in implementation tasks.

Instead, make coverage meaningful:

- happy path
- relevant edge cases
- user-facing error paths
- routing and auth behavior when the change affects navigation

### 4. Test Real Ownership Boundaries

Place tests where the logic actually lives:

- normalization and pure helpers: colocated `*.test.ts`
- hooks and store transitions: colocated `*.test.ts` or `*.test.tsx`
- view behavior and rendering: colocated `*.test.tsx`
- browser routing and critical workflows: Playwright under `tests/e2e/`

## Firefly Frontend Test Types

### Unit And Logic Tests

Use for:

- search query normalization
- API response mapping
- session store behavior
- small async helpers
- feature-specific pure utilities

Examples already in the repo:

- `apps/web/src/features/search/lib/search-query.test.ts`
- `apps/web/src/features/search/mappers/search.mappers.test.ts`
- `apps/web/src/store/session.store.test.ts`

### View And Component Tests

Use for:

- form interaction
- retry and error states
- auth-aware rendering
- feature view behavior
- component rendering with shared providers

Examples already in the repo:

- `apps/web/src/components/AppHeader.test.tsx`
- `apps/web/src/features/profile/views/ProfileView.test.tsx`
- `apps/web/src/features/jobs/views/JobsListView.test.tsx`

### E2E Tests

Use for:

- route smoke tests
- protected-route redirects
- admin flows
- end-to-end regressions that span multiple page concerns

Current example:

- `apps/web/tests/e2e/search-landing.spec.ts`

## TDD Workflow

### Step 1: State The Frontend Behavior

Describe the behavior in one or two sentences before coding.

Examples:

- "As a public user, I can submit search criteria from the landing page."
- "As an authenticated user, I can edit my profile and see a saved confirmation."

### Step 2: Choose The Lowest Owning Layer

Pick the first failing test target with this rule:

1. If the behavior is pure logic, start with a colocated `*.test.ts`.
2. If the behavior is a hook, store, component, or view concern, start with a colocated `*.test.tsx` or hook test.
3. If the behavior depends on real browser routing or full-page workflow, add a Playwright spec.

### Step 3: Write The Failing Test

Write the smallest failing test that proves the missing behavior.

Examples already in the repo:

- `apps/web/src/features/search/components/SearchForm.test.tsx`
- `apps/web/src/features/search/hooks/useJobSearch.test.tsx`
- `apps/web/src/features/workspace/views/WorkspaceView.test.tsx`
- `apps/web/tests/e2e/search-landing.spec.ts`

### Step 4: Run The Smallest Relevant Test Command

Prefer targeted commands while iterating:

```bash
npm test -- SearchForm
npm test -- useJobSearch
npm run test:e2e -- --grep "Search landing"
```

The first run should fail for the new behavior.

### Step 5: Implement The Smallest Passing Change

Add only the code needed to make the new test pass.

Follow `frontend-patterns`:

- keep route modules thin
- keep feature behavior with the owning feature
- prefer composition over large monolithic render blocks
- use shared test helpers instead of duplicating provider setup

### Step 6: Re-Run Focused Tests, Then Refactor

Once the targeted test is green:

- remove duplication
- improve naming
- simplify helpers
- keep the tests green throughout

### Step 7: Verify The Frontend Slice

Before finishing frontend work, run the relevant app commands:

```bash
npm run lint
npm test
npm run build
```

If browser behavior changed, also run:

```bash
npm run test:e2e
```

If the task only touched one focused slice, explain which narrower command you ran instead of the whole suite.

## Patterns To Follow

### Pure Helper Test Pattern

```typescript
import { describe, expect, it } from "vitest";
import { createSearchPath } from "@/features/search/lib/search-query";

describe("search-query", () => {
  it("builds the search path from normalized criteria", () => {
    expect(
      createSearchPath({ keyword: "designer", postcode: "EC2A", pageIndex: 0, pageSize: 20 })
    ).toContain("/search");
  });
});
```

### View Test Pattern

```typescript
import { screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { WorkspaceView } from "@/features/workspace/views/WorkspaceView";
import { renderWithProviders } from "@/test/render";

describe("WorkspaceView", () => {
  it("keeps the workspace focused on supported search actions", () => {
    renderWithProviders(<WorkspaceView />);

    expect(screen.getByRole("heading", { name: "Your workspace" })).toBeInTheDocument();
  });
});
```

### Async View Pattern

```typescript
import { screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import { ProfileView } from "@/features/profile/views/ProfileView";
import { getCurrentProfile } from "@/api/profile/profile.api";
import { renderWithProviders } from "@/test/render";

vi.mock("@/api/profile/profile.api", () => ({
  getCurrentProfile: vi.fn()
}));

describe("ProfileView", () => {
  it("loads profile data", async () => {
    vi.mocked(getCurrentProfile).mockResolvedValueOnce(/* ... */);

    renderWithProviders(<ProfileView />, { route: "/app/profile" });

    expect(await screen.findByLabelText("Preferred title")).toBeInTheDocument();
  });
});
```

### Browser Smoke Pattern

```typescript
import { expect, test } from "@playwright/test";

test("shows the public entry points", async ({ page }) => {
  await page.goto("/");

  await expect(page.getByRole("link", { name: "Search" })).toBeVisible();
});
```

## What Not To Do

- Do not build a second provider setup in every test file when `src/test/render.tsx` already fits.
- Do not default to Playwright when a view or hook test is the real owning layer.
- Do not assert implementation details such as internal state or brittle DOM wrappers.
- Do not add frontend tests for untouched behavior just to make a change look bigger.
- Do not claim TDD if the tests were added only after the implementation was already complete.

## Frontend-Specific Mistakes To Avoid

### Wrong: Testing Through The Wrong Layer

Do not use a browser test for string normalization, mapper logic, or store transitions that belong in Vitest.

### Right: Match The Ownership Boundary

If the change is local to a feature view, test the feature view.
If the change is a pure utility, test the utility.
If the change is a real browser journey, use Playwright.

### Wrong: Rebuilding Providers In Every Test

Avoid repeating `ThemeProvider`, `MemoryRouter`, and app setup manually when the shared renderer already supports the case.

### Right: Reuse Shared Test Helpers

Prefer `renderWithProviders(...)` and extend it only when a new cross-test need is truly shared.

## Success Criteria

Frontend work is done when:

- the first test was written before the behavior change
- the test sits at the correct ownership layer
- changed behavior is covered by focused Vitest or Playwright tests
- the relevant frontend commands pass
- the implementation still follows Firefly's frontend structure

In this repo, good frontend TDD means small red-green-refactor loops with tests placed exactly where the UI architecture says the behavior belongs.
