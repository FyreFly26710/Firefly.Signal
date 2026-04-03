# Messaging, Migrations, And Outbox

This document defines the backend infrastructure style for database startup, seeding, integration events, and RabbitMQ usage.
The current repo uses a simplified eShop-style event bus: explicit subscriptions, one durable direct exchange, and one durable queue per consuming API.

## 1. Shared Migration Helper

Keep a shared migration helper patterned like this:

```csharp
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.AspNetCore.Hosting;

internal static class MigrateDbContextExtensions
{
    private static readonly string ActivitySourceName = "DbMigrations";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    public static IServiceCollection AddMigration<TContext>(this IServiceCollection services)
        where TContext : DbContext
        => services.AddMigration<TContext>((_, _) => Task.CompletedTask);

    public static IServiceCollection AddMigration<TContext>(
        this IServiceCollection services,
        Func<TContext, IServiceProvider, Task> seeder)
        where TContext : DbContext
    {
        services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource(ActivitySourceName));
        return services.AddHostedService(sp => new MigrationHostedService<TContext>(sp, seeder));
    }

    public static IServiceCollection AddMigration<TContext, TDbSeeder>(this IServiceCollection services)
        where TContext : DbContext
        where TDbSeeder : class, IDbSeeder<TContext>
    {
        services.AddScoped<IDbSeeder<TContext>, TDbSeeder>();
        return services.AddMigration<TContext>((context, sp) =>
            sp.GetRequiredService<IDbSeeder<TContext>>().SeedAsync(context));
    }

    private static async Task MigrateDbContextAsync<TContext>(
        this IServiceProvider services,
        Func<TContext, IServiceProvider, Task> seeder)
        where TContext : DbContext
    {
        using var scope = services.CreateScope();
        var scopeServices = scope.ServiceProvider;
        var logger = scopeServices.GetRequiredService<ILogger<TContext>>();
        var context = scopeServices.GetRequiredService<TContext>();

        using var activity = ActivitySource.StartActivity($"Migration operation {typeof(TContext).Name}");

        try
        {
            logger.LogInformation("Migrating database for context {DbContextName}", typeof(TContext).Name);

            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await context.Database.MigrateAsync();
                await seeder(context, scopeServices);
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database migration failed for context {DbContextName}", typeof(TContext).Name);
            throw;
        }
    }

    private sealed class MigrationHostedService<TContext>(
        IServiceProvider services,
        Func<TContext, IServiceProvider, Task> seeder) : BackgroundService
        where TContext : DbContext
    {
        public override Task StartAsync(CancellationToken cancellationToken) =>
            services.MigrateDbContextAsync(seeder);

        protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
            Task.CompletedTask;
    }
}

public interface IDbSeeder<in TContext> where TContext : DbContext
{
    Task SeedAsync(TContext context);
}
```

## 2. Migration Rules

- Each `DbContext` owns its own migrations.
- Keep migration files in the owning API project because the current repo keeps application, domain, and infrastructure inside each API project.
- Use startup comments in the `DbContext` file to document the migration command.
- Keep seeders idempotent.

Example comment:

```csharp
/// <remarks>
/// dotnet ef migrations add --startup-project ../Firefly.Signal.JobSearch.Api InitialCreate
/// </remarks>
```

## 3. Seeder Rules

Use one seeder per `DbContext`.
Seed only when the target table set is empty.

Example:

```csharp
public sealed class OrderingContextSeed : IDbSeeder<OrderingContext>
{
    public async Task SeedAsync(OrderingContext context)
    {
        if (!context.CardTypes.Any())
        {
            context.CardTypes.AddRange(GetPredefinedCardTypes());

            await context.SaveChangesAsync();
        }

        await context.SaveChangesAsync();
    }

    private static IEnumerable<CardType> GetPredefinedCardTypes()
    {
        yield return new CardType { Id = 1, Name = "Amex" };
        yield return new CardType { Id = 2, Name = "Visa" };
        yield return new CardType { Id = 3, Name = "MasterCard" };
    }
}
```

Do not build a generic mega-seeding system.

## 4. DbContext Conventions

### Simple Context

Use a straightforward context for simple services:

```csharp
public sealed class JobSearchDbContext(DbContextOptions<JobSearchDbContext> options) : DbContext(options)
{
    public const string SchemaName = "jobsearch";

    public DbSet<JobPosting> Jobs => Set<JobPosting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(JobSearchDbContext).Assembly);
    }
}
```

### Domain-Heavy Context

If a service grows into richer aggregates and commands, it can later own:
- explicit transaction methods
- domain event dispatch
- unit-of-work style save methods

Do not apply this heavier pattern to every service by default.

## 5. Event Bus Abstractions

Keep messaging contracts in a dedicated shared project.

### `IntegrationEvent`

