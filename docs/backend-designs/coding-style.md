# Backend Coding Style

This document defines the backend coding style for Firefly Signal APIs.
It captures the patterns we want to keep and turns them into standalone conventions.

## Primary Style Rules
- Keep `Program.cs` small.
- Register dependencies in extension methods, not inline in startup.
- Use minimal APIs with explicit route groups.
- Use typed results and `ProblemDetails`.
- Keep cross-cutting setup obvious.
- Prefer explicit boundaries over magic abstractions.
- Favor vertical slices inside a service.

## 1. Keep `Program.cs` Thin

Each API should have a startup file that is easy to understand in under one minute.

Preferred shape:
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddFireflyServiceDefaults();
builder.AddApplicationServices();
builder.Services.AddProblemDetails();

var versioning = builder.Services.AddApiVersioning();
builder.AddDefaultOpenApi(versioning);

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapJobSearchApi();

app.UseDefaultOpenApi();
app.Run();
```

Keep `Program.cs` responsible only for:
- building the host
- calling extension methods
- mapping top-level endpoints
- running the app

Do not put these directly into `Program.cs` unless the app is extremely small:
- large DI registration blocks
- endpoint handler bodies
- database setup logic
- event bus wiring
- auth configuration details

## 2. Use Extension-Based Composition Roots

Each API should have a composition root such as:
- `Extensions/ApplicationServiceExtensions.cs`
- `Extensions/AuthenticationExtensions.cs`
- `Extensions/OpenApiExtensions.cs`

Recommended pattern:
```csharp
namespace Firefly.Signal.JobSearch.Api.Extensions;

internal static class ApplicationServiceExtensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddDbContext<JobSearchContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("jobsearchdb"));
        });

        services.AddMigration<JobSearchContext, JobSearchContextSeeder>();

        services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService<JobSearchContext>>();
        services.AddTransient<IJobSearchIntegrationEventService, JobSearchIntegrationEventService>();

        builder.AddRabbitMqEventBus("eventbus")
               .AddSubscription<JobCollectedIntegrationEvent, JobCollectedIntegrationEventHandler>();

        services.AddScoped<IJobProviderClient, ReedJobProviderClient>();
        services.AddScoped<ISearchService, SearchService>();
    }
}
```

This file should answer:
- what data store does the service use?
- does it auto-migrate?
- does it publish or consume events?
- what feature services are registered?
- what auth or identity adapters are involved?

## 3. Organize Minimal APIs Into Static Endpoint Modules

Use one mapping module per feature area.

Example:
```csharp
namespace Firefly.Signal.JobSearch.Api.Apis;

public static class JobSearchApi
{
    public static RouteGroupBuilder MapJobSearchApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/job-search")
            .WithTags("Job Search")
            .HasApiVersion(1.0);

        api.MapGet("/", SearchJobsAsync);
        api.MapGet("/{id:guid}", GetJobAsync);

        return api;
    }

    public static async Task<Results<Ok<SearchJobsResponse>, BadRequest<ProblemDetails>>> SearchJobsAsync(
        [AsParameters] SearchJobsRequest request,
        ISearchService service,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Postcode))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid postcode",
                Detail = "A postcode is required."
            });
        }

        var result = await service.SearchAsync(request, cancellationToken);
        return TypedResults.Ok(result);
    }
}
```

Benefits:
- route definitions stay discoverable
- handler signatures stay explicit
- tests can target endpoint behavior clearly
- large features can later split by request/response/handler without a full controller migration

## 4. Use Typed Results And `ProblemDetails`

Prefer:
- `Results<...>`
- `Ok<T>`
- `Created<T>`
- `NoContent`
- `NotFound`
- `BadRequest<ProblemDetails>`

Avoid returning raw `IResult` everywhere unless a handler truly needs that flexibility.

Example:
```csharp
public static async Task<Results<Ok<JobDetailsResponse>, NotFound>> GetJobAsync(
    Guid id,
    IJobReadService service,
    CancellationToken cancellationToken)
{
    var job = await service.GetAsync(id, cancellationToken);
    return job is null ? TypedResults.NotFound() : TypedResults.Ok(job);
}
```

Validation guidance:
- return early for invalid transport-level input
- throw domain or application exceptions only for real business failures
- map unexpected failures to `ProblemDetails` centrally

## 5. Keep Requests And Responses Explicit

Do not use anonymous object payloads in production handlers.

Prefer:
- request records
- response records
- feature-local DTOs

Example:
```csharp
public sealed record SearchJobsRequest(
    string Postcode,
    string Keyword,
    int PageIndex = 0,
    int PageSize = 20);

