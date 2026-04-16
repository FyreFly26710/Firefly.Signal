# Testing Style

This document defines the backend testing style for Firefly Signal.

## Test Stack
- MSTest
- NSubstitute
- `WebApplicationFactory`
- container-backed functional test dependencies

## 1. Separate Unit And Functional Tests

Use:
- unit tests for domain and application behavior
- functional tests for HTTP API behavior

Recommended project names:
- `Firefly.Signal.JobSearch.UnitTests`
- `Firefly.Signal.JobSearch.FunctionalTests`
- `Firefly.Signal.Identity.FunctionalTests`

The repository does not currently include these test projects.
Until tests are added back, keep `Program.Testing.cs` and `InternalsVisibleTo` hooks minimal, and do not reference missing test projects from `Firefly.Signal.Api.slnx`.

## 2. Unit Test Style

Unit tests should be:
- direct
- short
- behavior-focused
- not dependent on real infrastructure unless needed

Example:
```csharp
[TestClass]
public class SearchRequestValidatorTests
{
    [TestMethod]
    public async Task Empty_keyword_is_invalid()
    {
        var validator = new SearchJobsRequestValidator();
        var result = await validator.ValidateAsync(new SearchJobsRequest("SW1A 1AA", ""));
        Assert.IsFalse(result.IsValid);
    }
}
```

Use NSubstitute for:
- repositories
- provider clients
- mediator or event publisher collaborators
- clock or environment abstractions when needed

## 3. Builder Pattern In Tests

When objects are non-trivial, prefer builders instead of repeating noisy setup.

Example:
```csharp
public sealed class JobRecordBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _title = "Backend Developer";
    private string _company = "Example Ltd";

    public JobRecordBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public JobRecord Build() => new(_id, _title, _company);
}
```

Keep builders in the test project only.

## 4. Functional Test Pattern

Each API should support `WebApplicationFactory<Program>`.

### `Program.Testing.cs`
```csharp
public partial class Program { }
```

### `.csproj`
```xml
<ItemGroup>
  <InternalsVisibleTo Include="Firefly.Signal.JobSearch.FunctionalTests" />
</ItemGroup>
```

### Fixture Responsibilities
Each fixture should:
- start required infrastructure
- override app configuration
- optionally replace auth
- expose an `HttpClient`

## 5. Replace Aspire With Testcontainers Or Compose

Do not use Aspire for functional test infrastructure.

Preferred option:
- Testcontainers for PostgreSQL, Redis, and RabbitMQ

Alternative:
- local Docker Compose-backed integration runs

Recommended direction:
- use Testcontainers in CI-friendly functional tests
- keep Compose available for local debugging and manual integration testing

## 6. Auth Override Pattern For Functional Tests

Keep a test-only auth bypass so protected endpoints can be tested without the real Google flow.

Example middleware:
```csharp
internal sealed class AutoAuthorizeMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        var identity = new ClaimsIdentity("test");
        identity.AddClaim(new Claim("sub", "test-user-id"));
        identity.AddClaim(new Claim(ClaimTypes.Name, "test-user-id"));

        context.User.AddIdentity(identity);
        await next(context);
    }
}
```

Or use a fake authentication scheme if that fits better.

## 7. Functional Test Scope

Functional tests should verify:
- routing
- status codes
- serialization
- auth behavior
- database integration
- event bus side effects where practical

Functional tests should not try to replace:
- pure domain tests
- every external provider contract test
- all end-to-end manual verification

## 8. Test Naming Guidance

Prefer names like:
- `Search_jobs_returns_ok_for_valid_input`
- `Get_job_returns_not_found_for_unknown_id`
- `Google_callback_rejects_missing_state`

Keep names behavior-oriented.

## 9. Recommended MSTest Project Shape

Example `.csproj`:
```xml
<Project Sdk="MSTest.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsPublishable>false</IsPublishable>
    <IsPackable>false</IsPackable>
    <OutputType>Exe</OutputType>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="NSubstitute" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\\..\\src\\Firefly.Signal.JobSearch.Api\\Firefly.Signal.JobSearch.Api.csproj" />
  </ItemGroup>
</Project>
```

## 10. Recommended Test Rules
- Keep unit tests fast.
- Keep functional tests realistic.
- Avoid giant base classes.
- Prefer builders over hidden magic setup.
- Keep auth override explicit.
- Keep container configuration localized to fixtures.
