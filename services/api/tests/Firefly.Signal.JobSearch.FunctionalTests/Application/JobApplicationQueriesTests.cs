using Firefly.Signal.JobSearch.Application.Queries;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.FunctionalTests.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Firefly.Signal.JobSearch.FunctionalTests.Application;

[TestClass]
public sealed class JobApplicationQueriesTests
{
    [TestMethod]
    public async Task GetAppliedJobsAsync_ReturnsAppliedAndLatestTimestamps()
    {
        await using var database = new JobSearchSqliteTestDatabase();
        await using var dbContext = database.CreateDbContext();

        var job = JobSearchTestData.CreateJob("Backend Engineer", postedAtUtc: DateTime.UtcNow.AddDays(-1));
        var application = JobApplication.Create(userAccountId: 42, jobPostingId: job.Id, note: "Applied");
        var appliedAtUtc = new DateTime(2026, 4, 10, 10, 0, 0, DateTimeKind.Utc);
        var latestAtUtc = new DateTime(2026, 4, 12, 11, 0, 0, DateTimeKind.Utc);
        var appliedEntry = JobApplicationStatusEntry.Create(application.Id, JobApplicationStatus.Applied, statusAtUtc: appliedAtUtc);
        var phoneEntry = JobApplicationStatusEntry.Create(application.Id, JobApplicationStatus.TelephoneInterview, statusAtUtc: latestAtUtc);

        dbContext.Jobs.Add(job);
        dbContext.JobApplications.Add(application);
        dbContext.JobApplicationStatusEntries.AddRange(appliedEntry, phoneEntry);
        await dbContext.SaveChangesAsync();

        var queries = new JobApplicationQueries(dbContext);

        var results = await queries.GetAppliedJobsAsync(42, CancellationToken.None);

        Assert.HasCount(1, results);
        Assert.AreEqual(appliedAtUtc, results[0].AppliedAtUtc);
        Assert.AreEqual(latestAtUtc, results[0].LatestStatusAtUtc);
    }

    [TestMethod]
    public async Task GetAppliedJobDetailAsync_ReturnsTimelineWithRoundNumbers()
    {
        await using var database = new JobSearchSqliteTestDatabase();
        await using var dbContext = database.CreateDbContext();

        var job = JobSearchTestData.CreateJob("Backend Engineer", postedAtUtc: DateTime.UtcNow.AddDays(-1));
        var application = JobApplication.Create(userAccountId: 42, jobPostingId: job.Id, note: "Round two");
        var appliedEntry = JobApplicationStatusEntry.Create(application.Id, JobApplicationStatus.Applied, statusAtUtc: new DateTime(2026, 4, 10, 10, 0, 0, DateTimeKind.Utc));
        var firstRound = JobApplicationStatusEntry.Create(application.Id, JobApplicationStatus.FaceToFaceInterview, roundNumber: 1, statusAtUtc: new DateTime(2026, 4, 11, 10, 0, 0, DateTimeKind.Utc));
        var secondRound = JobApplicationStatusEntry.Create(application.Id, JobApplicationStatus.FaceToFaceInterview, roundNumber: 2, statusAtUtc: new DateTime(2026, 4, 12, 10, 0, 0, DateTimeKind.Utc));

        dbContext.Jobs.Add(job);
        dbContext.JobApplications.Add(application);
        dbContext.JobApplicationStatusEntries.AddRange(appliedEntry, firstRound, secondRound);
        await dbContext.SaveChangesAsync();

        var queries = new JobApplicationQueries(dbContext);

        var detail = await queries.GetAppliedJobDetailAsync(application.Id, 42, CancellationToken.None);

        Assert.IsNotNull(detail);
        Assert.AreEqual(job.Id, detail.JobPostingId);
        Assert.AreEqual(JobApplicationStatus.FaceToFaceInterview.ToString(), detail.CurrentStatus);
        Assert.HasCount(3, detail.StatusHistory);
        Assert.AreEqual(1, detail.StatusHistory[1].RoundNumber);
        Assert.AreEqual(2, detail.StatusHistory[2].RoundNumber);
    }
}
