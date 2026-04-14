using Firefly.Signal.Ai.Api.Apis;
using Firefly.Signal.Ai.Api.Extensions;
using Firefly.Signal.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddFireflyServiceDefaults();
builder.AddApplicationServices();
builder.Services.AddProblemDetails();
builder.AddDefaultOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapAiApi();

app.UseDefaultOpenApi();
app.Run();
