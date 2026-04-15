using Firefly.Signal.Identity.Application.Commands;
using Firefly.Signal.Identity.Application.Queries;
using Firefly.Signal.Identity.Contracts.Requests;
using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.SharedKernel.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Firefly.Signal.Identity.Api.Apis;

public static class UserProfileApi
{
    public static RouteGroupBuilder MapUserProfileApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/users/profile").RequireAuthorization();

        group.MapGet("/", GetCurrentAsync);
        group.MapPut("/", UpsertCurrentAsync);

        return group;
    }

    private static async Task<Results<Ok<UserProfileResponse>, NotFound, UnauthorizedHttpResult>> GetCurrentAsync(
        IIdentityService identityService,
        IUserProfileQueries queries,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var profile = await queries.GetCurrentAsync(userId.Value, cancellationToken);

        return profile is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(profile);
    }

    private static async Task<Results<Created<UserProfileResponse>, Ok<UserProfileResponse>, UnauthorizedHttpResult>> UpsertCurrentAsync(
        UserProfileRequest request,
        IIdentityService identityService,
        IUserProfileCommands commands,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var result = await commands.UpsertCurrentAsync(userId.Value, request, cancellationToken);
        if (result is null)
        {
            return TypedResults.Unauthorized();
        }

        return result.Created
            ? TypedResults.Created("/api/users/profile", result.Response)
            : TypedResults.Ok(result.Response);
    }
}
