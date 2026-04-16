---
name: backend-patterns
description: .NET 10 backend architecture patterns for Firefly Signal — eShop-aligned coding standards covering startup, CQRS, domain model, persistence, mapping, events, validation, and middleware.
---

# Backend Development Patterns (.NET 10 / eShop-Aligned)

Coding standards for all Firefly Signal backend services. The eShop reference codebase (`sources/eshop/src/`) is the source of truth for every pattern shown here.

## When to Activate

- Writing or reviewing any `.NET` API project
- Refactoring a fat `Program.cs` or moving DI inline code to extensions
- Adding a new command, query, or handler
- Modifying domain entities, value objects, or aggregates
- Writing or reviewing EF entity configurations or migrations
- Adding integration events or domain events
- Writing validators for commands
- Designing a new endpoint module or route group
- Reviewing mapping code (checking for AutoMapper, anonymous objects)
- Adding or modifying shared infrastructure projects

---

## 1. Solution & Folder Structure

### Per-Service Layout

Every API service follows this folder shape (derived from eShop `Ordering.API` and `Catalog.API`):

```
Firefly.Signal.JobSearch.Api/
  Apis/                              # Minimal API endpoint modules
  Models/                            # API-local transport models when needed
  Options/                           # Startup-bound options moved out of Program.cs
  Application/
    Commands/                        # IRequest<T> + IRequestHandler colocated
    Queries/                         # Interface + implementation + view models
    Behaviors/                       # MediatR pipeline behaviors
    DomainEventHandlers/             # INotificationHandler<TDomainEvent>
    IntegrationEvents/
      Events/                        # record XxxIntegrationEvent : IntegrationEvent
      EventHandling/                 # IIntegrationEventHandler<T> implementations
    Validations/                     # AbstractValidator<TCommand>
  Domain/
    JobPostingAggregate/
      JobPosting.cs                  # Entity : IAggregateRoot
      IJobPostingRepository.cs       # Repository interface lives with aggregate
  Infrastructure/
    EntityConfigurations/            # IEntityTypeConfiguration<T> — one per entity
    Repositories/                    # Repository implementations
    Services/                        # IdentityService, external provider adapters
    Persistence/
      JobSearchDbContext.cs          # DbContext implementing IUnitOfWork
      JobSearchDbContextSeed.cs
      JobSearchDbContextFactory.cs
      Migrations/
  Extensions/
    ApplicationServiceExtensions.cs  # AddApplicationServices composition root
  GlobalUsings.cs
  Program.cs                         # ≤ 25 lines
  Program.Testing.cs
```

### Shared Infrastructure Projects

```
services/api/src/
  Firefly.Signal.EventBus/
  Firefly.Signal.EventBusRabbitMQ/
  Firefly.Signal.IntegrationEventLogEF/
  Firefly.Signal.ServiceDefaults/
  Firefly.Signal.SharedKernel/       # Entity base, IAggregateRoot, IRepository, ValueObject
```

---

## 2. Program.cs — Thin Startup

**Rule**: `Program.cs` must be ≤ 25 lines. It calls extension methods only. No DI registrations, no auth config, no options binding, no class definitions.

```csharp
// PASS: eShop Ordering.API shape
var builder = WebApplication.CreateBuilder(args);

builder.AddFireflyServiceDefaults();
builder.AddApplicationServices();
builder.Services.AddProblemDetails();

var withApiVersioning = builder.Services.AddApiVersioning();
builder.AddDefaultOpenApi(withApiVersioning);

var app = builder.Build();

app.MapDefaultEndpoints();

var jobs = app.NewVersionedApi("Jobs");
jobs.MapJobSearchApiV1().RequireAuthorization();

app.UseDefaultOpenApi();
app.Run();
```

```csharp
// FAIL: fat Program.cs — DI inline, auth inline, class defined inside
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<JobSearchDbContext>(options =>       // FAIL
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("FireflySignalDb"));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)  // FAIL
    .AddJwtBearer(options => { ... });

var app = builder.Build();
app.Run();

internal sealed class JwtOptions { ... }  // FAIL — class in Program.cs
```

---

## 3. Composition Root — Extensions/ApplicationServiceExtensions.cs

**Rule**: All DI registrations live in `AddApplicationServices(this IHostApplicationBuilder builder)` inside `Extensions/ApplicationServiceExtensions.cs`. This is the single place to answer: what database, what auth, what event bus, what services does this API use?

