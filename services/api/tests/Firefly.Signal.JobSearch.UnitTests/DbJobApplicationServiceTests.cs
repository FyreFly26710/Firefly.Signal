using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Firefly.Signal.JobSearch.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.UnitTests;

[TestClass]
public class DbJobApplicationServiceTests
{
    // ──────────────────────────────────────────────────────────
    // ApplyJobAsync
    // ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task ApplyJobAsync_returns_null_when_job_not_found()
    {
        await using var db = CreateDbContext();
        var service = new DbJobApplicationService(db);

        var result = await service.ApplyJobAsync(jobId: 999, userAccountId: 1, note: null);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task ApplyJobAsync_creates_application_with_initial_applied_entry()
    {
        await using var db = CreateDbContext();
        var job = AddJob(db);
        await db.SaveChangesAsync();

        var service = new DbJobApplicationService(db);
        var result = await service.ApplyJobAsync(job.Id, userAccountId: 1, note: "My note");

        Assert.IsNotNull(result);
        Assert.AreEqual(job.Id, result.JobPostingId);
        Assert.AreEqual("My note", result.Note);
        Assert.AreEqual(nameof(JobApplicationStatus.Applied), result.CurrentStatus);
        Assert.HasCount(1, result.StatusHistory);
        Assert.AreEqual(nameof(JobApplicationStatus.Applied), result.StatusHistory[0].Status);
        Assert.AreEqual(1, await db.JobApplications.CountAsync());
        Assert.AreEqual(1, await db.JobApplicationStatusEntries.CountAsync());
    }

    [TestMethod]
    public async Task ApplyJobAsync_is_idempotent_returns_existing_application()
    {
        await using var db = CreateDbContext();
        var job = AddJob(db);
        await db.SaveChangesAsync();

        var service = new DbJobApplicationService(db);
        var first = await service.ApplyJobAsync(job.Id, userAccountId: 1, note: null);
        var second = await service.ApplyJobAsync(job.Id, userAccountId: 1, note: null);

        Assert.AreEqual(first!.Id, second!.Id);
        Assert.AreEqual(1, await db.JobApplications.CountAsync());
        Assert.AreEqual(1, await db.JobApplicationStatusEntries.CountAsync());
    }

    // ──────────────────────────────────────────────────────────
    // AdvanceApplicationStatusAsync
    // ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task AdvanceApplicationStatusAsync_returns_null_when_no_application_exists()
    {
        await using var db = CreateDbContext();
        var service = new DbJobApplicationService(db);

        var result = await service.AdvanceApplicationStatusAsync(jobId: 999, userAccountId: 1, JobApplicationStatus.TelephoneInterview);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task AdvanceApplicationStatusAsync_adds_status_entry_and_updates_current_status()
    {
        await using var db = CreateDbContext();
        var job = AddJob(db);
        await db.SaveChangesAsync();

        var service = new DbJobApplicationService(db);
        await service.ApplyJobAsync(job.Id, userAccountId: 1, note: null);

        var result = await service.AdvanceApplicationStatusAsync(job.Id, userAccountId: 1, JobApplicationStatus.TelephoneInterview);

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(JobApplicationStatus.TelephoneInterview), result.CurrentStatus);
        Assert.HasCount(2, result.StatusHistory);
        Assert.AreEqual(2, await db.JobApplicationStatusEntries.CountAsync());
    }

    [TestMethod]
    public async Task AdvanceApplicationStatusAsync_allows_skipping_intermediate_stages()
    {
        await using var db = CreateDbContext();
        var job = AddJob(db);
        await db.SaveChangesAsync();

        var service = new DbJobApplicationService(db);
        await service.ApplyJobAsync(job.Id, userAccountId: 1, note: null);

        var result = await service.AdvanceApplicationStatusAsync(job.Id, userAccountId: 1, JobApplicationStatus.Offered);

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(JobApplicationStatus.Offered), result.CurrentStatus);
    }

