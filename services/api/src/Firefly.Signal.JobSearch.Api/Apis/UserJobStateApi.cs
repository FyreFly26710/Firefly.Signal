using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Infrastructure.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Firefly.Signal.JobSearch.Api.Apis;

public static class UserJobStateApi
{
    public static IEndpointRouteBuilder MapUserJobStateApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/job-search/jobs").RequireAuthorization();

        group.MapPost("/{id:long}/save", SaveAsync);
        group.MapDelete("/{id:long}/save", UnsaveAsync);
        group.MapPost("/{id:long}/hide", HideAsync);
        group.MapDelete("/{id:long}/hide", UnhideAsync);

        return endpoints;
    }

    private static async Task<Results<Ok<UserJobStateResponse>, NotFound, UnauthorizedHttpResult>> SaveAsync(
        long id,
        ICurrentUserContext currentUserContext,
        IUserJobStateService service,
        CancellationToken cancellationToken)
    {
        var userId = currentUserContext.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var result = await service.SaveJobAsync(id, userId.Value, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<UserJobStateResponse>, NotFound, UnauthorizedHttpResult>> UnsaveAsync(
        long id,
        ICurrentUserContext currentUserContext,
        IUserJobStateService service,
        CancellationToken cancellationToken)
    {
        var userId = currentUserContext.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var result = await service.UnsaveJobAsync(id, userId.Value, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<UserJobStateResponse>, NotFound, UnauthorizedHttpResult>> HideAsync(
        long id,
        ICurrentUserContext currentUserContext,
        IUserJobStateService service,
        CancellationToken cancellationToken)
    {
        var userId = currentUserContext.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var result = await service.HideJobForUserAsync(id, userId.Value, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<UserJobStateResponse>, NotFound, UnauthorizedHttpResult>> UnhideAsync(
        long id,
        ICurrentUserContext currentUserContext,
        IUserJobStateService service,
        CancellationToken cancellationToken)
    {
        var userId = currentUserContext.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var result = await service.UnhideJobForUserAsync(id, userId.Value, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }
}