```csharp
// PASS: Extensions/ApplicationServiceExtensions.cs
internal static class ApplicationServiceExtensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        // Auth
        builder.AddDefaultAuthentication();

        // Database
        services.AddDbContext<JobSearchDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("jobsearchdb"),
                npgsql => npgsql.MigrationsHistoryTable(
                    HistoryRepository.DefaultTableName, JobSearchDbContext.SchemaName));
        });

        // Migrations + seeding
        services.AddMigration<JobSearchDbContext, JobSearchDbContextSeed>();

        // Integration events (outbox)
        services.AddTransient<IIntegrationEventLogService,
            IntegrationEventLogService<JobSearchDbContext>>();
        services.AddTransient<IJobSearchIntegrationEventService,
            JobSearchIntegrationEventService>();

        // Event bus
        builder.AddRabbitMqEventBus("job-search-api")
               .AddEventBusSubscriptions();

        // MediatR + pipeline behaviors
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining(typeof(Program));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        // Validators
        services.AddValidatorsFromAssemblyContaining<CreateJobCommandValidator>();

        // Identity
        services.AddHttpContextAccessor();
        services.AddTransient<IIdentityService, IdentityService>();

        // Repositories
        services.AddScoped<IJobPostingRepository, JobPostingRepository>();

        // Queries (read side)
        services.AddScoped<IJobSearchQueries, JobSearchQueries>();

        // Options — bind here, never in Program.cs
        services.AddOptions<AdzunaOptions>()
                .BindConfiguration(nameof(AdzunaOptions));

        // External providers
        services.AddHttpClient<AdzunaJobSearchProvider>();
        services.AddScoped<IJobSearchProvider, AdzunaJobSearchProvider>();
    }

    private static void AddEventBusSubscriptions(this IEventBusBuilder eventBus)
    {
        eventBus.AddSubscription<JobCollectedIntegrationEvent,
            JobCollectedIntegrationEventHandler>();
    }
}
```

### Startup-Owned Types Live Outside Program.cs

If a type exists only because startup needs it, it still does **not** belong in `Program.cs`.

Move these into dedicated files:
- `Options/JwtOptions.cs`
- `Options/DownstreamOptions.cs`
- `Services/GatewayProxyClient.cs`
- `Services/GatewayDemoClient.cs`

`Program.cs` should not define:
- option records/classes
- enums
- HTTP clients
- helper methods
- local infrastructure types

---

## 3.1 Current User Access — Small Adapter, No Claims Parsing In Handlers

When a route needs the current authenticated user, use a tiny service that owns claim parsing.

```csharp
public interface ICurrentUserContext
{
    long? GetUserId();
}

internal sealed class HttpContextCurrentUserContext(IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    public long? GetUserId()
    {
        var user = httpContextAccessor.HttpContext?.User;
        var subject = user?.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? user?.FindFirstValue(ClaimTypes.NameIdentifier);

        return long.TryParse(subject, out var userId) ? userId : null;
    }
}
```

Register it in `AddApplicationServices`:

```csharp
services.AddHttpContextAccessor();
services.AddScoped<ICurrentUserContext, HttpContextCurrentUserContext>();
```

Handlers should call `currentUserContext.GetUserId()` and should **not** parse claims directly.

---

## 4. Minimal API Endpoint Modules — Apis/

**Rule**: One static class per feature area in `Apis/`. The class exposes a `MapXxxApiV1(this IEndpointRouteBuilder app)` extension method returning `RouteGroupBuilder`. Handler methods can be `private static async` when they are only route targets inside that module.

### Service Aggregator Class

Bundle common handler dependencies into a single `[AsParameters]` class instead of listing them on every handler:

```csharp
// Apis/JobSearchServices.cs
public class JobSearchServices(
    IMediator mediator,
    IJobSearchQueries queries,
    IIdentityService identityService,
    ILogger<JobSearchServices> logger)
{
    public IMediator Mediator { get; } = mediator;
    public IJobSearchQueries Queries { get; } = queries;
    public IIdentityService IdentityService { get; } = identityService;
    public ILogger<JobSearchServices> Logger { get; } = logger;
}
```

### Endpoint Module

