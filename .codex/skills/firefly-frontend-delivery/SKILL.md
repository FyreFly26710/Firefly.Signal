---
name: firefly-frontend-delivery
description: Implement frontend changes in the Firefly Signal repository. Use when Codex is editing the React web app, frontend tests, styling, routing, state, or frontend build configuration in this repo.
---

# Firefly Frontend Delivery

Start by reading:
- `AGENTS.md`
- `docs/plans.md`
- `docs/frontend-designs.md`
- `docs/frontend-designs/architecture.md`
- the relevant file in `docs/frontend-designs/`

Implementation rules:
- keep route files thin
- keep feature behavior inside feature folders
- prefer local state before Zustand
- keep API modules small and typed
- use MUI for accessible primitives
- use Tailwind for layout, spacing, and tokens
- keep loading, empty, error, and success states explicit

When touching styling:
- preserve token-driven styling
- avoid default-library-looking screens
- keep responsive behavior intentional and mobile-safe

When touching tests:
- prefer behavior-focused tests
- keep unit and component tests close to the feature that owns the behavior
- avoid low-signal snapshot sprawl

Before finishing:
- run the relevant frontend checks that exist
- summarize assumptions
- note any frontend design docs that changed or should change
