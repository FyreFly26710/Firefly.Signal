using Firefly.Signal.EventBus;
using Firefly.Signal.EventBus.Events.Ai;
using Firefly.Signal.JobSearch.Api.Options;
using Firefly.Signal.JobSearch.Application.Commands;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.FunctionalTests.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Firefly.Signal.JobSearch.FunctionalTests.Application;

[TestClass]
public sealed class StartUserJobAiChatDemoCommandHandlerTests
{
    [TestMethod]
    public async Task Handle_WhenJobExists_PersistsPendingRunAndPublishesAiRequestEvent()
    {
        await using var database = new JobSearchSqliteTestDatabase();
        await using var dbContext = database.CreateDbContext();

        var job = JobSearchTestData.CreateJob("Demo AI Role", postedAtUtc: DateTime.UtcNow.AddDays(-1));
        dbContext.Jobs.Add(job);
        await dbContext.SaveChangesAsync();

        var eventBus = Substitute.For<IEventBus>();
        AiChatRequestedIntegrationEvent? publishedEvent = null;
        eventBus.PublishAsync(Arg.Do<AiChatRequestedIntegrationEvent>(e => publishedEvent = e), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var handler = new StartUserJobAiChatDemoCommandHandler(
            dbContext,
            eventBus,
            Options.Create(new DemoAiChatOptions
            {
                Provider = "ChatGpt",
                Model = "gpt-5.4-nano"
            }));

        var response = await handler.Handle(new StartUserJobAiChatDemoCommand(job.Id, UserAccountId: 42), CancellationToken.None);

        Assert.IsNotNull(response);
        Assert.AreEqual("Pending", response.Status);
        Assert.IsNotNull(publishedEvent);
        Assert.AreEqual("ChatGpt", publishedEvent.Provider);
        Assert.AreEqual("gpt-5.4-nano", publishedEvent.Model);
        StringAssert.StartsWith(publishedEvent.CorrelationId, "job-demo-");
        StringAssert.Contains(publishedEvent.UserPrompt, "Demo AI Role");

        var persistedRun = await dbContext.UserJobAiChatDemoRuns.SingleAsync();
        Assert.AreEqual(UserJobAiChatDemoStatus.Pending, persistedRun.Status);
        Assert.AreEqual(publishedEvent.CorrelationId, persistedRun.CorrelationId);
        Assert.AreEqual(42L, persistedRun.UserAccountId);
        Assert.AreEqual(job.Id, persistedRun.JobPostingId);
    }
}
