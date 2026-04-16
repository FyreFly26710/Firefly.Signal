---
name: frontend-e2e-testing
description: Use this skill when adding or refactoring Playwright coverage for Firefly Signal's web app under apps/web. Enforces repo-specific frontend E2E patterns, configuration, and smoke-flow coverage.
---

# Frontend E2E Testing

This skill defines the Playwright workflow for Firefly Signal under `apps/web/`.

Use it together with `frontend-patterns` and `frontend-tdd-workflow` when browser-level behavior is part of the change.

## When to Activate

- Adding Playwright coverage under `apps/web/tests/e2e`
- Validating critical user journeys through the running web app
- Refactoring frontend behavior that needs end-to-end confidence
- Fixing flaky browser tests
- Updating Playwright config, fixtures, or smoke flows

## Firefly Rules

### 1. E2E Covers User-Visible Flows

Use Playwright for real browser behavior that spans routing, rendering, and user interaction.

Good E2E targets in this repo:

- public search entry flows
- sign-in and protected-route behavior
- workspace navigation
- admin job-management flows
- regression smoke checks after significant UI refactors

Do not use Playwright for behavior already covered well by a colocated Vitest component or hook test.

### 2. Keep The Suite Small And Intentional

The current frontend suite is mostly Vitest-based.
Playwright should stay focused on critical journeys, not duplicate every component test in a browser.

Prefer:

- one smoke test for a route or flow
- one happy-path browser test per critical workflow
- one regression test for a previously broken cross-page behavior

Avoid:

- broad “click every button” scripts
- asserting styling details that belong in component tests
- duplicating all form validation cases already covered in Vitest

### 3. Follow The Existing Repo Layout

The current standard is:

```text
apps/web/
  playwright.config.ts
  tests/
    e2e/
      *.spec.ts
```

Keep:

- Playwright config in `apps/web/playwright.config.ts`
- browser specs in `apps/web/tests/e2e/`
- future fixtures under `apps/web/tests/fixtures/` only when reuse is real

Do not introduce a separate repo-root frontend E2E setup for normal web app work.

### 4. Prefer Accessible Selectors First

Use:

- `getByRole`
- `getByLabelText` equivalents through Playwright locators
- visible text when stable

Only add `data-testid` when the UI does not expose a stable accessible hook.

## Firefly Playwright Commands

Run frontend E2E work from `apps/web`:

```bash
npm run test:e2e
npm run test:e2e -- --grep "Search landing"
npm run test:e2e:headed
npm run test:e2e:ui
npm run test:e2e:install
```

Current config expectations:

- local base URL defaults to `http://127.0.0.1:4173`
- Playwright starts the Vite dev server automatically
- Chromium is the default project
- artifacts are written to ignored Playwright output folders

## E2E Workflow

### Step 1: State The User Journey

Describe the browser behavior in one or two sentences before coding.

Examples:

- "As a public user, I can reach the search entry points from the landing page."
- "As an admin, I can open manage jobs and hide a job after a delete conflict."

### Step 2: Choose The Smallest Browser Slice

Pick the narrowest flow that proves the behavior:

1. one route if the regression is page-local
2. one cross-page flow if routing or auth matters
3. one admin flow if permissions are part of the behavior

### Step 3: Write The Failing Spec

Add the browser spec under `apps/web/tests/e2e/`.

Current example:

- `apps/web/tests/e2e/search-landing.spec.ts`

### Step 4: Run Only The Relevant Browser Test

Prefer focused runs while iterating:

```bash
npm run test:e2e -- --grep "Search landing"
```

The first run should fail for the new behavior.

### Step 5: Implement The Smallest Passing UI Change

Add only the code needed to make the browser test pass.

Keep `frontend-patterns` in mind:

- route modules stay thin
- feature behavior stays in feature-owned code
- shared UI is intentional
- selectors should reflect accessible UI

### Step 6: Re-Run The Spec, Then Refactor

Once the focused Playwright test is green:

- simplify selectors
- remove duplication
- tighten naming
- keep the browser spec readable and small

### Step 7: Verify The Frontend Slice

Before finishing frontend work that changes browser behavior, run:

```bash
npm run lint
npm test
npm run test:e2e
npm run build
```

If the task only touched one browser flow and the full E2E suite would be unnecessarily heavy later on, explain which narrower Playwright command you ran instead.

## Patterns To Follow

### Route Smoke Pattern

```typescript
import { expect, test } from "@playwright/test";

test.describe("Search landing", () => {
  test("shows the public entry points", async ({ page }) => {
    await page.goto("/");

    await expect(page.getByRole("link", { name: "Discover" })).toBeVisible();
    await expect(page.getByRole("link", { name: "Search" })).toBeVisible();
    await expect(page.getByRole("link", { name: "Workspace" })).toBeVisible();
  });
});
```

### Protected Route Pattern

```typescript
import { expect, test } from "@playwright/test";

test("redirects unauthenticated users away from workspace routes", async ({ page }) => {
  await page.goto("/app");

  await expect(page).toHaveURL(/\/login/);
});
```

### Admin Flow Pattern

```typescript
import { expect, test } from "@playwright/test";

test("admin can reach manage jobs", async ({ page }) => {
  // Arrange auth state with a fixture or helper before navigation.
  await page.goto("/admin/manage-jobs");

  await expect(page.getByRole("heading", { name: "Manage jobs" })).toBeVisible();
});
```

## Flaky Test Rules

### Prefer Specific Waiting

Use:

- locator auto-waiting
- `expect(...).toBeVisible()`
- `expect(page).toHaveURL(...)`

Avoid:

- arbitrary `waitForTimeout(...)`
- selectors tied to fragile DOM structure

### Stabilize Browser Intent

If a flow depends on async page work:

- wait for the visible result the user actually sees
- avoid waiting on private implementation details unless there is no stable UI signal

### Quarantine Only With Context

If you must `fixme` or `skip` a flaky Playwright test, leave a concrete reason and issue reference.

## What Not To Do

- Do not put Playwright specs under `src/`.
- Do not use E2E to replace normal component, hook, or mapper tests.
- Do not assert MUI class names or implementation-only DOM wrappers in browser tests.
- Do not add multiple browsers or mobile projects unless the task explicitly needs them.
- Do not introduce screenshots, traces, or fixtures everywhere by default.

## Success Criteria

Frontend E2E work is done when:

- the spec lives under `apps/web/tests/e2e/`
- the test covers a real user-visible workflow
- selectors are accessible and stable
- the focused Playwright run passes
- the normal frontend verification commands still pass

In this repo, good E2E coverage is a small set of reliable smoke and workflow checks that complement Vitest instead of competing with it.