public sealed record SearchJobsResponse(
    int PageIndex,
    int PageSize,
    long TotalCount,
    IReadOnlyList<JobCardDto> Jobs);
```

Keep DTOs close to the feature that owns them.

## 6. Keep `GlobalUsings.cs` Small And Intentional

Use one `GlobalUsings.cs` per project, but only for namespaces that are truly common across most files.

Good candidates:
- `Microsoft.EntityFrameworkCore`
- `Microsoft.AspNetCore.Mvc`
- versioning namespaces
- shared event bus namespaces
- service-local domain namespaces

Do not use `GlobalUsings.cs` as a dumping ground for every namespace in the project.

## 7. Centralize Build Conventions

Use:
- `Directory.Build.props`
- `Directory.Build.targets`
- `Directory.Packages.props`

Recommended `Directory.Build.props` baseline:
```xml
<Project>
  <PropertyGroup>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <DebugType>embedded</DebugType>
    <DebugSymbols>true</DebugSymbols>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
</Project>
```

Recommended `Directory.Packages.props` categories:
- ASP.NET Core packages
- EF Core and Npgsql
- OpenTelemetry
- RabbitMQ packages
- testing packages
- FluentValidation if used
- MediatR if used

## 8. Share Only The Right Things

There are two good kinds of reuse:

1. Tiny shared source files
- `MigrateDbContextExtensions.cs`
- `ActivityExtensions.cs`

2. Small shared libraries
- `Firefly.Signal.EventBus`
- `Firefly.Signal.EventBusRabbitMQ`
- `Firefly.Signal.IntegrationEventLogEF`

Do not create broad shared utility libraries just because multiple services exist.

## 9. Use Simple Identity Adapters In APIs

Each service should access identity through a small interface, not by parsing claims all over the codebase.

Example:
```csharp
public interface IIdentityContext
{
    string? GetUserId();
    string? GetUserName();
}

public sealed class IdentityContext(IHttpContextAccessor accessor) : IIdentityContext
{
    public string? GetUserId() => accessor.HttpContext?.User.FindFirst("sub")?.Value;
    public string? GetUserName() => accessor.HttpContext?.User.Identity?.Name;
}
```

This is especially important once the identity API is custom.

## 10. Use Pipeline Behaviors Only Where They Help

Command-driven services may benefit from:
- logging behavior
- validation behavior
- transaction behavior

Example registration:
```csharp
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining(typeof(Program));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
    cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
});
```

Use this for services with real command orchestration.
Do not force MediatR into the gateway or very small read-heavy services.

## 11. Keep Logging Structured

Prefer:
```csharp
logger.LogInformation(
    "Searching jobs for postcode {Postcode} and keyword {Keyword}",
    request.Postcode,
    request.Keyword);
```

Avoid:
- string interpolation inside logs when structured logging fits
- logging sensitive tokens
- logging raw OAuth payloads

## 12. Keep Functional-Test Hooks Built In

Each API should support `WebApplicationFactory` cleanly.

Pattern:
- add `Program.Testing.cs`
- add `InternalsVisibleTo` to the functional test project

`Program.Testing.cs`:
```csharp
public partial class Program { }
```

`.csproj` snippet:
```xml
<ItemGroup>
  <InternalsVisibleTo Include="Firefly.Signal.JobSearch.FunctionalTests" />
</ItemGroup>
```

## 13. Recommended Folder Shape Inside An API

```text
Firefly.Signal.JobSearch.Api/
  Apis/
  Contracts/
    Requests/
    Responses/
  Application/
    Commands/
    Queries/
    Mappers/
  Extensions/
  Infrastructure/
  Options/
  Program.cs
  Program.Testing.cs
  GlobalUsings.cs
```

Notes:
- `Apis/` owns route modules and request-to-application translation.
- `Contracts/` owns transport models.
- `Application/` owns read/write interfaces plus mapping that should not live in the HTTP layer.
- `Infrastructure/` owns EF Core, storage, provider adapters, and service implementations.
- Keep `Program.Testing.cs` even if no test project currently exists so functional tests can be reintroduced cleanly later.

If the service grows, split feature logic further. If it stays small, keep it simple.

## 14. Backend Style Summary
- thin startup
- explicit extension-based DI
- static minimal API modules
- typed request and response models
- typed results with `ProblemDetails`
- small global usings
- central build configuration
- thin identity adapters
- test hooks built in
- shared infrastructure only where it clearly earns its place
