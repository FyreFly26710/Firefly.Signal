using Firefly.Signal.Ai.Api.Consumers;
using Firefly.Signal.Ai.Api.Endpoints;
using Firefly.Signal.Ai.Api.Services;
using Firefly.Signal.EventBus;
using Firefly.Signal.EventBusRabbitMQ;
using Firefly.Signal.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddFireflyServiceDefaults();
builder.AddDefaultOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<IJobInsightService, NoOpJobInsightService>();
builder.AddRabbitMqEventBus("ai-api")
    .AddSubscription<JobSearchRequestedIntegrationEvent, JobSearchRequestedIntegrationEventHandler>();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapGet("/", () => Results.Ok(new
{
    service = "ai",
    message = "Firefly Signal AI API is running."
}));
app.MapAiEndpoints();

app.UseDefaultOpenApi();
app.Run();
