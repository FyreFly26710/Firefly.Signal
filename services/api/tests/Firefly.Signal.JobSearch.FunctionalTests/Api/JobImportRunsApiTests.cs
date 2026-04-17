using System.Net;
using System.Net.Http.Json;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.FunctionalTests.Testing;
using Firefly.Signal.SharedKernel.Models;
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

        var response = await client.GetAsync("/api/job-search/jobs/import-runs");

        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [TestMethod]
    public async Task GetRecent_WhenUserIsAdmin_ReturnsPagedRecentImportRuns()
    {
        using var factory = new JobSearchApiFactory();

        var olderRun = JobRefreshRun.Start(
            providerName: "Adzuna",
            countryCode: "gb",
            requestFiltersJson: "{\"where\":\"york\"}",
            requestedPageSize: 10,
            requestedMaxPages: 1);
        olderRun.RecordFetchedPage(1);
        olderRun.RecordInsertedJobs(1);
        olderRun.Complete();

        await Task.Delay(20);

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

        await factory.SeedAsync(completedRun, failedRun, olderRun);

        using var client = factory.CreateAuthenticatedClient(role: "admin");

        var response = await client.GetAsync("/api/job-search/jobs/import-runs?pageIndex=0&pageSize=2");

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<Paged<JobImportRunResponse>>();

        Assert.IsNotNull(body);
        Assert.AreEqual(0, body.PageIndex);
        Assert.AreEqual(2, body.PageSize);
        Assert.AreEqual(3L, body.TotalCount);
        Assert.HasCount(2, body.Items);
        Assert.AreEqual(failedRun.Id, body.Items[0].Id);
        Assert.AreEqual(JobRefreshRunStatus.Failed.ToString(), body.Items[0].Status);
        Assert.AreEqual("Provider import failed.", body.Items[0].FailureSummary);
        Assert.AreEqual(olderRun.Id, body.Items[1].Id);
        Assert.AreEqual(JobRefreshRunStatus.Completed.ToString(), body.Items[1].Status);
        Assert.IsNull(body.Items[1].FailureSummary);
    }
}
