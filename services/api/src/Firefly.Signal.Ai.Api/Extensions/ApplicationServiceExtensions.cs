using System.Text.Json.Serialization;
using Firefly.Signal.Ai.Infrastructure.Persistence;
using Firefly.Signal.EventBus;
using Firefly.Signal.EventBusRabbitMQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Firefly.Signal.SharedKernel.Extensions;

namespace Firefly.Signal.Ai.Api.Extensions;

internal static class ApplicationServiceExtensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.AddFireflyMediator(typeof(Program).Assembly);

        services.AddDbContext<AiDbContext>(options =>
        {
            if (builder.Environment.IsEnvironment("Testing"))
            {
                options.UseInMemoryDatabase(builder.Configuration["Testing:DatabaseName"] ?? "firefly-signal-ai-testing");
                return;
            }

            options.UseNpgsql(
                builder.Configuration.GetConnectionString("FireflySignalDb"),
                npgsql => npgsql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, AiDbContext.SchemaName));
        });

        if (builder.Environment.IsEnvironment("Testing"))
        {
            services.AddSingleton<IEventBus, NoOpEventBus>();
        }
        else
        {
            builder.AddRabbitMqEventBus("ai-api");
            services.AddMigration<AiDbContext>();
        }
    }
}
