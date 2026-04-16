using Firefly.Signal.JobSearch.Application.Commands;
using Firefly.Signal.JobSearch.Application.Queries;
using Firefly.Signal.JobSearch.Contracts.Requests;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.SharedKernel.Services;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Firefly.Signal.JobSearch.Api.Apis;

public static class JobApplicationApi
{
    public static IEndpointRouteBuilder MapJobApplicationApi(this IEndpointRouteBuilder endpoints)
    {
        var jobsGroup = endpoints.MapGroup("/api/job-search/jobs").RequireAuthorization();
        var applicationsGroup = endpoints.MapGroup("/api/job-search/applications").RequireAuthorization();

        jobsGroup.MapPost("/{id:long}/apply", ApplyAsync);
        jobsGroup.MapPut("/{id:long}/apply/status", AdvanceStatusAsync);
        jobsGroup.MapPut("/{id:long}/apply/note", UpdateNoteAsync);
        applicationsGroup.MapGet("/", GetAppliedJobsAsync);

        return endpoints;
    }

    private static async Task<Results<Ok<JobApplicationResponse>, NotFound, UnauthorizedHttpResult>> ApplyAsync(
        long id,
        [FromBody] ApplyJobRequest request,
        [FromServices] IIdentityService identityService,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var result = await mediator.Send(JobApplicationApiMappers.ToApplyCommand(id, userId.Value, request), cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<JobApplicationResponse>, NotFound, BadRequest<ProblemDetails>, UnauthorizedHttpResult>> AdvanceStatusAsync(
        long id,
        [FromBody] AdvanceApplicationStatusRequest request,
        [FromServices] IIdentityService identityService,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        if (!JobApplicationApiMappers.TryParseStatus(request.Status, out var newStatus))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid status",
                Detail = $"'{request.Status}' is not a valid application status."
            });
        }

        var result = await mediator.Send(
            JobApplicationApiMappers.ToAdvanceStatusCommand(id, userId.Value, newStatus),
            cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<JobApplicationResponse>, NotFound, UnauthorizedHttpResult>> UpdateNoteAsync(
        long id,
        [FromBody] UpdateApplicationNoteRequest request,
        [FromServices] IIdentityService identityService,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var result = await mediator.Send(JobApplicationApiMappers.ToUpdateNoteCommand(id, userId.Value, request), cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<IReadOnlyList<AppliedJobSummaryResponse>>, UnauthorizedHttpResult>> GetAppliedJobsAsync(
        [FromServices] IIdentityService identityService,
        [FromServices] IJobApplicationQueries queries,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var result = await queries.GetAppliedJobsAsync(userId.Value, cancellationToken);
        return TypedResults.Ok(result);
    }
}
