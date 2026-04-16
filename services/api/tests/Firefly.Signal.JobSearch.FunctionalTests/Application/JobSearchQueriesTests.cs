using Firefly.Signal.JobSearch.Application.Queries;
using Firefly.Signal.JobSearch.Contracts.Requests;
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
}
