using Firefly.Signal.Gateway.Api.Apis;
using Firefly.Signal.Gateway.Api.Extensions;
using Firefly.Signal.Gateway.Api.Options;
using Firefly.Signal.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddFireflyServiceDefaults();
builder.AddApplicationServices();
builder.Services.AddProblemDetails();
builder.AddDefaultOpenApi();

var app = builder.Build();

app.UseCors(GatewayCorsPolicy.Frontend);
app.MapDefaultEndpoints();
app.MapGatewayStatusApi();
app.MapGatewayProxyApi();

app.UseDefaultOpenApi();
app.Run();
