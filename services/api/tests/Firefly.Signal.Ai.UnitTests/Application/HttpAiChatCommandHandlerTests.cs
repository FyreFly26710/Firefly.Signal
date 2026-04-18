using Firefly.Signal.Ai.Api.Application.Commands;
using Firefly.Signal.Ai.Domain;
using Firefly.Signal.Ai.Infrastructure.AiProviders;
using Firefly.Signal.Ai.UnitTests.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Firefly.Signal.Ai.UnitTests.Application;

[TestClass]
public sealed class HttpAiChatCommandHandlerTests
{
    private static AiProviderResolver BuildResolver(IAiChatProvider chatGpt, IAiChatProvider deepSeek)
    {
        var sp = new ServiceCollection()
            .AddKeyedSingleton(AiProvider.ChatGpt, chatGpt)
            .AddKeyedSingleton(AiProvider.DeepSeek, deepSeek)
            .BuildServiceProvider();
        return new AiProviderResolver(sp);
    }

    private static IAiChatProvider StubProvider(string content = "Response", int? promptTokens = 10, int? completionTokens = 5)
    {
        var provider = Substitute.For<IAiChatProvider>();
        provider.CompleteAsync(Arg.Any<AiProviderRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new AiProviderResponse(content, promptTokens, completionTokens)));
        return provider;
    }

    [TestMethod]
    public async Task Handle_WhenProviderSucceeds_PersistsRequestAndResponseToDb()
    {
        await using var database = new AiSqliteTestDatabase();
        await using var db = database.CreateDbContext();

        var handler = new HttpAiChatCommandHandler(db, BuildResolver(StubProvider(), Substitute.For<IAiChatProvider>()));

        await handler.Handle(new HttpAiChatCommand
        {
            Provider = AiProvider.ChatGpt,
            Model = "gpt-4o",
            UserPrompt = "Tell me about .NET 10"
        }, CancellationToken.None);

        Assert.HasCount(1, db.AiRequests.ToList());
        Assert.HasCount(1, db.AiResponses.ToList());
    }

    [TestMethod]
    public async Task Handle_WhenProviderSucceeds_ReturnsResponseWithCorrectContent()
    {
        await using var database = new AiSqliteTestDatabase();
        await using var db = database.CreateDbContext();

        var handler = new HttpAiChatCommandHandler(
            db,
            BuildResolver(StubProvider("GPT answer here", 20, 8), Substitute.For<IAiChatProvider>()));

        var response = await handler.Handle(new HttpAiChatCommand
        {
            Provider = AiProvider.ChatGpt,
            Model = "gpt-4o",
            UserPrompt = "What is .NET?"
        }, CancellationToken.None);

        Assert.AreEqual("GPT answer here", response.Content);
        Assert.AreEqual(20, response.PromptTokens);
        Assert.AreEqual(8, response.CompletionTokens);
        Assert.AreEqual(28, response.TotalTokens);
    }

    [TestMethod]
    public async Task Handle_WhenProviderSucceeds_SetsRequestStatusToCompleted()
    {
        await using var database = new AiSqliteTestDatabase();
        await using var db = database.CreateDbContext();

        var handler = new HttpAiChatCommandHandler(db, BuildResolver(StubProvider(), Substitute.For<IAiChatProvider>()));

        await handler.Handle(new HttpAiChatCommand
        {
            Provider = AiProvider.ChatGpt,
            Model = "gpt-4o",
            UserPrompt = "ping"
        }, CancellationToken.None);

        var persisted = db.AiRequests.Single();
        Assert.AreEqual(AiRequestStatus.Completed, persisted.Status);
        Assert.IsNotNull(persisted.CompletedAtUtc);
    }

    [TestMethod]
    public async Task Handle_WhenProviderThrows_SetsRequestStatusToFailed()
    {
        await using var database = new AiSqliteTestDatabase();
        await using var db = database.CreateDbContext();

        var provider = Substitute.For<IAiChatProvider>();
        provider.CompleteAsync(Arg.Any<AiProviderRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<AiProviderResponse>(new InvalidOperationException("Provider is down")));

        var handler = new HttpAiChatCommandHandler(db, BuildResolver(provider, Substitute.For<IAiChatProvider>()));

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
            handler.Handle(new HttpAiChatCommand
            {
                Provider = AiProvider.ChatGpt,
                Model = "gpt-4o",
                UserPrompt = "failing call"
            }, CancellationToken.None));

        var persisted = db.AiRequests.Single();
        Assert.AreEqual(AiRequestStatus.Failed, persisted.Status);
        Assert.AreEqual("Provider is down", persisted.FailureSummary);
    }

    [TestMethod]
    public async Task Handle_WithSystemPromptId_PassesSystemMessageToProvider()
    {
        await using var database = new AiSqliteTestDatabase();
        await using var db = database.CreateDbContext();

        var systemMsg = AiTestData.SystemPrompt("Analyse the job listings.");
        db.AiMessages.Add(systemMsg);
        await db.SaveChangesAsync();

        AiProviderRequest? capturedRequest = null;
        var provider = Substitute.For<IAiChatProvider>();
        provider.CompleteAsync(Arg.Do<AiProviderRequest>(r => capturedRequest = r), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new AiProviderResponse("Analysis done", 5, 3)));

        var handler = new HttpAiChatCommandHandler(db, BuildResolver(provider, Substitute.For<IAiChatProvider>()));

        await handler.Handle(new HttpAiChatCommand
        {
            Provider = AiProvider.ChatGpt,
            Model = "gpt-4o",
            SystemPromptMessageId = systemMsg.Id,
            UserPrompt = "Analyse these jobs"
        }, CancellationToken.None);

        Assert.IsNotNull(capturedRequest);
        var systemMessages = capturedRequest.Messages.Where(m => m.Role == "system").ToList();
        Assert.HasCount(1, systemMessages);
        Assert.AreEqual("Analyse the job listings.", systemMessages[0].Content);
    }

    [TestMethod]
    public async Task Handle_WithDeepSeekProvider_RoutesToDeepSeekProvider()
    {
        await using var database = new AiSqliteTestDatabase();
        await using var db = database.CreateDbContext();

        var chatGpt = Substitute.For<IAiChatProvider>();
        var deepSeek = Substitute.For<IAiChatProvider>();
        deepSeek.CompleteAsync(Arg.Any<AiProviderRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new AiProviderResponse("DeepSeek reply", 4, 2)));

        var handler = new HttpAiChatCommandHandler(db, BuildResolver(chatGpt, deepSeek));

        await handler.Handle(new HttpAiChatCommand
        {
            Provider = AiProvider.DeepSeek,
            Model = "deepseek-chat",
            UserPrompt = "Hello"
        }, CancellationToken.None);

        await chatGpt.DidNotReceive().CompleteAsync(Arg.Any<AiProviderRequest>(), Arg.Any<CancellationToken>());
        await deepSeek.Received(1).CompleteAsync(Arg.Any<AiProviderRequest>(), Arg.Any<CancellationToken>());
    }
}
