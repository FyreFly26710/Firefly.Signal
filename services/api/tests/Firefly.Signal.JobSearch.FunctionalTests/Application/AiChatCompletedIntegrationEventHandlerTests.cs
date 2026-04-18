using Firefly.Signal.EventBus.Events.Ai;
using Firefly.Signal.JobSearch.Application.IntegrationEventHandlers;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.FunctionalTests.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Firefly.Signal.JobSearch.FunctionalTests.Application;

[TestClass]
public sealed class AiChatCompletedIntegrationEventHandlerTests
{
    [TestMethod]
    public async Task HandleAsync_WhenMatchingDemoRunExists_PersistsResponseIdAndContent()
    {
        await using var database = new JobSearchSqliteTestDatabase();
        await using var dbContext = database.CreateDbContext();

        var job = JobSearchTestData.CreateJob("Demo Handler Role", postedAtUtc: DateTime.UtcNow.AddDays(-1));
        var demoRun = UserJobAiChatDemoRun.Start(
            userAccountId: 42,
            jobPostingId: job.Id,
            correlationId: "job-demo-corr",
            provider: "ChatGpt",
            model: "gpt-5.4-nano",
            prompt: "Demo prompt");

        dbContext.Jobs.Add(job);
        dbContext.UserJobAiChatDemoRuns.Add(demoRun);
        await dbContext.SaveChangesAsync();

        var handler = new AiChatCompletedIntegrationEventHandler(dbContext);

        await handler.HandleAsync(new AiChatCompletedIntegrationEvent
        {
            CorrelationId = "job-demo-corr",
            ResponseId = 555,
            ResponseContent = "Mock completion content"
        }, CancellationToken.None);

        var persistedRun = await dbContext.UserJobAiChatDemoRuns.SingleAsync();
        Assert.AreEqual(UserJobAiChatDemoStatus.Completed, persistedRun.Status);
        Assert.AreEqual(555L, persistedRun.AiResponseId);
        Assert.AreEqual("Mock completion content", persistedRun.AiResponseContent);
        Assert.IsNotNull(persistedRun.CompletedAtUtc);
    }
}
