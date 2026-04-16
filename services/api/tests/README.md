# Backend Test Standard

This folder is the backend test convention for `services/api/`.

## Project Split

Use two test layers per feature service:

- `*.UnitTests`
  Pure domain rules, pure mappers, pure validation helpers, and other deterministic logic with no app host.
- `*.FunctionalTests`
  Command handlers, query classes, endpoint behavior, auth behavior, exception-to-problem-details mapping, and persistence-backed workflows.

Current feature test projects:

- `Firefly.Signal.Identity.UnitTests`
- `Firefly.Signal.Identity.FunctionalTests`
- `Firefly.Signal.JobSearch.UnitTests`
- `Firefly.Signal.JobSearch.FunctionalTests`

`Gateway` and `Ai` stay out of this standard for now. Add tests there when those services gain meaningful domain or application behavior.

## Stack

- Test framework: `MSTest`
- Test doubles: `NSubstitute`
- Functional database default: a real relational provider in tests, not mocked persistence
- HTTP host: `WebApplicationFactory<Program>` when HTTP is part of the behavior under test

## Placement Rules

Put the first failing test at the lowest layer that owns the behavior:

1. Domain rule or normalization: `UnitTests`
2. Use-case behavior in a command handler or query: `FunctionalTests`
3. Transport/auth/validation/problem-details behavior: add an endpoint functional test

Do not:

- mock `DbContext`
- mock MediatR just to test APIs
- add tests for thin `Program.cs`
- write endpoint-only tests for business rules that belong to domain or application code

## TDD Workflow

For backend work written by Codex or Claude Code:

1. Add a failing test first.
2. Implement the smallest code change that makes it pass.
3. Refactor only with the test suite green.
4. Add endpoint coverage only when the change affects route binding, auth, validation, status codes, or exception mapping.

## Seeded Reference Examples

`Firefly.Signal.JobSearch` is the reference service for the initial standard.

- `UnitTests/Domain/JobApplicationTests.cs`
  Notes are trimmed and whitespace-only values become `null`.
- `FunctionalTests/Application/AdvanceApplicationStatusCommandHandlerTests.cs`
  Status transitions reject invalid moves and append new entries on success.
- `FunctionalTests/Application/JobSearchQueriesTests.cs`
  Paging defaults and user-state projection are exercised against a relational database.
- `FunctionalTests/Api/JobApplicationApiTests.cs`
  `/api/job-search/jobs/{id}/apply/status` returns `400`, `401`, `404`, and `200` for the expected HTTP concerns.

Keep helpers small and local to this folder. Prefer a couple of focused fixtures over building a custom testing framework.
