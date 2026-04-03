# Frontend Testing And Quality

This document defines the expected frontend testing and quality approach for Firefly Signal.

## 1. Quality Goals

- keep the frontend easy to change confidently
- focus testing on real user behavior and core feature logic
- avoid heavy test suites that slow down a single-maintainer workflow

## 2. Testing Layers

### Unit Tests

Use unit tests for:
- pure helpers
- data mappers
- small validation logic
- feature-level state helpers

### Component Tests

Use component tests for:
- search form interaction
- loading, empty, error, and success rendering
- route-level feature composition when the behavior matters

### End-To-End Tests

Keep E2E coverage narrow and valuable.
When the app exists, focus on:
- entering postcode and keyword
- submitting search
- receiving results
- handling empty or error responses

## 3. Test Priorities For MVP

Prioritize tests for:
- search input validation
- request status transitions
- response mapping that affects UI rendering
- auth guard behavior once protected routes exist

Do not spend early effort snapshot-testing every presentational component.

## 4. Linting And Formatting

The frontend should use:
- ESLint for code quality
- Prettier-compatible formatting conventions where configured
- strict TypeScript settings appropriate for app code

## 5. Frontend Quality Rule Set

- test behavior, not implementation trivia
- keep component tests close to the feature that owns the behavior
- prefer a few high-signal tests over many low-signal tests
- keep lint and type checking part of the normal frontend workflow