```csharp
// PASS: Apis/JobSearchApi.cs
public static class JobSearchApi
{
    public static RouteGroupBuilder MapJobSearchApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/job-search").HasApiVersion(1.0);

        api.MapGet("/jobs", GetJobsPageAsync)
            .WithName("GetJobsPage")
            .WithSummary("Get paginated job list")
            .WithTags("Jobs");

        api.MapGet("/jobs/{id:long}", GetJobByIdAsync)
            .WithName("GetJobById")
            .WithSummary("Get job by ID")
            .WithTags("Jobs");

        api.MapPost("/jobs", CreateJobAsync)
            .WithName("CreateJob")
            .WithTags("Jobs");

        return api;
    }

    public static async Task<Ok<PagedResponse<JobSearchResultResponse>>> GetJobsPageAsync(
        [AsParameters] GetJobsPageQuery query,
        [AsParameters] JobSearchServices services,
        CancellationToken cancellationToken)
    {
        var userId = services.IdentityService.GetUserId();
        var result = await services.Queries.GetPageAsync(query, userId, cancellationToken);
        return TypedResults.Ok(result);
    }

    public static async Task<Results<Ok<JobDetailsResponse>, NotFound>> GetJobByIdAsync(
        long id,
        [AsParameters] JobSearchServices services,
        CancellationToken cancellationToken)
    {
        var result = await services.Mediator.Send(new GetJobByIdQuery(id), cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    public static async Task<Results<Created<JobDetailsResponse>, BadRequest<ProblemDetails>>> CreateJobAsync(
        CreateJobCommand command,
        [AsParameters] JobSearchServices services,
        CancellationToken cancellationToken)
    {
        var result = await services.Mediator.Send(command, cancellationToken);
        return TypedResults.Created($"/api/job-search/jobs/{result.Id}", result);
    }
}
```

### Status / Root Endpoint Modules

If the service exposes a root or status endpoint such as `/`, map it in a dedicated API module instead of inline in `Program.cs`.

```csharp
public static class ServiceStatusApi
{
    public static IEndpointRouteBuilder MapServiceStatusApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", GetServiceStatusAsync);
        return endpoints;
    }

    private static IResult GetServiceStatusAsync()
        => TypedResults.Ok(new
        {
            service = "job-search",
            message = "Firefly Signal job search API is running."
        });
}
```

```csharp
// FAIL: endpoint handlers inline in Program.cs
app.MapGet("/api/job-search/jobs", async (IJobSearchService service) =>  // FAIL
{
    var jobs = await service.GetAllAsync();
    return Results.Ok(jobs);
});

// FAIL: handlers in Endpoints/ folder instead of Apis/
namespace Firefly.Signal.JobSearch.Endpoints;  // FAIL — should be Apis/
```

---

## 5. Typed Results & ProblemDetails

**Rule**: All handler return types use `Results<T1, T2>` or a single concrete typed result. Never return raw `IResult`.

```csharp
// PASS: typed result combinations

// Single success
public static async Task<Ok<IEnumerable<JobDetailsResponse>>> GetAllAsync(...) { ... }

// Success or not found
public static async Task<Results<Ok<JobDetailsResponse>, NotFound>> GetByIdAsync(...) { ... }

// Success, bad request, problem
public static async Task<Results<Ok, BadRequest<string>, ProblemHttpResult>> CancelAsync(...) { ... }

// Created with location
public static async Task<Results<Created<JobDetailsResponse>, BadRequest<ProblemDetails>>> CreateAsync(...)
{
    return TypedResults.Created($"/api/job-search/jobs/{result.Id}", result);
}

// Server error via Problem
return TypedResults.Problem(detail: "Import failed.", statusCode: 500);

// Validation error
return TypedResults.BadRequest(new ProblemDetails
{
    Title = "Invalid request",
    Detail = "Postcode is required."
});
```

```csharp
// FAIL: raw IResult
public static async Task<IResult> GetByIdAsync(long id, ...)  // FAIL
{
    var job = await service.GetAsync(id);
    return job is null ? Results.NotFound() : Results.Ok(job);  // FAIL: Results.* not TypedResults.*
}

// FAIL: anonymous objects
return Results.Ok(new { service = "job-search", status = "ok" });  // FAIL
```

---

## 6. Request & Response Types

**Rule**: All request inputs are `sealed record` types. All response payloads are `sealed record` types. No anonymous objects in handlers.

### Commands (write side)

```csharp
// Application/Commands/CreateJobCommand.cs
public sealed record CreateJobCommand(
    string Title,
    string Company,
    string Postcode,
    string LocationName,
    string Summary,
    string Url,
    string SourceName,
    bool IsRemote,
    DateTime PostedAtUtc) : IRequest<JobDetailsResponse>;
```

### Queries (read side)

```csharp
// Application/Queries/GetJobsPageQuery.cs
public sealed record GetJobsPageQuery(
    int PageIndex = 0,
    int PageSize = 20,
    string? Keyword = null,
    string? Company = null,
    string? Postcode = null,
    bool? IsHidden = false);
```

