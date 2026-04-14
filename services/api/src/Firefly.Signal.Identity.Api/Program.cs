using Firefly.Signal.Identity.Api.Apis;
using Firefly.Signal.Identity.Api.Extensions;
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
app.MapIdentityStatusApi();
app.MapAuthApi();
app.MapUserProfileApi();
app.MapUserApi();
app.MapUserDocumentApi();

app.UseDefaultOpenApi();
app.Run();
