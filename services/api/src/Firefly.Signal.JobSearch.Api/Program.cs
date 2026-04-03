using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.EventBus;
using Firefly.Signal.EventBusRabbitMQ;
using Firefly.Signal.JobSearch.Endpoints;
using Firefly.Signal.JobSearch.Infrastructure.External;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Firefly.Signal.JobSearch.Infrastructure.Services;
using Firefly.Signal.ServiceDefaults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.AddFireflyServiceDefaults();
builder.AddDefaultOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<JobSearchDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("FireflySignalDb"),
        npgsql => npgsql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, JobSearchDbContext.SchemaName));
});
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
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddMigration<JobSearchDbContext, JobSearchDbContextSeed>();
}

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => Results.Ok(new
{
    service = "job-search",
    message = "Firefly Signal job search API is running."
}));

app.MapJobSearchEndpoints();

app.UseDefaultOpenApi();
app.Run();