### Response view models

```csharp
// Application/Queries/JobViewModels.cs
public sealed record JobDetailsResponse(
    long Id,
    string Title,
    string Company,
    string LocationName,
    bool IsRemote,
    DateTime PostedAtUtc);

public sealed record JobSearchResultResponse(
    long Id,
    string Title,
    string Company,
    string LocationName,
    bool IsRemote,
    bool IsSaved,
    bool IsUserHidden);
```

Placement rules:
- Command records live in `Application/Commands/` next to their handler
- Query records and view models live in `Application/Queries/`
- Shared pagination wrapper (`PagedResponse<T>`) lives in `SharedKernel/Models/`

---

## 7. CQRS with MediatR

### Command Pattern

```csharp
// Application/Commands/CreateJobCommand.cs
public sealed record CreateJobCommand(
    string Title,
    string Company,
    string Postcode,
    string Url,
    string SourceName,
    bool IsRemote,
    DateTime PostedAtUtc) : IRequest<JobDetailsResponse>;

public class CreateJobCommandHandler(
    IJobPostingRepository repository,
    IJobSearchIntegrationEventService eventService,
    ILogger<CreateJobCommandHandler> logger)
    : IRequestHandler<CreateJobCommand, JobDetailsResponse>
{
    public async Task<JobDetailsResponse> Handle(
        CreateJobCommand command,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Creating job {Title} from {SourceName}",
            command.Title, command.SourceName);

        var job = JobPosting.Create(
            command.Title, command.Company, command.Postcode,
            command.Url, command.SourceName, command.IsRemote, command.PostedAtUtc);

        repository.Add(job);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return JobDetailsResponse.FromEntity(job);
    }
}
```

### Query Pattern

```csharp
// Application/Queries/IJobSearchQueries.cs
public interface IJobSearchQueries
{
    Task<PagedResponse<JobSearchResultResponse>> GetPageAsync(
        GetJobsPageQuery query, long? userId, CancellationToken cancellationToken);

    Task<JobDetailsResponse?> GetByIdAsync(long id, CancellationToken cancellationToken);
}

// Application/Queries/JobSearchQueries.cs
public class JobSearchQueries(JobSearchDbContext context) : IJobSearchQueries
{
    public async Task<PagedResponse<JobSearchResultResponse>> GetPageAsync(
        GetJobsPageQuery query, long? userId, CancellationToken cancellationToken)
    {
        var root = context.Jobs.AsQueryable().Where(j => !j.IsHidden);

        if (!string.IsNullOrWhiteSpace(query.Keyword))
            root = root.Where(j => j.Title.Contains(query.Keyword));

        var totalCount = await root.LongCountAsync(cancellationToken);

        var items = await root
            .OrderByDescending(j => j.PostedAtUtc)
            .Skip(query.PageIndex * query.PageSize)
            .Take(query.PageSize)
            .Select(j => new JobSearchResultResponse(
                j.Id, j.Title, j.Company, j.LocationName, j.IsRemote, false, false))
            .ToListAsync(cancellationToken);

        return new PagedResponse<JobSearchResultResponse>(
            query.PageIndex, query.PageSize, totalCount, items);
    }
}
```

**Command vs Query rule**:
- Commands own all write operations. They go through MediatR → behaviors → handler → repository → `SaveEntitiesAsync`.
- Queries own all read operations. They go through MediatR → handler → `IJobSearchQueries` → direct EF LINQ → view model. Queries **never** use repositories and **never** return domain entities.

---

## 8. Pipeline Behaviors — Application/Behaviors/

Registration order is critical: **Logging → Validator → Transaction**.

```csharp
// Extensions/Extensions.cs
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining(typeof(Program));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
    cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
});
```

### LoggingBehavior

```csharp
// Application/Behaviors/LoggingBehavior.cs
public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Handling {CommandName} ({@Command})",
            typeof(TRequest).Name, request);

        var response = await next();

        logger.LogInformation(
            "Handled {CommandName} — response: {@Response}",
            typeof(TRequest).Name, response);

        return response;
    }
}
```

### ValidatorBehavior

```csharp
// Application/Behaviors/ValidatorBehavior.cs
public class ValidatorBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidatorBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Validating {CommandType}", typeof(TRequest).Name);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(request, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(e => e != null)
            .ToList();

        if (failures.Count != 0)
        {
            logger.LogWarning(
                "Validation errors for {CommandType}: {@Errors}",
                typeof(TRequest).Name, failures);

            throw new JobSearchDomainException(
                $"Validation failed for {typeof(TRequest).Name}",
                new ValidationException("Validation exception", failures));
        }

        return await next();
    }
}
```

