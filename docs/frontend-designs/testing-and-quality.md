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
- stable query parsing and URL-building helpers
- store logic when it contains meaningful auth or session behavior

### Component Tests

Use component tests for:
- search form interaction
- loading, empty, error, and success rendering
- route-level feature composition when the behavior matters
- `View` behavior that coordinates feature hooks, search params, or screen states
- presentational components only when they contain meaningful conditional rendering or interaction logic

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
- search query parsing and search-param generation
- auth and session-store behavior
- auth guard behavior once protected routes exist

Do not spend early effort snapshot-testing every presentational component.
Do not require a test for every file.

## 4. What Should Be Tested

Test the code that protects user-critical behavior, non-trivial logic, or shared technical safety.

Good candidates for tests:
- feature `View` behavior
- feature hooks with async orchestration or derived state
- data mappers that change what the user sees
- validation helpers
- query parsing and URL-building helpers
- auth and session state flows
- shared technical utilities when a bug would affect multiple features

Usually not worth dedicated tests by default:
- thin route entry files
- trivial presentational wrappers
- static layout sections with no meaningful branching
- constants, simple type-only files, and pass-through glue code

## 5. Folder-Aligned Testing Rules

Testing should follow the same ownership boundaries as the code:

- `routes/`
  test only when route params, navigation state, or route composition contain meaningful behavior
- `features/<feature>/views/`
  highest-priority UI tests because `View`s own orchestration and user-visible states
- `features/<feature>/hooks/`
  test when hooks own async lifecycle, derived state, or feature actions
- `features/<feature>/mappers/`
  unit test when mapping affects displayed content or user decisions
- `components/`
  test shared components only when they contain meaningful conditional rendering, accessibility behavior, or interaction logic
- `store/`
  test stores when they hold important shared behavior such as session hydration, sign-in, or sign-out rules
- `lib/`
  test shared helpers when an error would have cross-feature impact

## 6. Current Early-Stage Coverage Direction

At the current stage of the app, the most valuable coverage is:
- search form validation and submission behavior
- search request lifecycle states
- search response mapping
- search query helpers
- session-store auth behavior

This is enough to protect the main early flow without building a heavy suite too soon.

## 7. Linting And Formatting

The frontend should use:
- ESLint for code quality
- Prettier-compatible formatting conventions where configured
- strict TypeScript settings appropriate for app code

## 8. Frontend Quality Rule Set

- test behavior, not implementation trivia
- keep component tests close to the feature that owns the behavior
- prefer a few high-signal tests over many low-signal tests
- keep lint and type checking part of the normal frontend workflow
- when a test reveals unstable architecture, prefer fixing the behavior rather than deleting the test
