namespace Firefly.Signal.Ai.Api.Apis;

public static class AiApi
{
    public static RouteGroupBuilder MapAiApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/ai");
        return group;
    }

}