    [TestMethod]
    public async Task AdvanceApplicationStatusAsync_throws_on_backwards_transition()
    {
        await using var db = CreateDbContext();
        var job = AddJob(db);
        await db.SaveChangesAsync();

        var service = new DbJobApplicationService(db);
        await service.ApplyJobAsync(job.Id, userAccountId: 1, note: null);
        await service.AdvanceApplicationStatusAsync(job.Id, userAccountId: 1, JobApplicationStatus.TelephoneInterview);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
            service.AdvanceApplicationStatusAsync(job.Id, userAccountId: 1, JobApplicationStatus.Applied));
    }

    [TestMethod]
    public async Task AdvanceApplicationStatusAsync_throws_when_status_is_already_rejected()
    {
        await using var db = CreateDbContext();
        var job = AddJob(db);
        await db.SaveChangesAsync();

        var service = new DbJobApplicationService(db);
        await service.ApplyJobAsync(job.Id, userAccountId: 1, note: null);
        await service.AdvanceApplicationStatusAsync(job.Id, userAccountId: 1, JobApplicationStatus.Rejected);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
            service.AdvanceApplicationStatusAsync(job.Id, userAccountId: 1, JobApplicationStatus.Offered));
    }

    // ──────────────────────────────────────────────────────────
    // UpdateApplicationNoteAsync
    // ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task UpdateApplicationNoteAsync_returns_null_when_no_application_exists()
    {
        await using var db = CreateDbContext();
        var service = new DbJobApplicationService(db);

        var result = await service.UpdateApplicationNoteAsync(jobId: 999, userAccountId: 1, note: "note");

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task UpdateApplicationNoteAsync_updates_note_on_existing_application()
    {
        await using var db = CreateDbContext();
        var job = AddJob(db);
        await db.SaveChangesAsync();

        var service = new DbJobApplicationService(db);
        await service.ApplyJobAsync(job.Id, userAccountId: 1, note: "original");

        var result = await service.UpdateApplicationNoteAsync(job.Id, userAccountId: 1, note: "updated");

        Assert.IsNotNull(result);
        Assert.AreEqual("updated", result.Note);
    }

    [TestMethod]
    public async Task UpdateApplicationNoteAsync_clears_note_when_null()
    {
        await using var db = CreateDbContext();
        var job = AddJob(db);
        await db.SaveChangesAsync();

        var service = new DbJobApplicationService(db);
        await service.ApplyJobAsync(job.Id, userAccountId: 1, note: "some note");

        var result = await service.UpdateApplicationNoteAsync(job.Id, userAccountId: 1, note: null);

        Assert.IsNotNull(result);
        Assert.IsNull(result.Note);
    }

    // ──────────────────────────────────────────────────────────
    // GetAppliedJobsAsync
    // ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetAppliedJobsAsync_returns_empty_when_user_has_no_applications()
    {
        await using var db = CreateDbContext();
        var service = new DbJobApplicationService(db);

        var result = await service.GetAppliedJobsAsync(userAccountId: 42);

        Assert.IsEmpty(result.ToList());
    }

    [TestMethod]
    public async Task GetAppliedJobsAsync_returns_all_applied_jobs_for_user()
    {
        await using var db = CreateDbContext();
        var job1 = AddJob(db);
        var job2 = AddJob(db);
        await db.SaveChangesAsync();

        var service = new DbJobApplicationService(db);
        await service.ApplyJobAsync(job1.Id, userAccountId: 1, note: null);
        await service.ApplyJobAsync(job2.Id, userAccountId: 1, note: null);

        var result = await service.GetAppliedJobsAsync(userAccountId: 1);

        Assert.HasCount(2, result);
    }

    [TestMethod]
    public async Task GetAppliedJobsAsync_reflects_latest_status_after_advance()
    {
        await using var db = CreateDbContext();
        var job = AddJob(db);
        await db.SaveChangesAsync();

        var service = new DbJobApplicationService(db);
        await service.ApplyJobAsync(job.Id, userAccountId: 1, note: null);
        await service.AdvanceApplicationStatusAsync(job.Id, userAccountId: 1, JobApplicationStatus.TelephoneInterview);

        var result = await service.GetAppliedJobsAsync(userAccountId: 1);

        Assert.HasCount(1, result);
        Assert.AreEqual(nameof(JobApplicationStatus.TelephoneInterview), result[0].CurrentStatus);
    }

    [TestMethod]
    public async Task GetAppliedJobsAsync_does_not_return_jobs_from_other_users()
    {
        await using var db = CreateDbContext();
        var job = AddJob(db);
        await db.SaveChangesAsync();

        var service = new DbJobApplicationService(db);
        await service.ApplyJobAsync(job.Id, userAccountId: 1, note: null);

        var result = await service.GetAppliedJobsAsync(userAccountId: 2);

        Assert.IsEmpty(result.ToList());
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
