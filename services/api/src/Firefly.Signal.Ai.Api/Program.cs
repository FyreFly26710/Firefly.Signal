using Firefly.Signal.Ai.Api.Apis;
using Firefly.Signal.Ai.Api.Extensions;
using Firefly.Signal.ServiceDefaults;
using Firefly.Signal.SharedKernel.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddFireflyServiceDefaults();
builder.AddApplicationServices();
builder.Services.AddProblemDetails();
builder.Services.AddFireflyExceptionHandling();
builder.AddDefaultOpenApi();

var app = builder.Build();

app.UseFireflyExceptionHandling();
app.MapDefaultEndpoints();
app.MapAiChatApi();

app.UseDefaultOpenApi();
app.Run();
