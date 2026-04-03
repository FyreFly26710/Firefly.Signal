# State, Routing, And API Design

This document defines how frontend state, routing, and backend communication should be handled in Firefly Signal.

## 1. State Strategy

Prefer this order:
1. local component state
2. feature-level hook state
3. Zustand store only when state is genuinely shared

Use Zustand for:
- auth/session state
- persisted user preferences
- cross-route filters when that becomes real
- app-wide UI concerns that truly span features

Do not put every form field into a global store.

## 2. Route Design

Routes should represent user-visible screens, not implementation details.

Route files should:
- read route params
- compose feature components
- stay thin

Feature folders should own the actual UI behavior.

## 3. Data Loading

For the first version:
- load data directly in feature hooks or route-level feature containers
- keep request lifecycle explicit
- avoid adding a dedicated data framework unless the app proves it is needed

Each async flow should explicitly represent:
- idle
- loading
- success
- empty
- error

## 4. API Client Pattern

Keep backend communication behind small typed modules.

Recommended shape:

```text
features/search/api/search.api.ts
lib/http/client.ts
```

Guidelines:
- one small API module per feature area
- keep request building and response parsing in the API layer
- keep UI-specific mapping close to the feature
- normalize backend errors into a stable frontend shape where useful

## 5. API Contract Handling

Backend DTOs should not leak everywhere in the UI.
Use mappers when the UI needs a cleaner display model.

Examples of useful mapping:
- formatting source labels
- combining location fields
- deriving display tags

Do not map just for the sake of mapping.

## 6. Form Handling

For the MVP search form:
- keep form state local
- validate required inputs clearly
- preserve the submitted values in the results view
- make retry easy

If forms become larger later, introduce a form library only when the complexity is real.

## 7. Error Handling

Define a stable frontend error shape for API failures.

At minimum the UI should be able to display:
- a user-friendly message
- retry guidance when appropriate
- whether the failure is likely validation, auth, or server-side

## 8. Authentication Direction

Frontend auth rules for the first web client:
- keep token handling simple
- centralize auth state if persisted client-side
- keep protected-route behavior explicit
- do not scatter token parsing across features

Future mobile clients should be able to follow the same auth contract without depending on web-only implementation details.

## 9. Request Flow Rule

Use the gateway as the default frontend API entry point.
Keep frontend HTTP calls straightforward and typed.
Do not make the frontend aware of backend service-to-service RabbitMQ flows.
