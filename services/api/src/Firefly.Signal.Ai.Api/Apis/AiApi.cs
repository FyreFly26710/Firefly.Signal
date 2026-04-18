using System.Text.Json;
using Firefly.Signal.Ai.Api.Application.Commands;
using Firefly.Signal.Ai.Api.Contracts.Requests;
using Firefly.Signal.Ai.Api.Contracts.Responses;
using MediatR;

namespace Firefly.Signal.Ai.Api.Apis;

public static class AiApi
{
    public static RouteGroupBuilder MapAiApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/ai");

        group.MapPost("/chat", HandleChatAsync)
            .Produces<AiChatResponse>()
            .ProducesProblem(400)
            .WithName("AiChat");

        group.MapPost("/chat/stream", HandleChatStreamAsync)
            .Produces(200, contentType: "text/event-stream")
            .ProducesProblem(400)
            .WithName("AiChatStream");

        return group;
    }

    private static async Task<IResult> HandleChatAsync(
        AiChatRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var response = await mediator.Send(request.ToCommand(), ct);

        return Results.Ok(response);
    }

    private static async Task HandleChatStreamAsync(
        AiChatRequest request,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        httpContext.Response.ContentType = "text/event-stream";
        httpContext.Response.Headers.CacheControl = "no-cache";
        httpContext.Response.Headers.Connection = "keep-alive";

        try
        {
            var stream = await mediator.Send(request.ToStreamCommand(), ct);

            await foreach (var streamEvent in stream.WithCancellation(ct))
            {
                switch (streamEvent)
                {
                    case AiChatStreamChunkEvent chunk:
                        await WriteSseEventAsync(
                            httpContext.Response,
                            "chunk",
                            JsonSerializer.Serialize(new AiChatStreamChunkResponse { Content = chunk.Content }),
                            ct);
                        break;
                    case AiChatStreamDoneEvent done:
                        await WriteSseEventAsync(httpContext.Response, "done", JsonSerializer.Serialize(done.Response), ct);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            await WriteSseEventAsync(httpContext.Response, "error", JsonSerializer.Serialize(ex.Message), ct);
        }
    }

    private static async Task WriteSseEventAsync(
        HttpResponse response,
        string eventName,
        string data,
        CancellationToken ct)
    {
        await response.WriteAsync($"event: {eventName}\ndata: {data}\n\n", ct);
        await response.Body.FlushAsync(ct);
    }
}
