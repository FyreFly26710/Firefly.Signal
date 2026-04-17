using Firefly.Signal.JobSearch.Application.Queries;
using Firefly.Signal.JobSearch.Contracts.Requests;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.FunctionalTests.Testing;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Firefly.Signal.JobSearch.FunctionalTests.Application;

[TestClass]
public sealed class JobSearchQueriesTests
{
    [TestMethod]
    public async Task SearchPageAsync_WhenPagingValuesAreInvalid_UsesDefaultsAndProjectsUserState()
    {
        await using var database = new JobSearchSqliteTestDatabase();
        await using var dbContext = database.CreateDbContext();

        var visibleJobs = new List<Firefly.Signal.JobSearch.Domain.JobPosting>();

        for (var index = 0; index < 22; index++)
        {
            visibleJobs.Add(JobSearchTestData.CreateJob(
                title: $"Role {index:00}",
                postedAtUtc: new DateTime(2026, 4, 1, 12, 0, 0, DateTimeKind.Utc).AddMinutes(index)));
        }

        var hiddenJob = JobSearchTestData.CreateJob(
            title: "Hidden role",
            postedAtUtc: new DateTime(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc),
            isHidden: true);

        dbContext.Jobs.AddRange(visibleJobs);
        dbContext.Jobs.Add(hiddenJob);

        var projectedState = Firefly.Signal.JobSearch.Domain.UserJobState.Create(userAccountId: 42, jobPostingId: visibleJobs[^1].Id);
        projectedState.MarkSaved();
        projectedState.Hide();

        dbContext.UserJobStates.Add(projectedState);
        await dbContext.SaveChangesAsync();

        var queries = new JobSearchQueries(dbContext);

        var page = await queries.SearchPageAsync(
            new GetJobsPageRequest(PageIndex: -5, PageSize: 0),
            userId: 42,
            cancellationToken: CancellationToken.None);

        Assert.AreEqual(0, page.PageIndex);
        Assert.AreEqual(20, page.PageSize);
        Assert.AreEqual(22L, page.TotalCount);
        Assert.HasCount(20, page.Items);
        Assert.AreEqual(visibleJobs[^1].Id, page.Items[0].Id);
        Assert.IsTrue(page.Items[0].IsSaved);
        Assert.IsTrue(page.Items[0].IsUserHidden);
        Assert.IsFalse(page.Items.Any(item => item.Id == hiddenJob.Id));
    }

    [TestMethod]
    public async Task GetRecentImportRunsAsync_ReturnsMostRecentRunsWithFailureSummary()
    {
        await using var database = new JobSearchSqliteTestDatabase();
        await using var dbContext = database.CreateDbContext();

        var olderRun = JobRefreshRun.Start(
            providerName: "Adzuna",
            countryCode: "gb",
            requestFiltersJson: "{\"where\":\"london\"}",
            requestedPageSize: 50,
            requestedMaxPages: 1);
        olderRun.RecordFetchedPage(4);
        olderRun.RecordInsertedJobs(3);
        olderRun.RecordHiddenJobs(1);
        olderRun.Complete();

        await Task.Delay(20);

        var newerRun = JobRefreshRun.Start(
            providerName: "Adzuna",
            countryCode: "gb",
            requestFiltersJson: "{\"where\":\"manchester\"}",
            requestedPageSize: 25,
            requestedMaxPages: 1);
        newerRun.RecordFetchedPage(2);
        newerRun.RecordFailedItems(1);
        newerRun.Fail("Provider rate limit hit.");

        dbContext.JobRefreshRuns.AddRange(olderRun, newerRun);
        await dbContext.SaveChangesAsync();

        var queries = new JobSearchQueries(dbContext);

        var runs = await queries.GetRecentImportRunsAsync(limit: 10, cancellationToken: CancellationToken.None);

        Assert.HasCount(2, runs);
        Assert.AreEqual(newerRun.Id, runs[0].Id);
        Assert.AreEqual("Adzuna", runs[0].ProviderName);
        Assert.AreEqual(JobRefreshRunStatus.Failed.ToString(), runs[0].Status);
        Assert.AreEqual(2, runs[0].RecordsReceived);
        Assert.AreEqual(0, runs[0].RecordsInserted);
        Assert.AreEqual(1, runs[0].RecordsFailed);
        Assert.AreEqual("Provider rate limit hit.", runs[0].FailureSummary);
        Assert.AreEqual(olderRun.Id, runs[1].Id);
        Assert.AreEqual(JobRefreshRunStatus.Completed.ToString(), runs[1].Status);
        Assert.AreEqual(3, runs[1].RecordsInserted);
        Assert.AreEqual(1, runs[1].RecordsHidden);
        Assert.IsNull(runs[1].FailureSummary);
    }
}
