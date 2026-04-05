using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Firefly.Signal.EventBus;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.External;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Firefly.Signal.JobSearch.FunctionalTests;

[TestClass]
public class JobSearchApiTests
{
    [TestMethod]
    public async Task Search_requires_authentication()
    {
        await using var factory = CreateFactory();
        SeedJobData(factory.Services);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/job-search/search?postcode=SW1A&keyword=.NET&pageIndex=0&pageSize=20");

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task Search_returns_results_when_bearer_token_is_present()
    {
        await using var factory = CreateFactory();
        SeedJobData(factory.Services);
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateAccessToken());

        var response = await client.GetAsync("/api/job-search/search?postcode=SW1A&keyword=.NET&pageIndex=0&pageSize=20");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadAsStringAsync();

        StringAssert.Contains(payload, ".NET Backend Developer");
    }

    private static WebApplicationFactory<Firefly.Signal.JobSearch.Api.Program> CreateFactory()
    {
        var databaseName = $"job-search-tests-{Guid.NewGuid():N}";

        return new WebApplicationFactory<Firefly.Signal.JobSearch.Api.Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.UseSetting("Testing:DatabaseName", databaseName);
                builder.ConfigureServices(_ =>
                {
                });
            });
    }

    private static void SeedJobData(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<JobSearchDbContext>();

        if (dbContext.Jobs.Any())
        {
            return;
        }

        dbContext.Jobs.AddRange(
            JobPosting.Create(
                ".NET Backend Developer",
                "North Star Tech",
                "SW1A 1AA",
                "London",
                "Build internal APIs with .NET 10, PostgreSQL and RabbitMQ.",
                "https://example.com/jobs/backend-dotnet",
                "sample-feed",
                true,
                DateTime.UtcNow.AddDays(-1)),
            JobPosting.Create(
                "Platform Engineer",
                "Cloud Rail",
                "EH1 1YZ",
                "Edinburgh",
                "Help modernize event-driven services and CI/CD pipelines.",
                "https://example.com/jobs/platform-engineer",
                "sample-feed",
                true,
                DateTime.UtcNow.AddDays(-3)));

        dbContext.SaveChanges();
    }

    private static string CreateAccessToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("firefly-signal-dev-signing-key-please-change"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "Firefly.Signal",
            audience: "Firefly.Signal.Client",
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, "1"),
                new Claim(JwtRegisteredClaimNames.UniqueName, "admin"),
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, "admin")
            ],
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
