using System.Text;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Endpoints;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Firefly.Signal.Identity.Infrastructure.Services;
using Firefly.Signal.Identity.Infrastructure.Storage;
using Firefly.Signal.ServiceDefaults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.AddFireflyServiceDefaults();
builder.AddDefaultOpenApi();
builder.Services.AddProblemDetails();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddUserDocumentStorage(builder.Configuration, builder.Environment);
builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    if (builder.Environment.IsEnvironment("Testing"))
    {
        options.UseInMemoryDatabase(builder.Configuration["Testing:DatabaseName"] ?? "firefly-signal-identity-testing");
        return;
    }

    options.UseNpgsql(
        builder.Configuration.GetConnectionString("FireflySignalDb"),
        npgsql => npgsql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, IdentityDbContext.SchemaName));
});
builder.Services.AddScoped<IPasswordHasher<UserAccount>, PasswordHasher<UserAccount>>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
        };
    });
builder.Services.AddAuthorization();
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddMigration<IdentityDbContext, IdentityDbContextSeed>();
}

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    service = "identity",
    message = "Firefly Signal identity API is running."
}));

app.MapAuthEndpoints();
app.MapUserProfileEndpoints();
app.MapUserEndpoints();
app.MapUserDocumentEndpoints();

app.UseDefaultOpenApi();
app.Run();