```csharp
public record IntegrationEvent
{
    public IntegrationEvent()
    {
        Id = SnowflakeId.GenerateId();
        CreationDateUtc = DateTime.UtcNow;
    }

    [JsonInclude]
    public long Id { get; init; }

    [JsonInclude]
    public DateTime CreationDateUtc { get; init; }
}
```

### `IEventBus`

```csharp
public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent;
}
```

### `IIntegrationEventHandler<T>`

```csharp
public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : IntegrationEvent
{
    Task HandleAsync(TIntegrationEvent @event, CancellationToken cancellationToken = default);

    Task IIntegrationEventHandler.HandleAsync(IntegrationEvent @event, CancellationToken cancellationToken)
        => HandleAsync((TIntegrationEvent)@event, cancellationToken);
}

public interface IIntegrationEventHandler
{
    Task HandleAsync(IntegrationEvent @event, CancellationToken cancellationToken = default);
}
```

### Subscription Metadata

Keep explicit event-type registration so the runtime never depends on `Type.GetType` for broker payload routing.
The repo stores this metadata in `EventBusSubscriptionInfo`, which maps routing keys to concrete event types and carries shared JSON serializer options.

## 6. RabbitMQ Registration Style

Preferred registration shape:

```csharp
builder.AddRabbitMqEventBus("ai-api")
    .AddSubscription<JobSearchRequestedIntegrationEvent, JobSearchRequestedIntegrationEventHandler>();
```

This should:
- bind `RabbitMq` and `EventBus` configuration
- register the repo-owned `IEventBus`
- register the RabbitMQ consumer as a hosted service
- create one durable queue per consuming API
- bind that queue to a shared durable direct exchange for each subscribed event name

## 7. Event Bus Configuration

Use config like:

```json
{
  "RabbitMq": {
    "Host": "rabbitmq",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "VirtualHost": "/"
  },
  "EventBus": {
    "SubscriptionClientName": "ai-api",
    "ExchangeName": "firefly_signal_event_bus",
    "RetryCount": 5
  }
}
```

Recommended option type:

```csharp
public sealed class EventBusOptions
{
    public string SubscriptionClientName { get; set; } = string.Empty;
    public string ExchangeName { get; set; } = "firefly_signal_event_bus";
    public int RetryCount { get; set; } = 5;
}
```

Recommended queue and exchange model:
- exchange: one durable direct exchange for the repo, currently `firefly_signal_event_bus`
- routing key: event CLR type name, for example `JobSearchRequestedIntegrationEvent`
- queue: one durable queue per consuming API, for example `ai-api`
- subscription: queue binding declared from the consuming API at startup

Recommended usage rule:
- HTTP between APIs for synchronous request-response work
- RabbitMQ for notifications, downstream enrichment, and other asynchronous follow-up work

## 8. Integration Event Log / Outbox

Keep a shared project for the integration event log.

### Model Builder Extension

```csharp
public static class IntegrationLogExtensions
{
    public static void UseIntegrationEventLogs(this ModelBuilder builder)
    {
        builder.Entity<IntegrationEventLogEntry>(builder =>
        {
            builder.ToTable("IntegrationEventLog");
            builder.HasKey(e => e.EventId);
        });
    }
}
```

### When To Use It

Use the outbox pattern when a service:
- commits database changes
- and publishes integration events that must not be lost

Do not add it to services that never publish events.
The current repo can publish directly for low-risk demo flows, but production-critical workflows should move to the integration event log and background publish model.

## 9. Transactional Publish Flow

Preferred flow:
1. save domain changes
2. save integration event record in the same database transaction
3. commit transaction
4. publish to RabbitMQ
5. mark the event as published

This reduces the classic DB-commit / broker-publish gap.

## 10. When RabbitMQ Is Appropriate

Good uses:
- scheduled collection
- background enrichment
- async analytics
- cross-service notifications
- eventual consistency workflows

Bad early uses:
- the first synchronous search request
- one-service-only orchestration that does not need async behavior
- replacing simple direct HTTP calls prematurely

## 11. Current Firefly Signal Example

Current example flow:
1. `JobSearch.Api` handles the user search request synchronously.
2. It returns the search response over HTTP.
3. It publishes `JobSearchRequestedIntegrationEvent` to RabbitMQ.
4. `Ai.Api` consumes that event from its own queue and performs async follow-up work.

This is the preferred baseline for API communication in this repo:
- synchronous client-facing and request-response logic stays on HTTP
- asynchronous side effects and enrichment move through RabbitMQ

## 12. Recommended Firefly Signal Rule Set

- Keep the shared migration helper.
- Keep one seeder per `DbContext`.
- Keep event bus abstractions in a shared project.
- Keep RabbitMQ in a separate implementation project.
- Keep the integration event log / outbox pattern for event-publishing services.
- Do not introduce these heavier patterns until a service has a real reason to use them.
