# Backend Design

## Objectives
- Support a web-first personal career intelligence product with a clean path to future mobile clients.
- Keep initial operations simple while preserving clear backend boundaries.
- Make the MVP capable of storing and managing the full personal workflow, not only transient search.

## Detailed Reference Docs
Use the files in `docs/backend-designs/` as the detailed backend source of truth for implementation style:
- `overview.md`
- `coding-style.md`
- `solution-structure.md`
- `testing-style.md`
- `messaging-migrations-and-outbox.md`
- `local-docker-and-compose.md`
- `identity-api-direction.md`
- `adzuna-api.md`
- `data-model-plan.md`

## High-Level Architecture
The backend should start as a small set of focused .NET 10 services behind a gateway:
- `Gateway`
  - public entry point exposed through Cloudflare Tunnel
  - routing, auth enforcement, request correlation, and cross-cutting concerns
- `Identity API`
  - seeded local account flow first, with Google OAuth added later if still needed
  - JWT issuance
  - user CRUD, current-user/auth endpoints, role claims, and user profile ownership
- `Job Search API`
  - provider-backed ingestion and persisted job catalog
  - admin job management, search, filtering, sorting, save/apply/reject workflow, and AI result persistence
- `AI API`
  - bounded job-fit and explanation endpoints
  - may use RabbitMQ later for asynchronous enrichment, but should start with explicit workflow contracts
- Future services only when needed
  - document service
  - profile service
  - AI workflow service

For the MVP, prefer extending the current Identity and Job Search services instead of immediately introducing more deployable services.

## Early Service Boundary Recommendation
Start with four deployable backend components:
1. API gateway
2. Identity API
3. Job Search API
4. AI API

Keep document and profile capabilities inside existing service boundaries until there is real pressure to split them out.

## Request Flow
1. Web client sends request to the Cloudflare-hosted domain.
2. Cloudflare routes API traffic through Tunnel to the Mac mini.
3. Gateway authenticates the request and forwards it to the appropriate backend service.
4. Identity API handles sign-in, token issuance, user-role claims, and profile ownership concerns.
5. Job Search API stores imported jobs, handles search and workflow state, and persists user-linked AI results.
6. AI API evaluates selected jobs against user profile material when requested.
7. Gateway returns client-safe responses with traceable metadata for logs and diagnostics.

## Technology Decisions
- Runtime: .NET 10
- Persistence: PostgreSQL via EF Core
- Messaging: RabbitMQ for asynchronous workflows only
- Caching or transient coordination: Redis only where it solves a specific problem
- Auth: seeded local auth first, with room for Google OAuth later
- Deployment: Docker containers on a Mac mini
- Public exposure: Cloudflare Tunnel

## Design Principles
- Use vertical slices within each service.
- Keep application logic independent of transport and persistence details.
- Favor explicit DTOs at service boundaries.
- Centralize logging, tracing, auth, and permission patterns.
- Prefer synchronous APIs first; add messaging when business flow truly benefits.
- Model the real personal workflow directly instead of hiding it behind placeholder abstractions.

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

## Data Strategy
### PostgreSQL
Use PostgreSQL as a first-class source of truth for the MVP for:
- user accounts and roles
- user profile information
- document metadata
- imported and normalized job records
- save/apply/reject workflow state
- application notes
- AI rating and explanation records

The MVP is not a stateless search-only product.
Persisted workflow data is part of the core requirement.

### File Storage
Use a simple local-first file storage approach for uploaded CVs, cover letters, and other user files during the MVP.
Persist document metadata in PostgreSQL and keep storage implementation easy to replace later.

### Redis
Use Redis only if needed for:
- external provider result caching
- rate-limit support
- short-lived coordination around provider ingestion or AI execution

### RabbitMQ
Use RabbitMQ when:
- provider ingestion becomes scheduled or asynchronous
- AI analysis needs background batch processing
- import and enrichment workflows benefit from decoupling

Do not force async workflows into the first implementation if synchronous request flows are still clear and manageable.

## Authentication And Authorization
- Support at least `admin` and `test-admin` roles.
- `admin` can perform backend write operations.
- `test-admin` should see the same frontend areas but be restricted to backend read-only access.
- Keep JWT generation and validation centralized.
- Model permission behavior explicitly instead of relying on frontend-only restrictions.

## API Design Guidance
- Version public APIs from the start, even if only `v1`.
- Return stable result models for job cards, job detail, workflow lists, user profile data, and AI outputs.
- Normalize provider-specific quirks before they reach the frontend.
- Keep search, workflow, profile, and AI contracts explicit and typed.
- Design bulk admin and AI-selection workflows carefully so they remain reviewable and auditable.

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
- Functional tests around auth, job management, search, and personal workflow endpoints

## Phased Backend Implementation
### Phase 1
- Formalize user roles and permission rules
- Add user profile and document metadata persistence
- Keep auth flows simple but explicit

### Phase 2
- Persist imported jobs and provider refresh runs
- Add admin CRUD and moderation workflows for jobs
- Add deduplication and provider auditability

### Phase 3
- Add stored search, filtering, sorting, and postcode-distance support
- Add personal save/apply/reject workflow data
- Add notes and application document linkage

### Phase 4
- Add AI analysis contracts, persistence, and bounded execution flow
- Move long-running work to RabbitMQ only if needed

## Open Questions
- Which provider or providers should be implemented first for dependable UK developer-job coverage?
- Should uploaded document binaries stay on local disk for the full MVP, or move earlier to a managed object store?
- Does postcode-distance filtering rely on a local postcode lookup dataset, an external postcode API, or both?

## Current Recommendation
Favor a thin gateway, a lightweight identity API, one strong job-search/workflow service, and a bounded AI API.
That matches the current codebase, supports the real MVP, and keeps the system understandable for one maintainer and outside reviewers.
