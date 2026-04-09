using System.Net;
using System.Net.Http;
using System.Text;
using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Infrastructure.External;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Firefly.Signal.JobSearch.UnitTests;

[TestClass]
public class AdzunaJobSearchProviderTests
{
    [TestMethod]
    public async Task SearchAsync_builds_provider_request_and_maps_results()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                """
                {
                  "count": 27,
                  "results": [
                    {
                      "id": "123",
                      "title": ".NET Developer",
                      "description": "Remote-first backend role.",
                      "redirect_url": "https://example.com/jobs/123",
                      "created": "2025-04-03T10:00:00Z",
                      "company": {
                        "display_name": "North Star Tech"
                      },
                      "location": {
                        "display_name": "London"
                      }
                    }
                  ]
                }
                """,
                Encoding.UTF8,
                "application/json")
        });

        var provider = new AdzunaJobSearchProvider(
            new HttpClient(handler),
            Options.Create(new AdzunaOptions
            {
                BaseUrl = "https://api.adzuna.com",
                CountryCode = "gb",
                AppId = "app-id",
                AppKey = "app-key"
            }),
            new AdzunaJobSearchRequestMapper(),
            new AdzunaJobSearchResponseMapper(),
            NullLogger<AdzunaJobSearchProvider>.Instance);

        var result = await provider.SearchAsync(new SearchJobsRequest("SW1A 1AA", ".NET", 1, 25, JobSearchProviderKind.Adzuna));

        Assert.AreEqual(27, result.TotalCount);
        Assert.HasCount(1, result.Jobs);
        Assert.AreEqual(".NET Developer", result.Jobs[0].Title);
        Assert.AreEqual("North Star Tech", result.Jobs[0].Company);
        Assert.AreEqual("London", result.Jobs[0].LocationName);
        Assert.AreEqual("Adzuna", result.Jobs[0].SourceName);
        Assert.IsTrue(result.Jobs[0].IsRemote);
        StringAssert.StartsWith(
            handler.LastRequestUri,
            "https://api.adzuna.com/v1/api/jobs/gb/search/2?");
        StringAssert.Contains(handler.LastRequestUri, "app_id=app-id");
        StringAssert.Contains(handler.LastRequestUri, "app_key=app-key");
        StringAssert.Contains(handler.LastRequestUri, "results_per_page=25");
        StringAssert.Contains(handler.LastRequestUri, "what=.NET");
        StringAssert.Contains(handler.LastRequestUri, "where=SW1A");
    }

    [TestMethod]
    public async Task SearchAsync_throws_when_credentials_are_missing()
    {
        var provider = new AdzunaJobSearchProvider(
            new HttpClient(new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK))),
            Options.Create(new AdzunaOptions()),
            new AdzunaJobSearchRequestMapper(),
            new AdzunaJobSearchResponseMapper(),
            NullLogger<AdzunaJobSearchProvider>.Instance);

        await Assert.ThrowsExactlyAsync<JobSearchProviderException>(() =>
            provider.SearchAsync(new SearchJobsRequest("SW1A 1AA", ".NET", 0, 20, JobSearchProviderKind.Adzuna)));
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler) : HttpMessageHandler
    {
        public string? LastRequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequestUri = request.RequestUri?.ToString();
            return Task.FromResult(handler(request));
        }
    }
}
