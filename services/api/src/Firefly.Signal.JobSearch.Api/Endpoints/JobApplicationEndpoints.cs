using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Firefly.Signal.JobSearch.Endpoints;

public static class JobApplicationEndpoints
{
    public static IEndpointRouteBuilder MapJobApplicationEndpoints(this IEndpointRouteBuilder endpoints)
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
        ClaimsPrincipal claimsPrincipal,
        IJobApplicationService service,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId(claimsPrincipal);
        if (userId is null) return TypedResults.Unauthorized();

        var result = await service.ApplyJobAsync(id, userId.Value, request.Note, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<JobApplicationResponse>, NotFound, BadRequest<ProblemDetails>, UnauthorizedHttpResult>> AdvanceStatusAsync(
        long id,
        AdvanceApplicationStatusRequest request,
        ClaimsPrincipal claimsPrincipal,
        IJobApplicationService service,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId(claimsPrincipal);
        if (userId is null) return TypedResults.Unauthorized();

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
        ClaimsPrincipal claimsPrincipal,
        IJobApplicationService service,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId(claimsPrincipal);
        if (userId is null) return TypedResults.Unauthorized();

        var result = await service.UpdateApplicationNoteAsync(id, userId.Value, request.Note, cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<IReadOnlyList<AppliedJobSummaryResponse>>, UnauthorizedHttpResult>> GetAppliedJobsAsync(
        ClaimsPrincipal claimsPrincipal,
        IJobApplicationService service,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId(claimsPrincipal);
        if (userId is null) return TypedResults.Unauthorized();

        var result = await service.GetAppliedJobsAsync(userId.Value, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static long? GetCurrentUserId(ClaimsPrincipal claimsPrincipal)
    {
        var subject = claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        return long.TryParse(subject, out var userId) ? userId : null;
    }
}
