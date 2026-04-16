using Firefly.Signal.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Firefly.Signal.Identity.FunctionalTests.Testing;

internal sealed class IdentityApiFactory : WebApplicationFactory<Firefly.Signal.Identity.Api.Program>
{
    private readonly string databaseName = $"identity-api-tests-{Guid.NewGuid():N}";

    public HttpClient CreateAuthenticatedClient(long userId = 42)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.UserIdHeaderName, userId.ToString());
        return client;
    }

    public async Task SeedAsync(params object[] entities)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        await dbContext.Database.EnsureCreatedAsync();
        dbContext.AddRange(entities);
        await dbContext.SaveChangesAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Testing:DatabaseName"] = databaseName,
                ["UserDocumentStorage:AllowedContentTypes:0"] = "application/pdf",
                ["UserDocumentStorage:AllowedFileExtensions:0"] = ".pdf"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthenticationHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthenticationHandler.SchemeName;
                    options.DefaultScheme = TestAuthenticationHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.SchemeName,
                    _ => { });
        });
    }
}
