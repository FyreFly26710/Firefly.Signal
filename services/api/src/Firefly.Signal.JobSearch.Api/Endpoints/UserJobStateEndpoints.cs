using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Firefly.Signal.JobSearch.Application;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Firefly.Signal.JobSearch.Endpoints;

public static class UserJobStateEndpoints
{
    public static IEndpointRouteBuilder MapUserJobStateEndpoints(this IEndpointRouteBuilder endpoints)
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
        ClaimsPrincipal claimsPrincipal,
        IUserJobStateService service,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId(claimsPrincipal);
        if (userId is null) return TypedResults.Unauthorized();

        var result = await service.SaveJobAsync(id, userId.Value, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<UserJobStateResponse>, NotFound, UnauthorizedHttpResult>> UnsaveAsync(
        long id,
        ClaimsPrincipal claimsPrincipal,
        IUserJobStateService service,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId(claimsPrincipal);
        if (userId is null) return TypedResults.Unauthorized();

        var result = await service.UnsaveJobAsync(id, userId.Value, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<UserJobStateResponse>, NotFound, UnauthorizedHttpResult>> HideAsync(
        long id,
        ClaimsPrincipal claimsPrincipal,
        IUserJobStateService service,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId(claimsPrincipal);
        if (userId is null) return TypedResults.Unauthorized();

        var result = await service.HideJobForUserAsync(id, userId.Value, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<UserJobStateResponse>, NotFound, UnauthorizedHttpResult>> UnhideAsync(
        long id,
        ClaimsPrincipal claimsPrincipal,
        IUserJobStateService service,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId(claimsPrincipal);
        if (userId is null) return TypedResults.Unauthorized();

        var result = await service.UnhideJobForUserAsync(id, userId.Value, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static long? GetCurrentUserId(ClaimsPrincipal claimsPrincipal)
    {
        var subject = claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        return long.TryParse(subject, out var userId) ? userId : null;
    }
}
