using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Firefly.Signal.EventBus;
using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.External;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace Firefly.Signal.JobSearch.FunctionalTests;

[TestClass]
public class JobSearchApiTests
{
    [TestMethod]
    public async Task Search_requires_authentication()
    {
        await using var factory = CreateFactory(new StubPublicJobSourceClient());
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/job-search/search?postcode=SW1A&keyword=.NET&pageIndex=0&pageSize=20");

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task Search_returns_results_when_bearer_token_is_present()
    {
        await using var factory = CreateFactory(new StubPublicJobSourceClient());
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateAccessToken());

        var response = await client.GetAsync("/api/job-search/search?postcode=SW1A&keyword=.NET&pageIndex=0&pageSize=20");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadAsStringAsync();

        StringAssert.Contains(payload, ".NET Backend Developer");
        StringAssert.Contains(payload, "\"totalCount\":1");
    }

    [TestMethod]
    public async Task Search_returns_service_unavailable_when_provider_fails()
    {
        await using var factory = CreateFactory(new ThrowingPublicJobSourceClient());
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateAccessToken());

        var response = await client.GetAsync("/api/job-search/search?postcode=SW1A&keyword=.NET&pageIndex=0&pageSize=20");

        Assert.AreEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        StringAssert.Contains(payload, "Job search provider unavailable");
    }

    private static WebApplicationFactory<Firefly.Signal.JobSearch.Api.Program> CreateFactory(IPublicJobSourceClient providerClient)
    {
        return new WebApplicationFactory<Firefly.Signal.JobSearch.Api.Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IPublicJobSourceClient>();
                    services.AddSingleton(providerClient);
                });
            });
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

    private sealed class StubPublicJobSourceClient : IPublicJobSourceClient
    {
        public Task<PublicJobSearchResult> SearchAsync(SearchJobsRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(new PublicJobSearchResult(
                1,
                [
                    JobPosting.Create(
                        ".NET Backend Developer",
                        "North Star Tech",
                        request.Postcode,
                        "London",
                        "Build internal APIs with .NET 10, PostgreSQL and RabbitMQ.",
                        "https://example.com/jobs/backend-dotnet",
                        "Adzuna",
                        true,
                        new DateTime(2025, 4, 3, 10, 0, 0, DateTimeKind.Utc))
                ]));
    }

    private sealed class ThrowingPublicJobSourceClient : IPublicJobSourceClient
    {
        public Task<PublicJobSearchResult> SearchAsync(SearchJobsRequest request, CancellationToken cancellationToken = default)
            => throw new JobSearchProviderException("The configured job search provider could not return results right now.");
    }
}
