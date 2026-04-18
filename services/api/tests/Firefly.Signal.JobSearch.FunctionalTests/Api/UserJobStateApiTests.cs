using System.Net;
using System.Net.Http.Json;
using Firefly.Signal.EventBus;
using Firefly.Signal.EventBus.Events.Ai;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.FunctionalTests.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Firefly.Signal.JobSearch.FunctionalTests.Api;

[TestClass]
public sealed class UserJobStateApiTests
{
    [TestMethod]
    public async Task StartDemoAiChat_WhenUnauthenticated_ReturnsUnauthorized()
    {
        using var factory = new JobSearchApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync("/api/job-search/jobs/999/demo-ai-chat", content: null);

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task StartDemoAiChat_WhenJobExists_ReturnsOkAndPersistsPendingRun()
    {
        var eventBus = Substitute.For<IEventBus>();
        AiChatRequestedIntegrationEvent? publishedEvent = null;
        eventBus.PublishAsync(Arg.Do<AiChatRequestedIntegrationEvent>(e => publishedEvent = e), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        using var factory = new JobSearchApiFactory(eventBus);
        var job = JobSearchTestData.CreateJob("API Demo Role", postedAtUtc: DateTime.UtcNow.AddDays(-1));
        await factory.SeedAsync(job);

        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PostAsync($"/api/job-search/jobs/{job.Id}/demo-ai-chat", content: null);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<UserJobAiChatDemoResponse>();

        Assert.IsNotNull(body);
        Assert.AreEqual(job.Id, body.JobPostingId);
        Assert.AreEqual("Pending", body.Status);
        Assert.IsNotNull(publishedEvent);

        var persisted = await factory.ExecuteDbContextAsync(async dbContext =>
            await dbContext.UserJobAiChatDemoRuns.SingleAsync());

        Assert.AreEqual(body.Id, persisted.Id);
        Assert.AreEqual(publishedEvent.CorrelationId, persisted.CorrelationId);
    }
}
