using Firefly.Signal.JobSearch.Application.Queries;
using Firefly.Signal.JobSearch.Contracts.Requests;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.FunctionalTests.Testing;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Firefly.Signal.SharedKernel.Models;
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
    public async Task GetRecentImportRunsAsync_ReturnsPagedRecentRunsWithFailureSummary()
    {
        await using var database = new JobSearchSqliteTestDatabase();
        await using var dbContext = database.CreateDbContext();

        var oldestRun = JobRefreshRun.Start(
            providerName: "Adzuna",
            countryCode: "gb",
            requestFiltersJson: "{\"where\":\"bristol\"}",
            requestedPageSize: 10,
            requestedMaxPages: 1);
        oldestRun.RecordFetchedPage(1);
        oldestRun.RecordInsertedJobs(1);
        oldestRun.Complete();

        await Task.Delay(20);

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
            requestFiltersJson: "{\"sortBy\":null,\"keyword\":\"Developer\",\"pageSize\":50,\"postcode\":\"E15\",\"provider\":0,\"pageIndex\":1,\"titleOnly\":false,\"distanceKilometers\":null}",
            requestedPageSize: 25,
            requestedMaxPages: 1);
        newerRun.RecordFetchedPage(2);
        newerRun.RecordFailedItems(1);
        newerRun.Fail("Provider rate limit hit.");

        dbContext.JobRefreshRuns.AddRange(olderRun, newerRun);
        dbContext.JobRefreshRuns.Add(oldestRun);
        await dbContext.SaveChangesAsync();

        var queries = new JobSearchQueries(dbContext);

        var runs = await queries.GetRecentImportRunsAsync(
            new PagedRequest(PageIndex: 0, PageSize: 2),
            cancellationToken: CancellationToken.None);

        Assert.AreEqual(0, runs.PageIndex);
        Assert.AreEqual(2, runs.PageSize);
        Assert.AreEqual(3L, runs.TotalCount);
        Assert.HasCount(2, runs.Items);
        Assert.AreEqual(newerRun.Id, runs.Items[0].Id);
        Assert.AreEqual("Adzuna", runs.Items[0].ProviderName);
        Assert.AreEqual(JobRefreshRunStatus.Failed.ToString(), runs.Items[0].Status);
        Assert.AreEqual("{\"keyword\":\"Developer\",\"pageSize\":50,\"postcode\":\"E15\",\"provider\":0,\"pageIndex\":1,\"titleOnly\":false}", runs.Items[0].JsonFilter);
        Assert.AreEqual(2, runs.Items[0].RecordsReceived);
        Assert.AreEqual(0, runs.Items[0].RecordsInserted);
        Assert.AreEqual(1, runs.Items[0].RecordsFailed);
        Assert.AreEqual("Provider rate limit hit.", runs.Items[0].FailureSummary);
        Assert.AreEqual(olderRun.Id, runs.Items[1].Id);
        Assert.AreEqual(JobRefreshRunStatus.Completed.ToString(), runs.Items[1].Status);
        Assert.AreEqual("{\"where\":\"london\"}", runs.Items[1].JsonFilter);
        Assert.AreEqual(3, runs.Items[1].RecordsInserted);
        Assert.AreEqual(1, runs.Items[1].RecordsHidden);
        Assert.IsNull(runs.Items[1].FailureSummary);
    }
}
