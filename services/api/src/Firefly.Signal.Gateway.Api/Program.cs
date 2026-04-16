using Firefly.Signal.Gateway.Api.Apis;
using Firefly.Signal.Gateway.Api.Extensions;
using Firefly.Signal.Gateway.Api.Options;
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
app.UseCors(GatewayCorsPolicy.Frontend);
app.MapDefaultEndpoints();
app.MapGatewayStatusApi();
app.MapGatewayProxyApi();

app.UseDefaultOpenApi();
app.Run();
