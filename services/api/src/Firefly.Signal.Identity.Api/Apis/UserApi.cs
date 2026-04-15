using Firefly.Signal.Identity.Application.Commands;
using Firefly.Signal.Identity.Application.Queries;
using Firefly.Signal.Identity.Contracts.Requests;
using Firefly.Signal.Identity.Contracts.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Firefly.Signal.Identity.Api.Apis;

public static class UserApi
{
    public static RouteGroupBuilder MapUserApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/users").RequireAuthorization();

        group.MapGet("/", ListAsync);
        group.MapGet("/{id:long}", GetByIdAsync);
        group.MapPost("/", CreateAsync);
        group.MapPut("/{id:long}", UpdateAsync);
        group.MapDelete("/{id:long}", DeleteAsync);

        return group;
    }

    private static async Task<Ok<IReadOnlyList<AuthenticatedUserResponse>>> ListAsync(
        IUserQueries queries,
        CancellationToken cancellationToken)
    {
        var users = await queries.ListAsync(cancellationToken);
        return TypedResults.Ok<IReadOnlyList<AuthenticatedUserResponse>>(users);
    }

    private static async Task<Results<Ok<AuthenticatedUserResponse>, NotFound>> GetByIdAsync(
        long id,
        IUserQueries queries,
        CancellationToken cancellationToken)
    {
        var user = await queries.GetByIdAsync(id, cancellationToken);
        return user is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(user);
    }

    private static async Task<Results<Created<AuthenticatedUserResponse>, Conflict<ProblemDetails>>> CreateAsync(
        CreateUserRequest request,
        IUserCommands commands,
        CancellationToken cancellationToken)
    {
        var response = await commands.CreateAsync(request, cancellationToken);
        if (response is null)
        {
            return TypedResults.Conflict(new ProblemDetails { Title = "User account already exists." });
        }

        return TypedResults.Created($"/api/users/{response.UserId}", response);
    }

    private static async Task<Results<Ok<AuthenticatedUserResponse>, NotFound>> UpdateAsync(
        long id,
        UpdateUserRequest request,
        IUserCommands commands,
        CancellationToken cancellationToken)
    {
        var user = await commands.UpdateAsync(id, request, cancellationToken);
        return user is null ? TypedResults.NotFound() : TypedResults.Ok(user);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteAsync(
        long id,
        IUserCommands commands,
        CancellationToken cancellationToken)
    {
        return await commands.DeleteAsync(id, cancellationToken) ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
