using System.Text;
using System.Text.Json.Serialization;
using Firefly.Signal.EventBus;
using Firefly.Signal.EventBus.Events.Ai;
using Firefly.Signal.EventBusRabbitMQ;
using Firefly.Signal.JobSearch.Application.IntegrationEventHandlers;
using Firefly.Signal.JobSearch.Application.Commands;
using Firefly.Signal.JobSearch.Application.Queries;
using Firefly.Signal.JobSearch.Api.Options;
using Firefly.Signal.JobSearch.Infrastructure.External;
using Firefly.Signal.JobSearch.Infrastructure.JobSearchProviders.Adzuna;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.Tokens;
using Firefly.Signal.SharedKernel.Extensions;
using Firefly.Signal.SharedKernel.Services;

namespace Firefly.Signal.JobSearch.Api.Extensions;

internal static class ApplicationServiceExtensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
        services.Configure<AdzunaOptions>(builder.Configuration.GetSection(AdzunaOptions.SectionName));
        services.Configure<DemoAiChatOptions>(builder.Configuration.GetSection(DemoAiChatOptions.SectionName));
        services.AddHttpContextAccessor();
        services.AddFireflyMediator(typeof(Program).Assembly);
        services.AddScoped<IIdentityService, HttpContextIdentityService>();
        services.AddScoped<IJobSearchQueries, JobSearchQueries>();
        services.AddScoped<IJobApplicationQueries, JobApplicationQueries>();
        services.AddScoped<IUserProfileQueries, UserProfileQueries>();
        services.AddSingleton<AdzunaJobSearchRequestMapper>();
        services.AddSingleton<AdzunaJobSearchResponseMapper>();
        services.AddSingleton<MockAdzunaJobSearchProvider>();
        services.AddHttpClient<AdzunaJobSearchProvider>();
        services.AddScoped<IJobSearchProvider>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AdzunaOptions>>().Value;
            return options.UseLiveApi
                ? serviceProvider.GetRequiredService<AdzunaJobSearchProvider>()
                : serviceProvider.GetRequiredService<MockAdzunaJobSearchProvider>();
        });

        services.AddDbContext<JobSearchDbContext>(options =>
        {
            if (builder.Environment.IsEnvironment("Testing"))
            {
                options.UseInMemoryDatabase(builder.Configuration["Testing:DatabaseName"] ?? "firefly-signal-job-search-testing");
                return;
            }

            options.UseNpgsql(
                builder.Configuration.GetConnectionString("FireflySignalDb"),
                npgsql => npgsql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, JobSearchDbContext.SchemaName));
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
                };
            });

        services.AddAuthorization();

        if (builder.Environment.IsEnvironment("Testing"))
        {
            services.AddSingleton<IEventBus, NoOpEventBus>();
        }
        else
        {
            builder.AddRabbitMqEventBus("job-search-api")
                .AddSubscription<AiChatCompletedIntegrationEvent, AiChatCompletedIntegrationEventHandler>();
            services.AddMigration<JobSearchDbContext, JobSearchDbContextSeed>();
        }
    }
}
