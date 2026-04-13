using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Firefly.Signal.JobSearch.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.UnitTests;

[TestClass]
public class DbUserJobStateServiceTests
{
    // ──────────────────────────────────────────────────────────
    // SaveJobAsync
    // ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task SaveJobAsync_returns_null_when_job_not_found()
    {
        await using var db = CreateDbContext();
        var service = new DbUserJobStateService(db);

        var result = await service.SaveJobAsync(jobId: 999, userAccountId: 1);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task SaveJobAsync_creates_user_job_state_with_is_saved_true()
    {
        await using var db = CreateDbContext();
        var job = AddJob(db);
        await db.SaveChangesAsync();

        var service = new DbUserJobStateService(db);
        var result = await service.SaveJobAsync(job.Id, userAccountId: 1);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsSaved);
        Assert.AreEqual(1, await db.UserJobStates.CountAsync());
    }

    [TestMethod]
    public async Task SaveJobAsync_is_idempotent()
    {
        await using var db = CreateDbContext();
        var job = AddJob(db);
        await db.SaveChangesAsync();

        var service = new DbUserJobStateService(db);
        await service.SaveJobAsync(job.Id, userAccountId: 1);
        await service.SaveJobAsync(job.Id, userAccountId: 1);

        Assert.AreEqual(1, await db.UserJobStates.CountAsync());
        Assert.IsTrue((await db.UserJobStates.SingleAsync()).IsSaved);
    }

    // ──────────────────────────────────────────────────────────
    // UnsaveJobAsync
    // ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task UnsaveJobAsync_returns_null_when_job_not_found()
    {
        await using var db = CreateDbContext();
        var service = new DbUserJobStateService(db);

        var result = await service.UnsaveJobAsync(jobId: 999, userAccountId: 1);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task UnsaveJobAsync_sets_IsSaved_false()
    {
        await using var db = CreateDbContext();
        var job = AddJob(db);
        await db.SaveChangesAsync();

        var service = new DbUserJobStateService(db);
        await service.SaveJobAsync(job.Id, userAccountId: 1);
        var result = await service.UnsaveJobAsync(job.Id, userAccountId: 1);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsSaved);
    }

    // ──────────────────────────────────────────────────────────
    // HideJobForUserAsync
    // ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task HideJobForUserAsync_returns_null_when_job_not_found()
    {
        await using var db = CreateDbContext();
        var service = new DbUserJobStateService(db);

        var result = await service.HideJobForUserAsync(jobId: 999, userAccountId: 1);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task HideJobForUserAsync_sets_IsHidden_true()
    {
        await using var db = CreateDbContext();
        var job = AddJob(db);
        await db.SaveChangesAsync();

        var service = new DbUserJobStateService(db);
        var result = await service.HideJobForUserAsync(job.Id, userAccountId: 1);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsHidden);
    }

    [TestMethod]
    public async Task HideJobForUserAsync_is_idempotent()
    {
        await using var db = CreateDbContext();
        var job = AddJob(db);
        await db.SaveChangesAsync();

        var service = new DbUserJobStateService(db);
        await service.HideJobForUserAsync(job.Id, userAccountId: 1);
        await service.HideJobForUserAsync(job.Id, userAccountId: 1);

        Assert.AreEqual(1, await db.UserJobStates.CountAsync());
        Assert.IsTrue((await db.UserJobStates.SingleAsync()).IsHidden);
    }

    // ──────────────────────────────────────────────────────────
    // UnhideJobForUserAsync
    // ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task UnhideJobForUserAsync_sets_IsHidden_false()
    {
        await using var db = CreateDbContext();
        var job = AddJob(db);
        await db.SaveChangesAsync();

        var service = new DbUserJobStateService(db);
        await service.HideJobForUserAsync(job.Id, userAccountId: 1);
        var result = await service.UnhideJobForUserAsync(job.Id, userAccountId: 1);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsHidden);
    }

    // ──────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────

    private static JobSearchDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<JobSearchDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        return new JobSearchDbContext(options);
    }

    private static JobPosting AddJob(JobSearchDbContext db)
    {
        var job = JobPosting.Create(
            title: "Software Engineer",
            company: "Acme Ltd",
            postcode: "SW1A 1AA",
            locationName: "London",
            summary: "Build things.",
            url: "https://example.com/jobs/1",
            sourceName: "Adzuna",
            isRemote: false,
            postedAtUtc: new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        db.Jobs.Add(job);
        return job;
    }
}
