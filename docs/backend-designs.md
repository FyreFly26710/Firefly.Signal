# Backend Design

## Purpose
This document explains the current backend shape of Firefly Signal.
It is a project-level overview of what services exist, what they are responsible for, and how the backend supports the personal job-search workflow.

## Product Role
The backend owns:
- authentication and user identity
- user profile and document metadata
- job ingestion and stored job catalog
- personal workflow state such as saved, applied, and rejected jobs
- bounded AI analysis workflows

## Current Services
- `Gateway API`
  Public entry point, request forwarding, and cross-cutting HTTP concerns.
- `Identity API`
  Authentication, user accounts, profile data, and document metadata.
- `Job Search API`
  Job import, stored catalog, search, workflow state, and job management.
- `AI API`
  AI-facing endpoints for job-fit and related analysis workflows.

## Shared Backend Libraries
- `Firefly.Signal.SharedKernel`
- `Firefly.Signal.ServiceDefaults`
- `Firefly.Signal.EventBus`
- `Firefly.Signal.EventBusRabbitMQ`
- `Firefly.Signal.IntegrationEventLogEF`

## Current Runtime Stack
- .NET 10
- EF Core
- PostgreSQL
- RabbitMQ where async workflows are justified
- Selective Redis when it adds clear value
- Docker on a Mac mini
- Cloudflare Tunnel for exposure

## Current Solution Shape

```text
services/api/
  Firefly.Signal.Api.slnx
  src/
    Firefly.Signal.Gateway.Api/
    Firefly.Signal.Identity.Api/
    Firefly.Signal.JobSearch.Api/
    Firefly.Signal.Ai.Api/
    Firefly.Signal.SharedKernel/
    Firefly.Signal.ServiceDefaults/
    Firefly.Signal.EventBus/
    Firefly.Signal.EventBusRabbitMQ/
    Firefly.Signal.IntegrationEventLogEF/
```

## Data And Storage
- PostgreSQL is the primary data store for the MVP workflow.
- Job records, user profiles, workflow state, and AI outputs are intended to remain persisted and reviewable.
- Document metadata is part of the product data model.
- Binary document storage is separate from relational metadata.

## Current Recommendation
Keep the backend understandable as a small set of explicit services with stable contracts.
The system should support the real personal workflow without adding distributed complexity earlier than necessary.
