# Backend Designs Overview

This folder is the detailed backend source of truth for Firefly Signal.
It is intentionally self-contained so it remains useful after any temporary reference project is removed.

## Goals
- Preserve the good API architecture patterns already identified for this repo.
- Give future implementation work concrete conventions instead of loose preferences.
- Keep backend work practical for a single maintainer.
- Support GitHub issue to branch to PR delivery with minimal re-decision.

## Backend Direction In One Page
- Use `.NET 10`.
- Use `.slnx` solution files only.
- Do not use `.slnf`.
- Use a thin gateway in front of backend APIs.
- Start with a real `JobSearch` service and a lightweight custom `Identity` API.
- Use PostgreSQL with EF Core.
- Use RabbitMQ for integration events and async workflows when a real need exists.
- Use Redis only when caching or coordination clearly justifies it.
- Use Dockerfiles per API.
- Use Docker Compose for local infrastructure: PostgreSQL, pgweb, Redis, RedisInsight, RabbitMQ.
- Do not use Aspire.
- Do not use gRPC for the current plan.
- Keep a shared migration helper similar to `MigrateDbContextExtensions`.
- Keep a repo-owned event bus abstraction with a simplified eShop-style RabbitMQ implementation.

## Read Order
1. `solution-structure.md`
2. `coding-style.md`
3. `messaging-migrations-and-outbox.md`
4. `testing-style.md`
5. `identity-api-direction.md`
6. `local-docker-and-compose.md`

## What These Docs Optimize For
- small, reviewable API services
- explicit startup and dependency wiring
- minimal API endpoints with typed results
- centralized build and package conventions
- predictable testing setup
- future-proof messaging and migration patterns without premature complexity

## What These Docs Explicitly Avoid
- identity platform over-engineering
- service sprawl before the product earns it
- hidden infrastructure conventions
- framework-driven complexity that is hard to run locally
- repo patterns that only make sense when a sample app is still present