### TransactionBehavior

```csharp
// Application/Behaviors/TransactionBehavior.cs
public class TransactionBehavior<TRequest, TResponse>(
    JobSearchDbContext dbContext,
    IJobSearchIntegrationEventService eventService,
    ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (dbContext.HasActiveTransaction)
            return await next();

        var response = default(TResponse);
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.BeginTransactionAsync();

            logger.LogInformation(
                "Begin transaction {TransactionId} for {CommandName}",
                transaction.TransactionId, typeof(TRequest).Name);

            response = await next();

            await dbContext.CommitTransactionAsync(transaction);

            await eventService.PublishEventsThroughEventBusAsync(transaction.TransactionId);
        });

        return response!;
    }
}
```

---

## 9. FluentValidation — Application/Validations/

```csharp
// Application/Validations/CreateJobCommandValidator.cs
public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().MaximumLength(300);
        RuleFor(c => c.Company).NotEmpty().MaximumLength(160);
        RuleFor(c => c.Postcode).NotEmpty().MaximumLength(16);
        RuleFor(c => c.Url).NotEmpty().Must(u => Uri.TryCreate(u, UriKind.Absolute, out _))
            .WithMessage("Url must be a valid absolute URI.");
        RuleFor(c => c.PostedAtUtc).LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("PostedAtUtc cannot be in the future.");
    }
}
```

Registration (in `Extensions.cs`):
```csharp
services.AddValidatorsFromAssemblyContaining<CreateJobCommandValidator>();
```

Rule: one validator per command. Validators live in `Application/Validations/`. The `ValidatorBehavior` runs them automatically for every command that has a registered validator.

---

## 10. Domain Layer

### SharedKernel Base Classes

```csharp
// SharedKernel/Domain/Entity.cs
public abstract class Entity
{
    public long Id { get; protected set; } = SnowflakeId.GenerateId();

    private List<INotification>? _domainEvents;
    public IReadOnlyCollection<INotification> DomainEvents =>
        _domainEvents?.AsReadOnly() ?? ArraySegment<INotification>.Empty;

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents ??= [];
        _domainEvents.Add(eventItem);
    }

    public void ClearDomainEvents() => _domainEvents?.Clear();
}

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; private set; } = DateTime.UtcNow;
    public bool IsDeleted { get; private set; }

    public void Touch() => UpdatedAtUtc = DateTime.UtcNow;

    public virtual void MarkDeleted()
    {
        IsDeleted = true;
        Touch();
    }
}

// SharedKernel/IAggregateRoot.cs
public interface IAggregateRoot;

// SharedKernel/IRepository.cs
public interface IRepository<T> where T : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}

// SharedKernel/IUnitOfWork.cs
public interface IUnitOfWork
{
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}
```

### ValueObject Base

```csharp
// SharedKernel/Domain/ValueObject.cs
public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType()) return false;
        return GetEqualityComponents()
            .SequenceEqual(((ValueObject)obj).GetEqualityComponents());
    }

    public override int GetHashCode() =>
        GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
}
```

### Aggregate Root

```csharp
// Domain/JobPostingAggregate/JobPosting.cs
public sealed class JobPosting : AuditableEntity, IAggregateRoot
{
    private JobPosting() { }   // Required by EF

    public string Title { get; private set; } = string.Empty;
    public string Company { get; private set; } = string.Empty;
    public string Postcode { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public bool IsHidden { get; private set; }

    public static JobPosting Create(
        string title,
        string company,
        string postcode,
        string url,
        string sourceName,
        bool isRemote,
        DateTime postedAtUtc)
    {
        var job = new JobPosting
        {
            Title = title.Trim(),
            Company = company.Trim(),
            Postcode = postcode.Trim().ToUpperInvariant(),
            Url = url.Trim()
        };

        job.AddDomainEvent(new JobPostingCreatedDomainEvent(job));

        return job;
    }

    public void Hide()
    {
        IsHidden = true;
        Touch();
        AddDomainEvent(new JobPostingHiddenDomainEvent(this));
    }
}
```

Repository interface lives next to the aggregate:
```csharp
// Domain/JobPostingAggregate/IJobPostingRepository.cs
public interface IJobPostingRepository : IRepository<JobPosting>
{
    void Add(JobPosting job);
    Task<JobPosting?> GetAsync(long id, CancellationToken cancellationToken);
    void Update(JobPosting job);
}
```

