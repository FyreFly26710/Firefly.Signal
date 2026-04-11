# Frontend Solution Structure

This document defines the intended frontend project layout for Firefly Signal.

## 1. App Shape

The frontend starts as one web app:

```text
apps/
  web/
```

Future clients may be added later, but the current implementation target is web only.

## 2. Recommended Frontend Structure

```text
apps/web/
  public/
  src/
    app/
    components/
    features/
    lib/
    routes/
    store/
    styles/
    test/
```

## 3. Folder Responsibilities

### `src/app/`

Owns app-level composition:
- providers
- router setup
- theme setup
- app shell
- app-wide bootstrapping

Examples:
- `App.tsx`
- `providers.tsx`
- `router.tsx`
- `theme.ts`

### `src/components/`

Owns reusable UI pieces that are not tied to one feature.

Examples:
- page containers
- section headers
- common feedback blocks
- reusable cards or chips once repeated

Do not put feature-specific business components here.

### `src/features/`

This is the main implementation area.
Each feature owns its UI, types, API mapping, and feature-specific state.

Example:

```text
features/
  search/
    api/
    components/
    hooks/
    mappers/
    types/
```

### `src/lib/`

Owns shared technical utilities:
- API client base
- environment helpers
- date formatting helpers
- error normalization

This folder should stay small and boring.

### `src/routes/`

Owns route entry files.
Each route should mostly compose feature-owned modules.

### `src/store/`

Owns global Zustand stores only.
If a state slice is local to one feature or route, keep it out of this folder.

### `src/styles/`

Owns global CSS entrypoints, Tailwind layers, tokens, and app-wide visual definitions.

### `src/test/`

Owns test utilities:
- render helpers
- common mocks
- test setup

## 4. Feature Structure Guidance

A feature folder can grow like this:

```text
features/
  search/
    api/
      search.api.ts
    components/
      SearchForm.tsx
      SearchResults.tsx
      SearchResultCard.tsx
    hooks/
      useJobSearch.ts
    mappers/
      search.mappers.ts
    types/
      search.types.ts
```

Use only the subfolders that the feature actually needs.

## 5. Route Strategy

Start with a small route surface:
- `/`
- `/search`
- `/jobs/:id`
- `/login`
- `/app`
- `/saved`
- `/applied`
- `/profile`
- `/admin/*`

The route surface should stay focused, but the MVP is no longer assumed to be a one-route app.

## 6. Future Mobile Compatibility Rule

The web app should not pretend to be shared mobile UI, but it should preserve reusable assets for later:
- API request and response contracts
- feature state models
- domain naming
- validation rules
- UX state definitions

Those should stay separated enough that a future mobile client can reuse the intent even if the UI code is entirely different.
