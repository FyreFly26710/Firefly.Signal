using Firefly.Signal.JobSearch.Application.Commands;
using Firefly.Signal.JobSearch.Application.Exceptions;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.FunctionalTests.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Firefly.Signal.JobSearch.FunctionalTests.Application;

[TestClass]
public sealed class AdvanceApplicationStatusCommandHandlerTests
{
    [TestMethod]
    public async Task Handle_WhenCurrentStatusIsRejected_ThrowsInvalidTransition()
    {
        await using var database = new JobSearchSqliteTestDatabase();
        await using var dbContext = database.CreateDbContext();

        var job = JobSearchTestData.CreateJob("Senior Platform Engineer", postedAtUtc: DateTime.UtcNow.AddDays(-2));
        var application = JobApplication.Create(userAccountId: 42, jobPostingId: job.Id, note: "Already rejected");
        var rejectedEntry = JobApplicationStatusEntry.Create(application.Id, JobApplicationStatus.Rejected, DateTime.UtcNow.AddDays(-1));

        dbContext.Jobs.Add(job);
        dbContext.JobApplications.Add(application);
        dbContext.JobApplicationStatusEntries.Add(rejectedEntry);
        await dbContext.SaveChangesAsync();

        var handler = new AdvanceApplicationStatusCommandHandler(dbContext);

        await Assert.ThrowsExactlyAsync<InvalidApplicationStatusTransitionException>(() =>
            handler.Handle(
                new AdvanceApplicationStatusCommand(job.Id, UserAccountId: 42, NewStatus: JobApplicationStatus.Offered),
                CancellationToken.None));
    }

    [TestMethod]
    public async Task Handle_WhenStatusAdvances_AppendsEntryAndReturnsUpdatedResponse()
    {
        await using var database = new JobSearchSqliteTestDatabase();
        await using var dbContext = database.CreateDbContext();

        var job = JobSearchTestData.CreateJob("Backend Engineer", postedAtUtc: DateTime.UtcNow.AddDays(-1));
        var application = JobApplication.Create(userAccountId: 42, jobPostingId: job.Id, note: "Phone screen booked");

        dbContext.Jobs.Add(job);
        dbContext.JobApplications.Add(application);
        await dbContext.SaveChangesAsync();

        var handler = new AdvanceApplicationStatusCommandHandler(dbContext);

        var response = await handler.Handle(
            new AdvanceApplicationStatusCommand(job.Id, UserAccountId: 42, NewStatus: JobApplicationStatus.TelephoneInterview),
            CancellationToken.None);

        Assert.IsNotNull(response);
        Assert.AreEqual(JobApplicationStatus.TelephoneInterview.ToString(), response.CurrentStatus);
        Assert.HasCount(1, response.StatusHistory);
        Assert.AreEqual(JobApplicationStatus.TelephoneInterview.ToString(), response.StatusHistory[0].Status);

        var persistedStatuses = await dbContext.JobApplicationStatusEntries
            .Where(entry => entry.JobApplicationId == application.Id)
            .ToListAsync();

        Assert.HasCount(1, persistedStatuses);
        Assert.AreEqual(JobApplicationStatus.TelephoneInterview, persistedStatuses[0].Status);
    }
}
