using Firefly.Signal.Ai.Api.Application;
using Firefly.Signal.Ai.Api.Contracts.Requests;
using Firefly.Signal.Ai.Domain;
using Firefly.Signal.Ai.UnitTests.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Firefly.Signal.Ai.UnitTests.Application;

[TestClass]
public sealed class AiChatMessageBuilderTests
{
    [TestMethod]
    public async Task BuildAsync_WithSystemPromptId_IncludesSystemMessageFirst()
    {
        await using var database = new AiSqliteTestDatabase();
        await using var db = database.CreateDbContext();

        var systemMsg = AiTestData.SystemPrompt("You are a job analysis assistant.");
        db.AiMessages.Add(systemMsg);
        await db.SaveChangesAsync();

        var messages = await AiChatMessageBuilder.BuildAsync(db, systemMsg.Id, [], null, CancellationToken.None);

        Assert.HasCount(1, messages);
        Assert.AreEqual("system", messages[0].Role);
        Assert.AreEqual("You are a job analysis assistant.", messages[0].Content);
    }

    [TestMethod]
    public async Task BuildAsync_WithUnknownSystemPromptId_OmitsSystemMessage()
    {
        await using var database = new AiSqliteTestDatabase();
        await using var db = database.CreateDbContext();

        var messages = await AiChatMessageBuilder.BuildAsync(db, 999999L, [], "Hello", CancellationToken.None);

        Assert.HasCount(1, messages);
        Assert.AreEqual("user", messages[0].Role);
    }

    [TestMethod]
    public async Task BuildAsync_WithHistory_AppendsPriorTurnsBeforeCurrentUserMessage()
    {
        await using var database = new AiSqliteTestDatabase();
        await using var db = database.CreateDbContext();

        // Seed two prior requests (with user prompt messages) and their responses
        var req1 = AiRequest.QueueHttp("gpt-4o", null, "Turn 1 user");
        var req2 = AiRequest.QueueHttp("gpt-4o", null, "Turn 2 user");
        db.AiRequests.AddRange(req1, req2);
        await db.SaveChangesAsync();

        var resp1 = AiResponse.Create(req1.Id, "Turn 1 assistant");
        var resp2 = AiResponse.Create(req2.Id, "Turn 2 assistant");
        db.AiResponses.AddRange(resp1, resp2);
        await db.SaveChangesAsync();

        var history = new[]
        {
            new AiChatHistoryItem(req1.Id, resp1.Id),
            new AiChatHistoryItem(req2.Id, resp2.Id)
        };

        var messages = await AiChatMessageBuilder.BuildAsync(db, null, history, "Current question", CancellationToken.None);

        Assert.AreEqual("user", messages[0].Role);
        Assert.AreEqual("Turn 1 user", messages[0].Content);
        Assert.AreEqual("assistant", messages[1].Role);
        Assert.AreEqual("Turn 1 assistant", messages[1].Content);
        Assert.AreEqual("user", messages[2].Role);
        Assert.AreEqual("Turn 2 user", messages[2].Content);
        Assert.AreEqual("assistant", messages[3].Role);
        Assert.AreEqual("Turn 2 assistant", messages[3].Content);
        Assert.AreEqual("user", messages[^1].Role);
        Assert.AreEqual("Current question", messages[^1].Content);
    }

    [TestMethod]
    public async Task BuildAsync_WithCurrentUserPromptOnly_ReturnsSingleUserMessage()
    {
        await using var database = new AiSqliteTestDatabase();
        await using var db = database.CreateDbContext();

        var messages = await AiChatMessageBuilder.BuildAsync(db, null, [], "What jobs match my CV?", CancellationToken.None);

        Assert.HasCount(1, messages);
        Assert.AreEqual("user", messages[0].Role);
        Assert.AreEqual("What jobs match my CV?", messages[0].Content);
    }

    [TestMethod]
    public async Task BuildAsync_WithNullSystemAndNullUserPrompt_ReturnsEmptyList()
    {
        await using var database = new AiSqliteTestDatabase();
        await using var db = database.CreateDbContext();

        var messages = await AiChatMessageBuilder.BuildAsync(db, null, [], null, CancellationToken.None);

        Assert.IsEmpty(messages);
    }

    [TestMethod]
    public async Task BuildAsync_WithSystemAndUserPrompt_OrdersSystemFirst()
    {
        await using var database = new AiSqliteTestDatabase();
        await using var db = database.CreateDbContext();

        var systemMsg = AiTestData.SystemPrompt("Be concise.");
        db.AiMessages.Add(systemMsg);
        await db.SaveChangesAsync();

        var messages = await AiChatMessageBuilder.BuildAsync(db, systemMsg.Id, [], "Quick question", CancellationToken.None);

        Assert.HasCount(2, messages);
        Assert.AreEqual("system", messages[0].Role);
        Assert.AreEqual("user", messages[1].Role);
    }
}
