using System.Net;
using System.Net.Http.Json;
using Firefly.Signal.Ai.Api.Contracts.Requests;
using Firefly.Signal.Ai.Api.Contracts.Responses;
using Firefly.Signal.Ai.Domain;
using Firefly.Signal.Ai.FunctionalTests.Testing;
using Firefly.Signal.Ai.Infrastructure.AiProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Firefly.Signal.Ai.FunctionalTests.Api;

[TestClass]
public sealed class AiApiTests
{
    [TestMethod]
    public async Task PostChat_ReturnsOkAndPersistsCompletedResponse()
    {
        var provider = Substitute.For<IAiChatProvider>();
        provider.CompleteAsync(Arg.Any<AiProviderRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new AiProviderResponse("Helpful answer", 12, 8)));

        using var factory = new AiApiFactory(chatGptProvider: provider);
        var systemPrompt = new AiMessage(AiMessageType.SystemPrompt, "You are helpful.");
        await factory.SeedAsync(systemPrompt);

        using var client = factory.CreateClient();
        var request = new AiChatRequest
        {
            Provider = AiProvider.ChatGpt,
            Model = "gpt-5.4-nano",
            SystemPromptMessageId = systemPrompt.Id,
            UserPrompt = "Say hello"
        };

        var response = await client.PostAsJsonAsync("/api/ai/chats", request);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<AiChatResponse>();
        Assert.IsNotNull(payload);
        Assert.AreEqual("Helpful answer", payload.Content);
        Assert.AreEqual(20, payload.TotalTokens);

        var persistedMessage = await factory.ExecuteDbContextAsync(async db =>
            (await db.AiResponses
                .Include(x => x.Message)
                .SingleAsync(x => x.Id == payload.ResponseId))
            .Message.Content);
        Assert.AreEqual("Helpful answer", persistedMessage);
    }

    [TestMethod]
    public async Task PostChatStream_ReturnsChunkAndDoneEventsAndPersistsResponse()
    {
        var provider = Substitute.For<IAiChatProvider>();
        provider.StreamAsync(Arg.Any<AiProviderRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ => StreamChunks("Hello ", "world"));

        using var factory = new AiApiFactory(chatGptProvider: provider);
        var systemPrompt = new AiMessage(AiMessageType.SystemPrompt, "You are helpful.");
        await factory.SeedAsync(systemPrompt);

        using var client = factory.CreateClient();
        var request = new AiChatRequest
        {
            Provider = AiProvider.ChatGpt,
            Model = "gpt-5.4-nano",
            SystemPromptMessageId = systemPrompt.Id,
            UserPrompt = "Stream please"
        };

        var response = await client.PostAsJsonAsync("/api/ai/chats/stream", request);
        var body = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        StringAssert.Contains(body, "event: chunk");
        StringAssert.Contains(body, "\"content\":\"Hello \"");
        StringAssert.Contains(body, "\"content\":\"world\"");
        StringAssert.Contains(body, "event: done");
        StringAssert.Contains(body, "\"content\":\"Hello world\"");

        var persistedMessage = await factory.ExecuteDbContextAsync(async db =>
            (await db.AiResponses
                .Include(x => x.Message)
                .SingleAsync())
            .Message.Content);
        Assert.AreEqual("Hello world", persistedMessage);
    }

    private static async IAsyncEnumerable<string> StreamChunks(params string[] chunks)
    {
        foreach (var chunk in chunks)
        {
            yield return chunk;
            await Task.Yield();
        }
    }
}
