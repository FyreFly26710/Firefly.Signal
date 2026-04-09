using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.External;
using Firefly.Signal.JobSearch.Infrastructure.Services;
using Firefly.Signal.EventBus;
using NSubstitute;

namespace Firefly.Signal.JobSearch.UnitTests;

[TestClass]
public class JobSearchServiceTests
{
    [TestMethod]
    public async Task SearchAsync_returns_provider_backed_results()
    {
        var provider = Substitute.For<IPublicJobSourceClient>();
        provider.SearchAsync(Arg.Any<SearchJobsRequest>(), Arg.Any<CancellationToken>())
            .Returns(new PublicJobSearchResult(
                1,
                [
                    JobPosting.Create(
                        "Backend .NET Developer",
                        "North Star Tech",
                        "SW1A 1AA",
                        "London",
                        "Build internal APIs with .NET 10.",
                        "https://example.com/jobs/backend-dotnet",
                        "Adzuna",
                        true,
                        new DateTime(2025, 4, 3, 10, 0, 0, DateTimeKind.Utc))
                ]));

        var eventBus = Substitute.For<IEventBus>();
        var service = new DbJobSearchService(provider, eventBus);

        var result = await service.SearchAsync(new SearchJobsRequest("SW1A", ".NET"));

        Assert.AreEqual(1, result.TotalCount);
        Assert.AreEqual("Backend .NET Developer", result.Jobs[0].Title);
        Assert.AreEqual("https://example.com/jobs/backend-dotnet", result.Jobs[0].Id);
    }
}
