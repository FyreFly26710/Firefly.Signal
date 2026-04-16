using Firefly.Signal.JobSearch.Application.Commands;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.SharedKernel.Services;
using MediatR;
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
        IIdentityService identityService,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var result = await mediator.Send(UserJobStateApiMappers.ToSaveCommand(id, userId.Value), cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<UserJobStateResponse>, NotFound, UnauthorizedHttpResult>> UnsaveAsync(
        long id,
        IIdentityService identityService,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var result = await mediator.Send(UserJobStateApiMappers.ToUnsaveCommand(id, userId.Value), cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<UserJobStateResponse>, NotFound, UnauthorizedHttpResult>> HideAsync(
        long id,
        IIdentityService identityService,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var result = await mediator.Send(UserJobStateApiMappers.ToHideCommand(id, userId.Value), cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<UserJobStateResponse>, NotFound, UnauthorizedHttpResult>> UnhideAsync(
        long id,
        IIdentityService identityService,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var result = await mediator.Send(UserJobStateApiMappers.ToUnhideCommand(id, userId.Value), cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }
}
