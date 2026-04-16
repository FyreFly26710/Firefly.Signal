---
name: backend-tdd-workflow
description: Use this skill when writing backend features, fixing backend bugs, or refactoring code under services/api. Enforces Firefly Signal's backend TDD workflow with UnitTests and FunctionalTests aligned to the repo's clean-architecture boundaries.
---

# Backend TDD Workflow

This skill defines the backend testing workflow for Firefly Signal under `services/api/`.

Use it together with `backend-patterns` when changing backend behavior.

## When to Activate

- Writing new backend features or endpoints in `services/api`
- Fixing backend bugs
- Refactoring backend behavior
- Changing command handlers, query classes, domain entities, or endpoint contracts
- Adding backend tests or expanding existing backend coverage

## Firefly Rules

### 1. Tests Before Code

Always start with a failing test at the lowest layer that owns the behavior.

Use:

- `*.UnitTests` for pure domain rules and pure helper logic
- `*.FunctionalTests` for command handlers, query classes, and HTTP endpoint behavior

### 2. Follow The Existing Repo Split

The backend standard is:

- `services/api/tests/Firefly.Signal.<Feature>.UnitTests`
- `services/api/tests/Firefly.Signal.<Feature>.FunctionalTests`

Do not add a third backend test layer such as `IntegrationTests` in normal feature work unless the task explicitly requires it.

### 3. Coverage Means Behavior, Not Metrics Theater

The repo does not currently standardize on a numeric backend coverage threshold.
Do not invent one in implementation tasks.

Instead, ensure coverage is meaningful:

- happy path
- relevant edge cases
- expected error paths
- transport/auth behavior when HTTP is part of the change

### 4. Test Real Ownership Boundaries

Place tests where the logic actually lives:

- Domain normalization and invariants: `UnitTests`
- Command and query behavior: `FunctionalTests`
- Route binding, auth, status codes, validation, and problem-details behavior: API `FunctionalTests`

## Firefly Backend Test Types

### Unit Tests

Use for:

- domain entities such as `UserProfile`, `JobApplication`, `UserAccount`
- pure mappers
- pure validation helpers
- deterministic support logic with no app host

Do not:

- boot the web host
- mock through HTTP
- involve EF Core for simple domain behavior

### Functional Tests

Use for:

- MediatR command handlers
- query classes
- endpoint behavior
- auth behavior
- exception-to-problem-details behavior
- persistence-backed workflows

Default patterns in this repo:

- relational functional tests can use small SQLite harnesses when persistence semantics matter
- HTTP tests use `WebApplicationFactory<Program>` with the service's `Testing` environment
- service-owned fixtures stay local to `services/api/tests`

## TDD Workflow

### Step 1: State The Backend Behavior

Describe the behavior in one or two sentences before coding.

Examples:

- "As an authenticated user, I can advance an application status so the next workflow stage is persisted."
- "As a signed-in user, I can upsert my profile so later job-review flows use the latest profile context."

### Step 2: Choose The Lowest Owning Layer

Pick the first failing test target with this rule:

1. If the behavior is pure domain logic, start in `UnitTests`.
2. If the behavior is a use case in a handler or query, start in `FunctionalTests`.
3. If the change affects request binding, auth, validation, or response shape, add an API functional test.

### Step 3: Write The Failing Test

Write the smallest failing test that proves the missing behavior.

Examples already in the repo:

- `services/api/tests/Firefly.Signal.JobSearch.UnitTests/Domain/JobApplicationTests.cs`
- `services/api/tests/Firefly.Signal.JobSearch.FunctionalTests/Application/AdvanceApplicationStatusCommandHandlerTests.cs`
- `services/api/tests/Firefly.Signal.Identity.FunctionalTests/Api/AuthApiTests.cs`

### Step 4: Run The Smallest Relevant Test Command

Prefer targeted commands while iterating:

```bash
dotnet test services/api/tests/Firefly.Signal.JobSearch.UnitTests/Firefly.Signal.JobSearch.UnitTests.csproj
dotnet test services/api/tests/Firefly.Signal.Identity.FunctionalTests/Firefly.Signal.Identity.FunctionalTests.csproj
```

The first run should fail for the new behavior.

### Step 5: Implement The Smallest Passing Change

Add only the code needed to make the new test pass.

Follow `backend-patterns`:

- keep `Program.cs` thin
- keep writes in command handlers
- keep reads in query classes
- keep mapping explicit
- keep domain behavior on entities when it is truly domain behavior

