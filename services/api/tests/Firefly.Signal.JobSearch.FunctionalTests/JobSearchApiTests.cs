using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Firefly.Signal.JobSearch.FunctionalTests;

[TestClass]
public class JobSearchApiTests
{
    [TestMethod]
    public async Task Jobs_requires_authentication()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/job-search/jobs?pageIndex=0&pageSize=20");

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task Get_page_returns_seeded_jobs_for_authenticated_user()
    {
        await using var factory = CreateFactory();
        await SeedJobAsync(factory.Services);
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateAccessToken());

        var response = await client.GetAsync("/api/job-search/jobs?pageIndex=0&pageSize=20");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadAsStringAsync();
        StringAssert.Contains(payload, ".NET Backend Developer");
        StringAssert.Contains(payload, "\"totalCount\":1");
    }

    [TestMethod]
    public async Task Delete_returns_conflict_when_job_is_not_hidden()
    {
        await using var factory = CreateFactory();
        var jobId = await SeedJobAsync(factory.Services);
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateAccessToken());

        var response = await client.DeleteAsync($"/api/job-search/jobs/{jobId}");

        Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
    }

    [TestMethod]
    public async Task Hide_then_delete_removes_job()
    {
        await using var factory = CreateFactory();
        var jobId = await SeedJobAsync(factory.Services);
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateAccessToken());

        var hideResponse = await client.PostAsJsonAsync($"/api/job-search/jobs/{jobId}/catalog-hide", new { });
        hideResponse.EnsureSuccessStatusCode();

        var deleteResponse = await client.DeleteAsync($"/api/job-search/jobs/{jobId}");
        Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/job-search/jobs/{jobId}");
        Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [TestMethod]
    public async Task Provider_import_persists_jobs_for_admin()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateAccessToken());

        var response = await client.PostAsJsonAsync("/api/job-search/jobs/import/provider", new
        {
            postcode = "SW1A 1AA",
            keyword = "platform",
            pageSize = 20
        });

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<ImportJobsResponse>();
        Assert.IsNotNull(payload);
        Assert.AreEqual(2, payload.ImportedCount);

        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<JobSearchDbContext>();
        Assert.AreEqual(2, await dbContext.Jobs.CountAsync());
        Assert.AreEqual(1, await dbContext.JobRefreshRuns.CountAsync());
    }

    [TestMethod]
    public async Task Export_returns_json_file_for_admin()
    {
        await using var factory = CreateFactory();
        await SeedJobAsync(factory.Services);
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateAccessToken());

        var response = await client.PostAsJsonAsync("/api/job-search/jobs/export", new ExportJobsRequest());

        response.EnsureSuccessStatusCode();
        Assert.AreEqual("application/json", response.Content.Headers.ContentType?.MediaType);

        var body = await response.Content.ReadAsStringAsync();
        var payload = JsonSerializer.Deserialize<ExportJobsResponse>(body, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.IsNotNull(payload);
        Assert.AreEqual(1, payload.Count);
        Assert.AreEqual(".NET Backend Developer", payload.Jobs[0].Title);
    }

    [TestMethod]
    public async Task Json_import_rejects_invalid_payload()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateAccessToken());

        using var content = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("{not-json"));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        content.Add(fileContent, "file", "jobs.json");

        var response = await client.PostAsync("/api/job-search/jobs/import/json", content);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        StringAssert.Contains(payload, "JSON import failed");
    }

    private static WebApplicationFactory<Firefly.Signal.JobSearch.Api.Program> CreateFactory()
    {
        var databaseName = Guid.NewGuid().ToString("N");

        return new WebApplicationFactory<Firefly.Signal.JobSearch.Api.Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.Single(x => x.ServiceType == typeof(DbContextOptions<JobSearchDbContext>));
                    services.Remove(descriptor);
                    services.AddDbContext<JobSearchDbContext>(options =>
                        options.UseInMemoryDatabase(databaseName));
                });
            });
    }

    private static async Task<long> SeedJobAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<JobSearchDbContext>();

        var job = JobPosting.Create(
            jobRefreshRunId: null,
            sourceName: "Adzuna",
            sourceJobId: Guid.NewGuid().ToString("N"),
            sourceAdReference: null,
            title: ".NET Backend Developer",
            description: "Build internal APIs with .NET 10, PostgreSQL and RabbitMQ.",
            summary: "Build internal APIs with .NET 10, PostgreSQL and RabbitMQ.",
            url: "https://example.com/jobs/backend-dotnet",
            company: "North Star Tech",
            companyDisplayName: "North Star Tech",
            companyCanonicalName: "north-star-tech",
            postcode: "SW1A 1AA",
            locationName: "London",
            locationDisplayName: "London",
            locationAreaJson: "[\"UK\",\"London\"]",
            latitude: null,
            longitude: null,
            categoryTag: "it-jobs",
            categoryLabel: "IT Jobs",
            salaryMin: 70000,
            salaryMax: 90000,
            salaryCurrency: "GBP",
            salaryIsPredicted: false,
            contractTime: "full_time",
            contractType: "permanent",
            isFullTime: true,
            isPartTime: false,
            isPermanent: true,
            isContract: false,
            isRemote: true,
            postedAtUtc: new DateTime(2025, 4, 3, 10, 0, 0, DateTimeKind.Utc),
            importedAtUtc: DateTime.UtcNow,
            lastSeenAtUtc: DateTime.UtcNow,
            isHidden: false,
            rawPayloadJson: "{}");

        dbContext.Jobs.Add(job);
        await dbContext.SaveChangesAsync();

        return job.Id;
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
