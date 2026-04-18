using Firefly.Signal.Ai.Api.Application.Commands;
using Firefly.Signal.Ai.Domain;
using Firefly.Signal.Ai.Infrastructure.AiProviders;
using Firefly.Signal.Ai.UnitTests.Testing;
using Firefly.Signal.EventBus;
using Firefly.Signal.EventBus.Events.Ai;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Firefly.Signal.Ai.UnitTests.Application;

[TestClass]
public sealed class MqAiChatEventCommandHandlerTests
{
    private static AiProviderResolver BuildResolver(IAiChatProvider chatGpt, IAiChatProvider deepSeek)
    {
        var sp = new ServiceCollection()
            .AddKeyedSingleton(AiProvider.ChatGpt, chatGpt)
            .AddKeyedSingleton(AiProvider.DeepSeek, deepSeek)
            .BuildServiceProvider();
        return new AiProviderResolver(sp);
    }

    private static MqAiChatEventCommand BuildCommand(long? systemPromptMessageId, string userPrompt = "Analyse these jobs") =>
        new()
        {
            CorrelationId = "corr-123",
            CallerService = "job-search-api",
            ReplyEventType = "JobAiAnalysisCompletedIntegrationEvent",
            Provider = AiProvider.ChatGpt,
            Model = "gpt-4o",
            SystemPromptMessageId = systemPromptMessageId,
            UserPrompt = userPrompt
        };

    [TestMethod]
    public async Task Handle_WhenProviderSucceeds_PublishesCompletedEventWithCorrelationId()
    {
        await using var database = new AiSqliteTestDatabase();
        await using var db = database.CreateDbContext();

        var systemMsg = AiTestData.SystemPrompt("You are a job analyst.");
        db.AiMessages.Add(systemMsg);
        await db.SaveChangesAsync();

        var provider = Substitute.For<IAiChatProvider>();
        provider.CompleteAsync(Arg.Any<AiProviderRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new AiProviderResponse("Job analysis result", 15, 7)));

        var eventBus = Substitute.For<IEventBus>();
        var handler = new MqAiChatEventCommandHandler(db, BuildResolver(provider, Substitute.For<IAiChatProvider>()), eventBus);

        await handler.Handle(BuildCommand(systemMsg.Id), CancellationToken.None);

        await eventBus.Received(1).PublishAsync(
            Arg.Is<AiChatCompletedIntegrationEvent>(e =>
                e.CorrelationId == "corr-123" &&
                e.ResponseContent == "Job analysis result"),
            Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Handle_WhenProviderSucceeds_PublishedEventContainsPersistentResponseId()
    {
        await using var database = new AiSqliteTestDatabase();
        await using var db = database.CreateDbContext();

        var systemMsg = AiTestData.SystemPrompt("You are a job analyst.");
        db.AiMessages.Add(systemMsg);
        await db.SaveChangesAsync();

        var provider = Substitute.For<IAiChatProvider>();
        provider.CompleteAsync(Arg.Any<AiProviderRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new AiProviderResponse("Result", 5, 2)));

        AiChatCompletedIntegrationEvent? published = null;
        var eventBus = Substitute.For<IEventBus>();
        eventBus.PublishAsync(Arg.Do<AiChatCompletedIntegrationEvent>(e => published = e), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var handler = new MqAiChatEventCommandHandler(db, BuildResolver(provider, Substitute.For<IAiChatProvider>()), eventBus);
        await handler.Handle(BuildCommand(systemMsg.Id), CancellationToken.None);

        Assert.IsNotNull(published);
        Assert.AreNotEqual(0L, published.ResponseId);
        var persistedResponse = db.AiResponses.SingleOrDefault(r => r.Id == published.ResponseId);
        Assert.IsNotNull(persistedResponse);
    }

    [TestMethod]
    public async Task Handle_WhenProviderSucceeds_PersistsMqRequestWithCorrectFields()
    {
        await using var database = new AiSqliteTestDatabase();
        await using var db = database.CreateDbContext();

        var systemMsg = AiTestData.SystemPrompt("System context here.");
        db.AiMessages.Add(systemMsg);
        await db.SaveChangesAsync();

        var provider = Substitute.For<IAiChatProvider>();
        provider.CompleteAsync(Arg.Any<AiProviderRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new AiProviderResponse("Reply", null, null)));

        var handler = new MqAiChatEventCommandHandler(
            db, BuildResolver(provider, Substitute.For<IAiChatProvider>()), Substitute.For<IEventBus>());

        await handler.Handle(BuildCommand(systemMsg.Id), CancellationToken.None);

        var request = db.AiRequests.Single();
        Assert.AreEqual(AiRequestStatus.Completed, request.Status);
        Assert.AreEqual(AiRequestSource.MqEvent, request.Source);
        Assert.AreEqual("corr-123", request.CorrelationId);
        Assert.AreEqual("job-search-api", request.CallerService);
    }

    [TestMethod]
    public async Task Handle_WhenProviderThrows_SetsRequestToFailedAndDoesNotPublish()
    {
        await using var database = new AiSqliteTestDatabase();
        await using var db = database.CreateDbContext();

        var systemMsg = AiTestData.SystemPrompt("Context.");
        db.AiMessages.Add(systemMsg);
        await db.SaveChangesAsync();

        var provider = Substitute.For<IAiChatProvider>();
        provider.CompleteAsync(Arg.Any<AiProviderRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<AiProviderResponse>(new HttpRequestException("Provider unavailable")));

        var eventBus = Substitute.For<IEventBus>();
        var handler = new MqAiChatEventCommandHandler(
            db, BuildResolver(provider, Substitute.For<IAiChatProvider>()), eventBus);

        await Assert.ThrowsExactlyAsync<HttpRequestException>(() =>
            handler.Handle(BuildCommand(systemMsg.Id), CancellationToken.None));

        var request = db.AiRequests.Single();
        Assert.AreEqual(AiRequestStatus.Failed, request.Status);
        await eventBus.DidNotReceive()
            .PublishAsync(Arg.Any<AiChatCompletedIntegrationEvent>(), Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Handle_WhenTokenCountsAvailable_IncludesThemInPublishedEvent()
    {
        await using var database = new AiSqliteTestDatabase();
        await using var db = database.CreateDbContext();

        var systemMsg = AiTestData.SystemPrompt("Be concise.");
        db.AiMessages.Add(systemMsg);
        await db.SaveChangesAsync();

        var provider = Substitute.For<IAiChatProvider>();
        provider.CompleteAsync(Arg.Any<AiProviderRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new AiProviderResponse("Short reply", 100, 25)));

        AiChatCompletedIntegrationEvent? published = null;
        var eventBus = Substitute.For<IEventBus>();
        eventBus.PublishAsync(Arg.Do<AiChatCompletedIntegrationEvent>(e => published = e), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var handler = new MqAiChatEventCommandHandler(
            db, BuildResolver(provider, Substitute.For<IAiChatProvider>()), eventBus);
        await handler.Handle(BuildCommand(systemMsg.Id), CancellationToken.None);

        Assert.IsNotNull(published);
        Assert.AreEqual(100, published.PromptTokens);
        Assert.AreEqual(25, published.CompletionTokens);
    }
}
