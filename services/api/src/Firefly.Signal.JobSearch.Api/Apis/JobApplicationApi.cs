using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.SharedKernel.Services;
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
        ApplyJobRequest request,
        IIdentityService identityService,
        IJobApplicationService service,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var result = await service.ApplyJobAsync(id, userId.Value, request.Note, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<JobApplicationResponse>, NotFound, BadRequest<ProblemDetails>, UnauthorizedHttpResult>> AdvanceStatusAsync(
        long id,
        AdvanceApplicationStatusRequest request,
        IIdentityService identityService,
        IJobApplicationService service,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        if (!Enum.TryParse<JobApplicationStatus>(request.Status, ignoreCase: true, out var newStatus))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid status",
                Detail = $"'{request.Status}' is not a valid application status."
            });
        }

        try
        {
            var result = await service.AdvanceApplicationStatusAsync(id, userId.Value, newStatus, cancellationToken);
            return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid status transition",
                Detail = ex.Message
            });
        }
    }

    private static async Task<Results<Ok<JobApplicationResponse>, NotFound, UnauthorizedHttpResult>> UpdateNoteAsync(
        long id,
        UpdateApplicationNoteRequest request,
        IIdentityService identityService,
        IJobApplicationService service,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var result = await service.UpdateApplicationNoteAsync(id, userId.Value, request.Note, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<IReadOnlyList<AppliedJobSummaryResponse>>, UnauthorizedHttpResult>> GetAppliedJobsAsync(
        IIdentityService identityService,
        IJobApplicationService service,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var result = await service.GetAppliedJobsAsync(userId.Value, cancellationToken);
        return TypedResults.Ok(result);
    }
}
