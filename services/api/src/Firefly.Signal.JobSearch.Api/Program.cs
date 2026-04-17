using Firefly.Signal.JobSearch.Api.Apis;
using Firefly.Signal.JobSearch.Api.Extensions;
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
app.UseAuthentication();
app.UseAuthorization();
app.MapJobApi();
app.MapUserJobStateApi();
app.MapJobApplicationApi();

app.UseDefaultOpenApi();
app.Run();
