using System.Net;
using System.Net.Http.Json;
using Firefly.Signal.JobSearch.Contracts.Requests;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.FunctionalTests.Testing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Firefly.Signal.JobSearch.FunctionalTests.Api;

[TestClass]
public sealed class JobApplicationApiTests
{
    [TestMethod]
    public async Task AdvanceStatus_WhenRequestStatusIsInvalid_ReturnsBadRequest()
    {
        using var factory = new JobSearchApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PutAsJsonAsync(
            "/api/job-search/jobs/999/apply/status",
            new AdvanceApplicationStatusRequest("not-a-real-status"));

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.IsNotNull(problem);
        Assert.AreEqual("Invalid status", problem.Title);
    }

    [TestMethod]
    public async Task AdvanceStatus_WhenRequestIsUnauthenticated_ReturnsUnauthorized()
    {
        using var factory = new JobSearchApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync(
            "/api/job-search/jobs/999/apply/status",
            new AdvanceApplicationStatusRequest(JobApplicationStatus.TelephoneInterview.ToString()));

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task AdvanceStatus_WhenApplicationDoesNotExist_ReturnsNotFound()
    {
        using var factory = new JobSearchApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PutAsJsonAsync(
            "/api/job-search/jobs/999/apply/status",
            new AdvanceApplicationStatusRequest(JobApplicationStatus.TelephoneInterview.ToString()));

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task AdvanceStatus_WhenApplicationExists_ReturnsOk()
    {
        using var factory = new JobSearchApiFactory();
        var job = JobSearchTestData.CreateJob("API Test Role", postedAtUtc: DateTime.UtcNow.AddDays(-1));
        var application = JobApplication.Create(userAccountId: 42, jobPostingId: job.Id, note: "Applied via tests");

        await factory.SeedAsync(job, application);

        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PutAsJsonAsync(
            $"/api/job-search/jobs/{job.Id}/apply/status",
            new AdvanceApplicationStatusRequest(JobApplicationStatus.TelephoneInterview.ToString()));

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JobApplicationResponse>();

        Assert.IsNotNull(body);
        Assert.AreEqual(job.Id, body.JobPostingId);
        Assert.AreEqual(JobApplicationStatus.TelephoneInterview.ToString(), body.CurrentStatus);

        var entryCount = await factory.ExecuteDbContextAsync(
            async dbContext => await dbContext.JobApplicationStatusEntries.CountAsync(entry => entry.JobApplicationId == application.Id));

        Assert.AreEqual(1, entryCount);
    }
}
