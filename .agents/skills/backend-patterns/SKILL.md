---
name: backend-patterns
description: Firefly Signal backend structure and coding patterns for .NET 10 services. Use when refactoring or adding backend APIs, shared backend libraries, DbContexts, external provider adapters, transport contracts, or EventBus code to follow the repo's draft backend layout instead of ad hoc service shapes.
---

# Backend Patterns

Use this skill for Firefly Signal backend work under `services/api/`.

## What Good Looks Like

- `Program.cs` stays thin.
- `Extensions/ApplicationServiceExtensions.cs` owns service registration.
- API transport models live in `Contracts/Requests` and `Contracts/Responses`.
- Endpoint modules live in `Apis/`.
- Read and write application logic are split cleanly.
- Domain entities and constants are separated from infrastructure.
- External providers live under `Infrastructure/Services/<ProviderName>/`.
- Event bus abstractions are shared and structured consistently.
- Mapping is explicit. Do not use AutoMapper.

## Service Layout

For feature APIs such as `Identity.Api` and `JobSearch.Api`, prefer this layout:

```text
Feature.Api/
  Apis/
    <Resource>Api.cs
    <Resource>ApiMappers.cs
  Contracts/
    Requests/
    Responses/
  Application/
    Commands/
    Queries/
    Mappers/
    IntegrationEventHandlers/
  Infrastructure/
    Persistence/
    Services/
  Domain/
    Entities/
    Consts/
    DomainEvents/
  Extensions/
  Options/
  GlobalUsings.cs
  Program.cs
```

Gateway and AI APIs can stay lighter, but they should still follow the same spirit:
- no fat `Program.cs`
- explicit `Options/`
- explicit `Infrastructure/Services/`
- no transport models or helper types hidden inside startup

## Startup Rules

`Program.cs` should only:
- create the builder
- add shared defaults
- call `AddApplicationServices()`
- add problem details and OpenAPI
- build the app
- map endpoints
- run the app

Do not put these in `Program.cs`:
- option classes
- inline auth configuration types
- HTTP client classes
- database wiring details
- large endpoint handlers

Startup-owned types belong in normal files, usually `Options/` or `Infrastructure/Services/`.

## Application Layer Rules

Split reads and writes intentionally.

- Put read-side interfaces and implementations in `Application/Queries/`.
- Put write-side commands and handlers in `Application/Commands/`.
- Put each command in its own `Application/Commands/<Action>Command.cs` file.
- Put each command handler in its own `Application/Commands/<Action>CommandHandler.cs` file.
- Put entity-to-response mapping in `Application/Mappers/`.
- Keep request-to-command or request-to-query translation in `Apis/*ApiMappers.cs`.
- Keep query execution out of APIs. APIs translate transport input, then call a query service or `Mediator.Send(...)`.
- Use MediatR for every write. APIs must not call write services directly.
- Each write action gets one command and one handler. Do not group multiple commands or multiple handlers into one file.
- Keep query interfaces explicit. Queries do not need MediatR unless a task explicitly asks for it.

## API Layer Rules

Endpoint modules should:
- define route groups
- validate transport-level input
- translate request models into application calls
- return typed results

Endpoint modules should not:
- talk directly to EF Core
- own provider integration logic
- parse claims all over the place
- contain large mapping blocks that belong in mappers

Use a small current-user abstraction such as `IIdentityService` or another narrow adapter.

## Contracts And Mapping

- Transport contracts live under `Contracts/Requests` and `Contracts/Responses`.
- Do not mix request and response records into broad `Application/*Contracts.cs` buckets.
- Do not use anonymous payloads for stable endpoints.
- When constructing records in non-trivial mappings, prefer named arguments.

## Infrastructure Rules

Persistence:
- DbContext and migrations stay under `Infrastructure/Persistence/`
- keep the migration helper pattern
- keep seeders idempotent

External integrations:
- provider adapters and their models live under `Infrastructure/Services/<ProviderName>/`
- keep third-party request/response models close to the adapter that owns them

## Domain Rules

- Put entities in `Domain/Entities/`
- put enums and constants in `Domain/Consts/`
- keep domain logic on the entity when it is truly domain behavior
- do not let EF or transport concerns leak into domain types

## Event Bus Rules

`Firefly.Signal.EventBus` should follow this structure:

```text
Firefly.Signal.EventBus/
  Abstractions/
    EventBusSubscriptionInfo.cs
    IEventBus.cs
    IEventBusBuilder.cs
    IIntegrationEventHandler.cs
  Events/
    IntegrationEvent.cs
    <Feature>/<SomethingHappened>IntegrationEvent.cs
  Extensions/
    EventBusBuilderExtensions.cs
```

Rules:
- integration events describe something that already happened
- shared event contracts live in the event bus project when multiple services need them
- extensions own registration helpers

## Shared Kernel Rules

Keep shared code small and stable:
- entity base types
- id generation
- EF helpers
- narrow identity/current-user abstractions
- pipeline helpers only when already justified

Do not keep duplicate copies of the same shared type in different namespaces or folders.