### Step 6: Re-Run Focused Tests, Then Refactor

Once the targeted test is green:

- remove duplication
- improve naming
- simplify helpers
- keep the tests green throughout

### Step 7: Verify The Service Slice

Before finishing backend work, run the relevant project or solution tests:

```bash
dotnet test services/api/Firefly.Signal.Api.slnx
```

If the task only touched one service and the full solution run is unnecessarily heavy, explain what narrower test command you ran instead.

## Test Placement Rules

### Put In UnitTests

- note trimming
- postcode normalization
- role/default normalization
- guard behavior on domain entities
- pure mapper behavior

### Put In FunctionalTests/Application

- handler creates vs updates
- query ordering and filtering
- persistence-backed state transitions
- command/query null vs exception behavior

### Put In FunctionalTests/Api

- `401`, `404`, `400`, `409`, `200`, `201`, `204` outcomes
- auth-required flows
- request contract validation
- explicit route and body binding
- problem-details responses

## Patterns To Follow

### Domain Unit Test Pattern

```csharp
[TestMethod]
public void Create_NormalizesPostcode()
{
    var profile = UserProfile.Create(
        userAccountId: 42,
        fullName: "Alex Example",
        preferredTitle: null,
        primaryLocationPostcode: " sw1a 1aa ",
        linkedInUrl: null,
        githubUrl: null,
        portfolioUrl: null,
        summary: null,
        skillsText: null,
        experienceText: null,
        preferencesJson: " ");

    Assert.AreEqual("SW1A 1AA", profile.PrimaryLocationPostcode);
    Assert.AreEqual("{}", profile.PreferencesJson);
}
```

### Handler Functional Test Pattern

```csharp
[TestMethod]
public async Task Handle_WhenProfileExists_UpdatesExistingProfile()
{
    await using var database = new IdentitySqliteTestDatabase();
    await using var dbContext = database.CreateDbContext();

    var user = IdentityTestData.CreateUser();
    dbContext.Users.Add(user);
    await dbContext.SaveChangesAsync();

    var handler = new UpsertUserProfileCommandHandler(dbContext);

    var result = await handler.Handle(
        new UpsertUserProfileCommand(
            UserId: user.Id,
            FullName: "New Name",
            PreferredTitle: "Staff Engineer",
            PrimaryLocationPostcode: "ec1a 1bb",
            LinkedInUrl: null,
            GithubUrl: null,
            PortfolioUrl: null,
            Summary: "Updated",
            SkillsText: "Updated",
            ExperienceText: "Updated",
            PreferencesJson: "{\"remote\":true}"),
        CancellationToken.None);

    Assert.IsNotNull(result);
    Assert.IsFalse(result.Created);
}
```

### API Functional Test Pattern

```csharp
[TestMethod]
public async Task GetCurrent_WhenUnauthenticated_ReturnsUnauthorized()
{
    using var factory = new IdentityApiFactory();
    using var client = factory.CreateClient();

    var response = await client.GetAsync("/api/users/profile");

    Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
}
```

## What Not To Do

- Do not mock `DbContext`.
- Do not mock MediatR just to test endpoint behavior.
- Do not add tests for thin `Program.cs`.
- Do not create endpoint-only tests for business rules that belong to domain or application code.
- Do not introduce broad custom test frameworks when a small local factory or fixture will do.
- Do not claim TDD if the tests were added only after the implementation was already complete.

## Backend-Specific Mistakes To Avoid

### Wrong: Testing EF Queries With Fake Lists

Avoid replacing query behavior with LINQ-to-objects when the behavior depends on EF translation, ordering, or relational persistence.

### Right: Use A Small Relational Harness

Use the service-local SQLite harness pattern when persistence semantics matter.

### Wrong: Only Testing The Happy Path

Do not stop at the `200` or `201` path.
If the endpoint owns auth or validation behavior, cover the relevant `401`, `400`, `404`, or `409` outcomes too.

### Right: Match The Contract Surface

For API tests, assert the status code and the response contract fields that matter to callers.

## Success Criteria

Backend work is done when:

- the first test was written before the behavior change
- the test sits at the correct ownership layer
- the changed behavior is covered by focused tests
- relevant service or solution test commands pass
- the implementation still follows Firefly's backend structure

Tests are a delivery tool, not a formality. In this repo, good backend TDD means small red-green-refactor loops with tests placed exactly where the architecture says the behavior belongs.
