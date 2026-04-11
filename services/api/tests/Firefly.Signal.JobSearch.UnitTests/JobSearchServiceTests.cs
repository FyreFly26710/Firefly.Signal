using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.External;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Firefly.Signal.JobSearch.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.UnitTests;

[TestClass]
public class JobSearchServiceTests
{
    [TestMethod]
    public async Task GetPageAsync_returns_only_non_hidden_jobs_by_default()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Jobs.Add(JobPosting.Create(
            jobRefreshRunId: null,
            sourceName: "Adzuna",
            sourceJobId: "job-1",
            sourceAdReference: null,
            title: "Backend .NET Developer",
            description: "Build internal APIs with .NET 10.",
            summary: "Build internal APIs with .NET 10.",
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
            rawPayloadJson: "{}"));
        dbContext.Jobs.Add(JobPosting.Create(
            jobRefreshRunId: null,
            sourceName: "Adzuna",
            sourceJobId: "job-2",
            sourceAdReference: null,
            title: "Hidden Role",
            description: "Hidden role description.",
            summary: "Hidden role description.",
            url: "https://example.com/jobs/hidden",
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
            salaryMin: null,
            salaryMax: null,
            salaryCurrency: null,
            salaryIsPredicted: null,
            contractTime: null,
            contractType: null,
            isFullTime: false,
            isPartTime: false,
            isPermanent: false,
            isContract: true,
            isRemote: false,
            postedAtUtc: new DateTime(2025, 4, 1, 10, 0, 0, DateTimeKind.Utc),
            importedAtUtc: DateTime.UtcNow,
            lastSeenAtUtc: DateTime.UtcNow,
            isHidden: true,
            rawPayloadJson: "{}"));
        await dbContext.SaveChangesAsync();

        var service = new DbJobSearchService(dbContext, new NoOpJobSearchProvider());

        var result = await service.GetPageAsync(new GetJobsPageRequest(0, 20));

        Assert.AreEqual(1L, result.TotalCount);
        Assert.AreEqual("Backend .NET Developer", result.Items[0].Title);
    }

    [TestMethod]
    public async Task DeleteAsync_blocks_non_hidden_jobs()
    {
        await using var dbContext = CreateDbContext();
        var job = JobPosting.Create(
            "Backend .NET Developer",
            "North Star Tech",
            "SW1A 1AA",
            "London",
            "Build internal APIs with .NET 10.",
            "https://example.com/jobs/backend-dotnet",
            "Adzuna",
            true,
            new DateTime(2025, 4, 3, 10, 0, 0, DateTimeKind.Utc));
        dbContext.Jobs.Add(job);
        await dbContext.SaveChangesAsync();

        var service = new DbJobSearchService(dbContext, new NoOpJobSearchProvider());

        var result = await service.DeleteAsync([job.Id]);

        Assert.AreEqual(0, result.DeletedCount);
        CollectionAssert.AreEqual(new[] { job.Id }, result.NotHiddenIds.ToArray());
    }

    private static JobSearchDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<JobSearchDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new JobSearchDbContext(options);
    }

    private sealed class NoOpJobSearchProvider : IJobSearchProvider
    {
        public JobSearchProviderKind Provider => JobSearchProviderKind.Adzuna;

        public Task<PublicJobSearchResult> SearchAsync(SearchJobsRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(new PublicJobSearchResult(0, []));
    }
}
