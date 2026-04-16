using Firefly.Signal.Identity.Application.Queries;
using Firefly.Signal.Identity.Contracts.Requests;
using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.SharedKernel.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Firefly.Signal.Identity.Api.Apis;

public static class AuthApi
{
    public static RouteGroupBuilder MapAuthApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth");

        group.MapPost("/login", LoginAsync);
        group.MapGet("/me", GetCurrentUserAsync).RequireAuthorization();

        return group;
    }

    private static async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult>> LoginAsync(
        [FromBody] LoginUserRequest request,
        [FromServices] IAuthQueries queries,
        CancellationToken cancellationToken)
    {
        var response = await queries.AuthenticateAsync(request, cancellationToken);
        return response is null ? TypedResults.Unauthorized() : TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<AuthenticatedUserResponse>, NotFound, UnauthorizedHttpResult>> GetCurrentUserAsync(
        [FromServices] IIdentityService identityService,
        [FromServices] IAuthQueries queries,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var user = await queries.GetCurrentUserAsync(userId.Value, cancellationToken);
        return user is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(user);
    }
}
