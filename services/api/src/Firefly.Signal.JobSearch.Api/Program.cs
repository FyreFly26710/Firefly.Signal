using Firefly.Signal.JobSearch.Api.Apis;
using Firefly.Signal.JobSearch.Api.Extensions;
using Firefly.Signal.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddFireflyServiceDefaults();
builder.AddApplicationServices();
builder.Services.AddProblemDetails();
builder.AddDefaultOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseAuthentication();
app.UseAuthorization();
app.MapJobSearchApi();
app.MapUserJobStateApi();
app.MapJobApplicationApi();

app.UseDefaultOpenApi();
app.Run();