Domain exceptions:
```csharp
// Domain/Exceptions/JobSearchDomainException.cs
public class JobSearchDomainException(string message, Exception? innerException = null)
    : Exception(message, innerException);
```

---

## 11. DbContext / IUnitOfWork / Entity Configurations

### DbContext

```csharp
// Infrastructure/Persistence/JobSearchDbContext.cs
public sealed class JobSearchDbContext : DbContext, IUnitOfWork
{
    public const string SchemaName = "jobsearch";

    private readonly IMediator _mediator;
    private IDbContextTransaction? _currentTransaction;

    public JobSearchDbContext(DbContextOptions<JobSearchDbContext> options, IMediator mediator)
        : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<JobPosting> Jobs => Set<JobPosting>();
    public DbSet<JobRefreshRun> JobRefreshRuns => Set<JobRefreshRun>();

    public bool HasActiveTransaction => _currentTransaction != null;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        // PASS: delegate all config to IEntityTypeConfiguration<T> classes
        modelBuilder.ApplyConfiguration(new JobPostingEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new JobRefreshRunEntityTypeConfiguration());

        modelBuilder.UseIntegrationEventLogs();
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch domain events BEFORE saving — keeps side effects in the same transaction
        await _mediator.DispatchDomainEventsAsync(this);
        await base.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        _currentTransaction ??= await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        try
        {
            await SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public void RollbackTransaction()
    {
        _currentTransaction?.Rollback();
        _currentTransaction?.Dispose();
        _currentTransaction = null;
    }
}
```

### IEntityTypeConfiguration — One File Per Entity

```csharp
// PASS: Infrastructure/EntityConfigurations/JobPostingEntityTypeConfiguration.cs
sealed class JobPostingEntityTypeConfiguration : IEntityTypeConfiguration<JobPosting>
{
    public void Configure(EntityTypeBuilder<JobPosting> builder)
    {
        builder.ToTable("jobs");
        builder.HasKey(j => j.Id);
        builder.Property(j => j.Id).ValueGeneratedNever();

        builder.Property(j => j.Title).HasMaxLength(300).IsRequired();
        builder.Property(j => j.Company).HasMaxLength(160).IsRequired();
        builder.Property(j => j.Postcode).HasMaxLength(16).IsRequired();
        builder.Property(j => j.Url).HasMaxLength(1024).IsRequired();

        builder.Property(j => j.CreatedAtUtc).IsRequired();
        builder.Property(j => j.UpdatedAtUtc).IsRequired();

        builder.Ignore(j => j.DomainEvents);

        builder.HasIndex(j => j.Postcode);
        builder.HasIndex(j => j.IsHidden);
    }
}
```

```csharp
// FAIL: inline entity config in OnModelCreating
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<JobPosting>(entity =>  // FAIL — inline config
    {
        entity.ToTable("jobs");
        entity.Property(x => x.Title).HasMaxLength(300);
        // ... 50 more lines
    });
}
```

### Domain Event Dispatching Helper

```csharp
// Infrastructure/MediatorExtension.cs
internal static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(
        this IMediator mediator, JobSearchDbContext ctx)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(e => e.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}
```

---

## 12. Repository Pattern

```csharp
// Infrastructure/Repositories/JobPostingRepository.cs
public class JobPostingRepository(JobSearchDbContext context) : IJobPostingRepository
{
    public IUnitOfWork UnitOfWork => context;

    public void Add(JobPosting job) => context.Jobs.Add(job);

    public async Task<JobPosting?> GetAsync(long id, CancellationToken cancellationToken)
    {
        return await context.Jobs.FindAsync([id], cancellationToken);
    }

    public void Update(JobPosting job)
    {
        context.Entry(job).State = EntityState.Modified;
    }
}
```

Rules:
- Repository wraps exactly one aggregate root
- Write-side handlers use repositories; they **never** use `DbContext` directly
- Read-side queries (`IJobSearchQueries`) bypass the repository and use `DbContext` directly — this is intentional (CQRS read/write split)
- Never call `context.SaveChangesAsync()` inside a repository method — that is the caller's responsibility via `UnitOfWork.SaveEntitiesAsync()`

---

## 13. Mapping — Explicit, No AutoMapper

Three acceptable patterns in priority order:

### 1. EF Projection (preferred for queries)

```csharp
// Inside JobSearchQueries — EF translates this to SQL SELECT
var items = await root
    .Select(j => new JobSearchResultResponse(
        j.Id, j.Title, j.Company, j.LocationName, j.IsRemote, false, false))
    .ToListAsync(cancellationToken);
```

