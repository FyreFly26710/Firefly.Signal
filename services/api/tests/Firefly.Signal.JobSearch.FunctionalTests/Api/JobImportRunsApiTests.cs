using System.Net;
using System.Net.Http.Json;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.FunctionalTests.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Firefly.Signal.JobSearch.FunctionalTests.Api;

[TestClass]
public sealed class JobImportRunsApiTests
{
    [TestMethod]
    public async Task GetRecent_WhenUserIsNotAdmin_ReturnsForbidden()
    {
        using var factory = new JobSearchApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/job-search/jobs/import-runs/recent");

        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [TestMethod]
    public async Task GetRecent_WhenUserIsAdmin_ReturnsRecentImportRuns()
    {
        using var factory = new JobSearchApiFactory();

        var completedRun = JobRefreshRun.Start(
            providerName: "Adzuna",
            countryCode: "gb",
            requestFiltersJson: "{\"where\":\"london\"}",
            requestedPageSize: 50,
            requestedMaxPages: 1);
        completedRun.RecordFetchedPage(5);
        completedRun.RecordInsertedJobs(4);
        completedRun.Complete();

        await Task.Delay(20);

        var failedRun = JobRefreshRun.Start(
            providerName: "Adzuna",
            countryCode: "gb",
            requestFiltersJson: "{\"where\":\"leeds\"}",
            requestedPageSize: 25,
            requestedMaxPages: 1);
        failedRun.RecordFetchedPage(1);
        failedRun.RecordFailedItems(1);
        failedRun.Fail("Provider import failed.");

        await factory.SeedAsync(completedRun, failedRun);

        using var client = factory.CreateAuthenticatedClient(role: "admin");

        var response = await client.GetAsync("/api/job-search/jobs/import-runs/recent?limit=5");

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<IReadOnlyList<JobImportRunResponse>>();

        Assert.IsNotNull(body);
        Assert.HasCount(2, body);
        Assert.AreEqual(failedRun.Id, body[0].Id);
        Assert.AreEqual(JobRefreshRunStatus.Failed.ToString(), body[0].Status);
        Assert.AreEqual("Provider import failed.", body[0].FailureSummary);
        Assert.AreEqual(completedRun.Id, body[1].Id);
        Assert.AreEqual(JobRefreshRunStatus.Completed.ToString(), body[1].Status);
        Assert.IsNull(body[1].FailureSummary);
    }
}
