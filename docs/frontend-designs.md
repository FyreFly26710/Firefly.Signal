# Frontend Design

## Purpose
This document explains the current frontend surface of Firefly Signal.
It exists to help a reader understand what the web app is for, which parts of the product it owns, and how the frontend is currently organized at a high level.

## Product Role
The frontend is the web-first client for the personal job-search workflow.
It covers:
- public search and job discovery
- authentication entry
- protected workspace views
- profile management
- admin-facing job management

## Current Stack
- React 18
- TypeScript
- Vite
- Zustand
- MUI
- Tailwind CSS

## Current Source Layout
The app currently uses this high-level structure:

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

## Current Route Surface
- `/`
  Search landing page
- `/search`
  Search results
- `/jobs/:id`
  Public job detail
- `/login`
  Auth entry
- `/app`
  Authenticated workspace home
- `/app/jobs`
  Protected job workflow area
- `/app/profile`
  Profile area
- `/admin/...`
  Admin management flows

## UX Direction
- Fast access to search and job review
- Clear loading, empty, permission, and error states
- Desktop-first layouts that remain usable on mobile widths
- A product surface that feels intentional enough for external review

## Current Recommendation
Keep the frontend easy to explain:
- clear feature ownership
- explicit route surface
- typed API integration
- coherent design language across public and protected views
