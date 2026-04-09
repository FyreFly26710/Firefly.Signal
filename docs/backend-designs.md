# Backend Design

## Objectives
- Support a web-first career intelligence product with a clean path to future mobile clients.
- Keep initial operations simple while preserving clear backend boundaries.
- Make local development reliable with Docker and production deployment practical on a Mac mini.

## Detailed Reference Docs
Use the files in `docs/backend-designs/` as the detailed backend source of truth for implementation style:
- `overview.md`
- `coding-style.md`
- `solution-structure.md`
- `testing-style.md`
- `messaging-migrations-and-outbox.md`
- `local-docker-and-compose.md`
- `identity-api-direction.md`

## High-Level Architecture
The backend should start as a small set of focused .NET 10 services behind a gateway:
- `Gateway`
  - public entry point exposed through Cloudflare Tunnel
  - routing, auth enforcement, request correlation, and cross-cutting concerns
- `Identity API`
  - seeded local account flow first, with Google OAuth added next
  - JWT issuance
  - user CRUD and current-user/auth endpoints
- `Job Search Service`
  - provider-backed job search slice for live search
  - orchestrates provider lookups, normalization, ranking basics, and optional future persistence
- `AI API`
  - hosts no-op AI endpoints first
  - consumes RabbitMQ integration events for future enrichment workflows
- Future services only when needed
  - profile service
  - job tracking service
  - AI workflow service
  - document service

This should behave like a deliberate, modular platform rather than a fragmented fleet of services.

## Early Service Boundary Recommendation
Start with four deployable backend components:
1. API gateway
2. Identity API
3. Job search service
4. AI API

Keep the rest documented but unimplemented until the product proves the need.

## Request Flow
1. Web client sends request to Cloudflare-hosted domain.
2. Cloudflare routes API traffic through Tunnel to the Mac mini.
3. Gateway authenticates the request and forwards it to the appropriate backend service.
4. Identity API handles sign-in and token issuance for auth flows.
5. Job search service queries external job sources, normalizes results, and returns a stable contract.
6. Gateway returns a client-safe response with traceable metadata for logs and diagnostics.

## Technology Decisions
- Runtime: .NET 10
- Persistence: PostgreSQL via EF Core
- Messaging: RabbitMQ for asynchronous workflows only
- Caching or transient coordination: Redis only where it solves a specific problem
- Auth: Google OAuth using Gmail identity, issuing JWTs for API calls
- Deployment: Docker containers on a Mac mini
- Public exposure: Cloudflare Tunnel

## Design Principles
- Use vertical slices within each service.
- Keep application logic independent of transport and persistence details.
- Share contracts carefully; avoid a premature shared-kernel dependency tangle.
- Favor explicit DTOs at service boundaries.
- Centralize logging, tracing, and auth middleware patterns.
- Prefer synchronous APIs first; add messaging when business flow truly benefits.
- Use a repo-owned event bus abstraction with explicit subscriptions instead of a large broker framework.

## Suggested Solution Structure
```text
services/api/
  Firefly.Signal.Api.slnx
  Directory.Build.props
  Directory.Build.targets
  Directory.Packages.props
  docker-compose.yml
  src/
    Firefly.Signal.Ai.Api/
    Firefly.Signal.Gateway.Api/
    Firefly.Signal.Identity.Api/
    Firefly.Signal.JobSearch.Api/
    Firefly.Signal.SharedKernel/
    Firefly.Signal.ServiceDefaults/
    Firefly.Signal.EventBus/
    Firefly.Signal.EventBusRabbitMQ/
    Firefly.Signal.IntegrationEventLogEF/
  tests/
    Firefly.Signal.Identity.FunctionalTests/
    Firefly.Signal.JobSearch.UnitTests/
    Firefly.Signal.JobSearch.FunctionalTests/
```

This keeps Clean Architecture boundaries available without forcing every future service to share one rigid template.

## Data Strategy
### PostgreSQL
Use PostgreSQL for:
- persisted searches or saved jobs when introduced
- normalized job records when persistence becomes necessary
- user profile or preference data later

Do not make PostgreSQL the required source of truth for the first live search flow.
Introduce durable job storage only when a concrete persistence use case is real.

### Redis
Use Redis only if needed for:
- short-lived caching of external job queries
- rate-limit support
- distributed locks or short-lived coordination

Do not make Redis mandatory for the first end-to-end slice unless a concrete need appears.

### RabbitMQ
Use RabbitMQ when:
- background enrichment is valuable
- scheduled or batch collection is introduced
- non-blocking AI workflows are added

Keep the first synchronous search flow free of messaging if possible.

Current repo messaging shape:
- publishers depend on `Firefly.Signal.EventBus`
- RabbitMQ implementation lives in `Firefly.Signal.EventBusRabbitMQ`
- events are published to one durable direct exchange
- each consuming API owns one durable queue named by `SubscriptionClientName`
- handlers are registered explicitly with `AddSubscription<TEvent, THandler>()`
- keep request-response flows on HTTP; use RabbitMQ for asynchronous side effects and downstream processing

## Authentication And Authorization
- Use Google OAuth for sign-in, anchored to Gmail identity.
- Exchange validated identity for backend-issued JWTs.
- Keep JWT generation and validation centralized.
- Start with a single-user or low-user-count assumption, but avoid hard-coding identity shortcuts that block later expansion.
- Design role and permission claims lightly even if only one user exists initially.

## API Design Guidance
- Version public APIs from the start, even if only `v1`.
- Return stable result models for job cards and detail expansions.
- Normalize provider-specific quirks before they reach the frontend.
- Include paging and source metadata in search responses.
- Design idempotent endpoints where practical.

## Operational Guidance
- Use Docker Compose locally for PostgreSQL, pgweb, RabbitMQ, Redis, and service containers when they exist.
- Keep service configuration environment-based.
- Add health endpoints to the gateway and each service.
- Capture correlation IDs in every request path.
- Prefer structured logs from the start.

## Testing Strategy
- Unit tests for domain and application logic
- Integration tests for persistence and service boundaries
- Contract tests around gateway-to-service behavior when multiple services exist
- Keep end-to-end tests lean and focused on critical flows

## Phased Backend Implementation
### Phase 1
- Create the solution structure
- Create gateway, identity API, and job search service shells
- Add configuration, health checks, logging, and test projects

### Phase 2
- Implement search contract and first provider integration
- Add JWT auth flow and secure gateway forwarding
- Introduce Docker Compose for local dependencies

### Phase 3
- Add persistence only when needed
- Introduce caching if search behavior justifies it
- Add RabbitMQ-backed workflows for async enrichment or collection

## Open Questions
- Which public job sources are acceptable for the MVP from a reliability and terms perspective?
- Should the first release persist searches and results, or stay stateless?
- Is the gateway a thin reverse proxy plus auth layer, or should it also perform lightweight response shaping?

## Current Recommendation
Favor a thin gateway, a lightweight identity API, and one real backend service first.
That gives you a microservice-compatible starting point without paying the full coordination cost before the first product loop is proven.
