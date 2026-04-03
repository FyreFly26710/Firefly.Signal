# Solution Structure

This document defines the preferred backend solution structure for Firefly Signal.

## Core Decisions
- Put backend work under `services/api/`.
- Use one `.slnx` solution file for the backend.
- Do not use `.slnf`.
- Start with a small number of projects and add only when the boundary is real.

## Recommended Initial Shape

```text
services/api/
  Firefly.Signal.Api.slnx
  Directory.Build.props
  Directory.Build.targets
  Directory.Packages.props
  docker-compose.yml
  src/
    Firefly.Signal.Gateway.Api/
    Firefly.Signal.Identity.Api/
    Firefly.Signal.JobSearch.Api/
    Firefly.Signal.ServiceDefaults/
    Firefly.Signal.EventBus/
    Firefly.Signal.EventBusRabbitMQ/
    Firefly.Signal.IntegrationEventLogEF/
  tests/
    Firefly.Signal.JobSearch.UnitTests/
    Firefly.Signal.JobSearch.FunctionalTests/
```

## Project Responsibilities

### Gateway
Responsibilities:
- entry point for frontend API access
- auth enforcement
- route forwarding
- health endpoints
- rate limiting and correlation if needed

Non-responsibilities:
- business logic
- search orchestration
- database ownership

### Identity API
Responsibilities:
- Google OAuth flow
- token issuance
- current-user endpoint
- local auth-related persistence when needed

Non-responsibilities:
- full identity platform behavior
- generic federation framework
- overbuilt permission system

### JobSearch API
Responsibilities:
- job search endpoints
- provider orchestration
- response normalization
- feature-specific background events if needed

Keep `Application`, `Domain`, and `Infrastructure` as folders inside the API project until the codebase proves they need to become separate projects.

### Shared Event Bus Projects
Responsibilities:
- reusable abstractions
- RabbitMQ implementation
- integration event log support

## When To Collapse Projects

If a service is very small, it is acceptable to keep everything in one API project:
```text
Firefly.Signal.JobSearch.Api/
  Application/
  Domain/
  Infrastructure/
```

Split those folders into separate projects only when:
- business rules start growing
- test setup becomes awkward
- persistence and orchestration concerns are clearly diverging

## Build Files

### `Directory.Build.props`
Use for:
- warnings as errors
- nullable
- implicit usings
- shared build flags

### `Directory.Build.targets`
Use sparingly for:
- repo-wide build hooks
- uncommon compile or publish tweaks

### `Directory.Packages.props`
Use for:
- centralized package versions
- version consistency across APIs and test projects

## Example `.slnx` Membership

The backend solution should eventually include:
- gateway API
- identity API
- job search API
- event bus libraries
- integration event log library
- all backend tests

Keep frontend out of this solution.

## Shared Source Guidance

Keep tiny cross-service helpers in a shared project or colocated in the owning API.
Do not keep deep source-linked shared folder trees.

## Naming Guidance

Use consistent project names:
- `Firefly.Signal.Gateway.Api`
- `Firefly.Signal.Identity.Api`
- `Firefly.Signal.JobSearch.Api`
- `Firefly.Signal.ServiceDefaults`
- `Firefly.Signal.EventBus`
- `Firefly.Signal.EventBusRabbitMQ`
- `Firefly.Signal.IntegrationEventLogEF`

Keep namespaces aligned with project names.

## Solution Structure Summary
- one backend `.slnx`
- a flat `src/` with projects at one level
- a thin gateway
- a custom identity API
- one real job search service
- small shared infrastructure projects
- tests separated by style and service
