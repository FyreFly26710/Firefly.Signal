using Firefly.Signal.EventBus;
using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.External;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Firefly.Signal.JobSearch.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Firefly.Signal.JobSearch.UnitTests;

[TestClass]
public class DemoJobSearchServiceTests
{
    [TestMethod]
    public async Task SearchAsync_filters_by_keyword()
    {
        var dbOptions = new DbContextOptionsBuilder<JobSearchDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new JobSearchDbContext(dbOptions);
        dbContext.Jobs.Add(JobPosting.Create(
            "Backend .NET Developer",
            "North Star Tech",
            "SW1A 1AA",
            "London",
            "Build internal APIs with .NET 10.",
            "https://example.com/jobs/backend-dotnet",
            "sample-feed",
            true,
            DateTime.UtcNow));
        await dbContext.SaveChangesAsync();

        var eventBus = Substitute.For<IEventBus>();
        var service = new DbJobSearchService(dbContext, new NoOpPublicJobSourceClient(), eventBus);

        var result = await service.SearchAsync(new SearchJobsRequest("SW1A", ".NET"));

        Assert.AreEqual(1, result.TotalCount);
        Assert.AreEqual("Backend .NET Developer", result.Jobs[0].Title);
    }
}
