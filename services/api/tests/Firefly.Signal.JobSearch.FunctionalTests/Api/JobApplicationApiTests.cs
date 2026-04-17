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
        Assert.HasCount(1, body.StatusHistory);

        var entryCount = await factory.ExecuteDbContextAsync(
            async dbContext => await dbContext.JobApplicationStatusEntries.CountAsync(entry => entry.JobApplicationId == application.Id));

        Assert.AreEqual(1, entryCount);
    }

    [TestMethod]
    public async Task Apply_WhenJobExists_SetsUserStateAppliedAndReturnsTimeline()
    {
        using var factory = new JobSearchApiFactory();
        var job = JobSearchTestData.CreateJob("API Apply Role", postedAtUtc: DateTime.UtcNow.AddDays(-1));

        await factory.SeedAsync(job);

        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync(
            $"/api/job-search/jobs/{job.Id}/apply",
            new ApplyJobRequest("Applied from api test"));

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JobApplicationResponse>();

        Assert.IsNotNull(body);
        Assert.AreEqual(JobApplicationStatus.Applied.ToString(), body.CurrentStatus);
        Assert.HasCount(1, body.StatusHistory);

        var isApplied = await factory.ExecuteDbContextAsync(async dbContext =>
            await dbContext.UserJobStates
                .Where(state => state.JobPostingId == job.Id && state.UserAccountId == 42)
                .Select(state => state.IsApplied)
                .SingleAsync());

        Assert.IsTrue(isApplied);
    }

    [TestMethod]
    public async Task GetAppliedJobDetail_WhenApplicationExists_ReturnsExpandedDetail()
    {
        using var factory = new JobSearchApiFactory();
        var job = JobSearchTestData.CreateJob("API Test Role", postedAtUtc: DateTime.UtcNow.AddDays(-1));
        var application = JobApplication.Create(userAccountId: 42, jobPostingId: job.Id, note: "Applied via tests");
        var appliedEntry = JobApplicationStatusEntry.Create(application.Id, JobApplicationStatus.Applied, statusAtUtc: new DateTime(2026, 4, 10, 10, 0, 0, DateTimeKind.Utc));
        var firstRound = JobApplicationStatusEntry.Create(application.Id, JobApplicationStatus.FaceToFaceInterview, roundNumber: 1, statusAtUtc: new DateTime(2026, 4, 11, 10, 0, 0, DateTimeKind.Utc));

        await factory.SeedAsync(job, application, appliedEntry, firstRound);

        using var client = factory.CreateAuthenticatedClient();

        var response = await client.GetAsync($"/api/job-search/applications/{application.Id}");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<AppliedJobDetailResponse>();

        Assert.IsNotNull(body);
        Assert.AreEqual(job.Id, body.JobPostingId);
        Assert.HasCount(2, body.StatusHistory);
        Assert.AreEqual(1, body.StatusHistory[1].RoundNumber);
    }
}
