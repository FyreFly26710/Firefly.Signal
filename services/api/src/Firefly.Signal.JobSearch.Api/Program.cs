using System.Text;
using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.EventBus;
using Firefly.Signal.EventBusRabbitMQ;
using Firefly.Signal.JobSearch.Endpoints;
using Firefly.Signal.JobSearch.Infrastructure.External;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Firefly.Signal.JobSearch.Infrastructure.Services;
using Firefly.Signal.ServiceDefaults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.AddFireflyServiceDefaults();
builder.AddDefaultOpenApi();
builder.Services.AddProblemDetails();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddDbContext<JobSearchDbContext>(options =>
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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
builder.Services.AddAuthorization();
builder.Services.AddScoped<IJobSearchService, DbJobSearchService>();
builder.Services.AddSingleton<IPublicJobSourceClient, NoOpPublicJobSourceClient>();
if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddSingleton<IEventBus, NoOpEventBus>();
}
else
{
    builder.AddRabbitMqEventBus("job-search-api");
}
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddMigration<JobSearchDbContext, JobSearchDbContextSeed>();
}

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    service = "job-search",
    message = "Firefly Signal job search API is running."
}));

app.MapJobSearchEndpoints();

app.UseDefaultOpenApi();
app.Run();

internal sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; init; } = "Firefly.Signal";
    public string Audience { get; init; } = "Firefly.Signal.Client";
    public string SigningKey { get; init; } = "firefly-signal-dev-signing-key-please-change";
}
