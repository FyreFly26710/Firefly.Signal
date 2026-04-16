using Firefly.Signal.Identity.Api.Apis;
using Firefly.Signal.Identity.Api.Extensions;
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
app.MapAuthApi();
app.MapUserProfileApi();
app.MapUserApi();
app.MapUserDocumentApi();

app.UseDefaultOpenApi();
app.Run();
