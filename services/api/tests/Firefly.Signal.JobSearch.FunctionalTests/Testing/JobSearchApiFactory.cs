using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Firefly.Signal.EventBus;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Firefly.Signal.JobSearch.FunctionalTests.Testing;

internal sealed class JobSearchApiFactory : WebApplicationFactory<Firefly.Signal.JobSearch.Api.Program>
{
    private readonly string databaseName = $"job-search-api-tests-{Guid.NewGuid():N}";
    private readonly IEventBus? eventBus;

    public JobSearchApiFactory(IEventBus? eventBus = null)
    {
        this.eventBus = eventBus;
    }

    public HttpClient CreateAuthenticatedClient(long userId = 42, string? role = null)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.UserIdHeaderName, userId.ToString());
        if (!string.IsNullOrWhiteSpace(role))
        {
            client.DefaultRequestHeaders.Add(TestAuthenticationHandler.RoleHeaderName, role);
        }

        return client;
    }

    public async Task SeedAsync(params object[] entities)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<JobSearchDbContext>();

        await dbContext.Database.EnsureCreatedAsync();
        dbContext.AddRange(entities);
        await dbContext.SaveChangesAsync();
    }

    public async Task<TResult> ExecuteDbContextAsync<TResult>(Func<JobSearchDbContext, Task<TResult>> action)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<JobSearchDbContext>();

        await dbContext.Database.EnsureCreatedAsync();
        return await action(dbContext);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Testing:DatabaseName"] = databaseName
            });
        });
        builder.ConfigureServices(services =>
        {
            if (eventBus is not null)
            {
                services.RemoveAll<IEventBus>();
                services.AddSingleton(eventBus);
            }

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