### 2. Static Factory Method (for command responses)

```csharp
// Application/Queries/JobDetailsResponse.cs
public sealed record JobDetailsResponse(long Id, string Title, string Company)
{
    public static JobDetailsResponse FromEntity(JobPosting job) =>
        new(job.Id, job.Title, job.Company);
}

// Usage in handler
return JobDetailsResponse.FromEntity(job);
```

### 3. Extension Method (for shared/cross-cutting mapping)

```csharp
// Extensions/JobPostingMappingExtensions.cs
public static class JobPostingMappingExtensions
{
    public static JobDetailsResponse ToResponse(this JobPosting job) =>
        new(job.Id, job.Title, job.Company);
}

// Usage
return job.ToResponse();
```

```csharp
// FAIL: AutoMapper
services.AddAutoMapper(typeof(Program));  // FAIL — no AutoMapper in this repo

// FAIL: anonymous object response
return TypedResults.Ok(new { id = job.Id, title = job.Title });  // FAIL

// FAIL: unnamed inline projection in handler
var result = await context.Jobs
    .Select(j => new { j.Id, j.Title })  // FAIL — use a named record
    .ToListAsync();
```

---

## 14. Integration Events

Integration events cross service boundaries and use the outbox pattern.

### Event Definition

```csharp
// Application/IntegrationEvents/Events/JobImportedIntegrationEvent.cs
public record JobImportedIntegrationEvent(long JobId, string SourceName) : IntegrationEvent;
```

### Event Service Interface

```csharp
// Application/IntegrationEvents/IJobSearchIntegrationEventService.cs
public interface IJobSearchIntegrationEventService
{
    Task AddAndSaveEventAsync(IntegrationEvent evt);
    Task PublishEventsThroughEventBusAsync(Guid transactionId);
}
```

### Integration Event Handler

```csharp
// Application/IntegrationEvents/EventHandling/JobCollectedIntegrationEventHandler.cs
public class JobCollectedIntegrationEventHandler(
    IMediator mediator,
    ILogger<JobCollectedIntegrationEventHandler> logger)
    : IIntegrationEventHandler<JobCollectedIntegrationEvent>
{
    public async Task Handle(JobCollectedIntegrationEvent @event)
    {
        logger.LogInformation(
            "Handling {EventName} {EventId}",
            nameof(JobCollectedIntegrationEvent), @event.Id);

        var command = new ImportCollectedJobCommand(@event.JobId, @event.SourceName);
        await mediator.Send(command);
    }
}
```

### Registration

```csharp
// Extensions/Extensions.cs — private helper
private static void AddEventBusSubscriptions(this IEventBusBuilder eventBus)
{
    eventBus.AddSubscription<JobCollectedIntegrationEvent,
        JobCollectedIntegrationEventHandler>();
}
```

Rules:
- Integration event records inherit from `IntegrationEvent` (base record in `EventBus` project)
- Published via `IJobSearchIntegrationEventService.AddAndSaveEventAsync()` inside command handlers
- Published to the bus via `TransactionBehavior` after successful commit
- Consumed via subscriptions registered in `AddEventBusSubscriptions()`
- Do **not** publish domain events to the event bus

---

## 15. Domain Events

Domain events coordinate within the same bounded context. They are dispatched by `SaveEntitiesAsync` before the DB write.

### Event Definition

```csharp
// Domain/Events/JobPostingCreatedDomainEvent.cs
public record JobPostingCreatedDomainEvent(JobPosting Job) : INotification;

// Domain/Events/JobPostingHiddenDomainEvent.cs
public record JobPostingHiddenDomainEvent(JobPosting Job) : INotification;
```

### Domain Event Handler

```csharp
// Application/DomainEventHandlers/PublishIntegrationEventWhenJobCreatedDomainEventHandler.cs
public class PublishIntegrationEventWhenJobCreatedDomainEventHandler(
    IJobSearchIntegrationEventService eventService,
    ILogger<PublishIntegrationEventWhenJobCreatedDomainEventHandler> logger)
    : INotificationHandler<JobPostingCreatedDomainEvent>
{
    public async Task Handle(
        JobPostingCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Job {JobId} created — queuing integration event",
            domainEvent.Job.Id);

        var integrationEvent = new JobImportedIntegrationEvent(
            domainEvent.Job.Id,
            domainEvent.Job.SourceName);

        await eventService.AddAndSaveEventAsync(integrationEvent);
    }
}
```

