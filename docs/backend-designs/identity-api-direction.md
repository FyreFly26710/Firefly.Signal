# Identity API Direction

This document defines the identity approach for Firefly Signal.

## Core Decision

Build a custom identity API that supports:
- Google OAuth sign-in
- Gmail identity as the primary login path
- JWT issuance for Firefly Signal backend APIs

Do not build or copy a full identity-server-style platform.

## Identity API Responsibilities
- start Google OAuth flow
- handle Google callback
- validate OAuth state and token responses
- look up or provision the local user record when needed
- issue Firefly Signal JWTs
- return current-user information to the frontend

## Recommended Route Shape
- Keep identity routes under the gateway-forwarded groups:
- `POST /api/auth/google/start`
- `GET /api/auth/google/callback`
- `POST /api/auth/logout`
- `GET /api/auth/me`
- `GET /api/users/profile`
- `PUT /api/users/profile`
- `GET /api/users/documents`
- `POST /api/users/documents`

The exact route names can change, but keep the surface small and aligned with the gateway route groups.

## Recommended Project Shape

```text
src/
  Firefly.Signal.Identity.Api/
```

Keep supporting folders such as `Identity/`, `Auth/`, or `Infrastructure/` inside the API project until the codebase proves they need to be split out.

## JWT Guidance

The identity API should issue backend-facing JWTs that include:
- `sub`
- `name` when available
- any minimal internal claims the APIs need

Keep claims small and purposeful.

Do not front-load:
- complex role hierarchies
- generic authorization policy frameworks
- multi-client identity registration systems

## Service-Side Identity Access

Other APIs should access identity through a thin adapter:

```csharp
public interface IIdentityContext
{
    string? GetUserId();
    string? GetUserName();
}
```

This keeps claim parsing out of endpoint handlers and application code.

## Testing Guidance

Unit test:
- token issuance helpers
- OAuth state validation
- claim mapping

Functional test:
- auth endpoints
- callback edge cases
- protected route behavior using fake auth where appropriate

Do not require live Google auth for normal test runs.

## Practical Guidance

For the first release:
- keep the identity API small
- keep the login surface narrow
- keep the token shape simple
- avoid turning personal-use auth into a platform project