Rules:
- Domain events are `record` types implementing `INotification`, placed in `Domain/Events/`
- Raised inside aggregate methods via `AddDomainEvent(new XxxDomainEvent(...))`
- Dispatched automatically by `SaveEntitiesAsync()` → `_mediator.DispatchDomainEventsAsync(this)`
- Handlers in `Application/DomainEventHandlers/` — one handler per event
- Domain event handlers may enqueue integration events; they do **not** publish directly to the bus

---

## 16. GlobalUsings.cs

One file per project. Only namespaces used across 3+ files. Organised by group, alphabetical within groups.

```csharp
// PASS: JobSearch.Api/GlobalUsings.cs
global using FluentValidation;
global using MediatR;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;

global using Firefly.Signal.EventBus;
global using Firefly.Signal.EventBusRabbitMQ;
global using Firefly.Signal.IntegrationEventLogEF;
global using Firefly.Signal.ServiceDefaults;
global using Firefly.Signal.SharedKernel.Domain;
global using Firefly.Signal.SharedKernel.Models;

global using Firefly.Signal.JobSearch.Application.Behaviors;
global using Firefly.Signal.JobSearch.Application.Commands;
global using Firefly.Signal.JobSearch.Application.IntegrationEvents;
global using Firefly.Signal.JobSearch.Application.IntegrationEvents.EventHandling;
global using Firefly.Signal.JobSearch.Application.IntegrationEvents.Events;
global using Firefly.Signal.JobSearch.Application.Queries;
global using Firefly.Signal.JobSearch.Application.Validations;
global using Firefly.Signal.JobSearch.Domain.JobPostingAggregate;
global using Firefly.Signal.JobSearch.Domain.Events;
global using Firefly.Signal.JobSearch.Extensions;
global using Firefly.Signal.JobSearch.Infrastructure;
global using Firefly.Signal.JobSearch.Infrastructure.Services;
```

---

## 17. Identity Service Adapter

Never parse claims directly in endpoint handlers. Use a thin adapter registered via DI.

```csharp
// Infrastructure/Services/IIdentityService.cs
public interface IIdentityService
{
    long? GetUserId();
    string? GetUserName();
    bool IsAdmin();
}

// Infrastructure/Services/IdentityService.cs
public sealed class IdentityService(IHttpContextAccessor accessor) : IIdentityService
{
    public long? GetUserId()
    {
        var value = accessor.HttpContext?.User.FindFirst("sub")?.Value;
        return long.TryParse(value, out var id) ? id : null;
    }

    public string? GetUserName() =>
        accessor.HttpContext?.User.Identity?.Name;

    public bool IsAdmin() =>
        accessor.HttpContext?.User.IsInRole("admin") ?? false;
}
```

Registration (in `Extensions.cs`):
```csharp
services.AddHttpContextAccessor();
services.AddTransient<IIdentityService, IdentityService>();
```

Usage (via service aggregator in endpoint handler):
```csharp
var userId = services.IdentityService.GetUserId();
```

---

## 18. Build Conventions

### Directory.Build.props

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

### Directory.Packages.props (no versions in .csproj)

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <!-- ASP.NET Core -->
    <PackageVersion Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.0.0" />
    <PackageVersion Include="Microsoft.AspNetCore.OpenApi" Version="10.0.0" />
    <!-- EF Core + Npgsql -->
    <PackageVersion Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.1" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0" />
    <!-- API Versioning -->
    <PackageVersion Include="Asp.Versioning.Http" Version="8.1.0" />
    <!-- CQRS + Validation -->
    <PackageVersion Include="MediatR" Version="12.4.0" />
    <PackageVersion Include="FluentValidation" Version="11.9.0" />
    <PackageVersion Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.0" />
    <!-- Testing -->
    <PackageVersion Include="MSTest" Version="3.6.0" />
    <PackageVersion Include="NSubstitute" Version="5.3.0" />
  </ItemGroup>
</Project>
```

### .csproj — No Version Attributes

```xml
<ItemGroup>
  <PackageReference Include="MediatR" />                 <!-- version from Directory.Packages.props -->
  <PackageReference Include="FluentValidation" />
  <PackageReference Include="Asp.Versioning.Http" />
  <InternalsVisibleTo Include="Firefly.Signal.JobSearch.FunctionalTests" />
</ItemGroup>
```

---
> Deprecated reference. This version predates `draft.md` and the active skill in `.agents/skills/backend-patterns/SKILL.md`.
> Use `draft.md` and `backend-patterns/SKILL.md` as the current source of truth.
