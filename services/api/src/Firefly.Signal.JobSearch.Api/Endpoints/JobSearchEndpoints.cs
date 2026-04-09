using Firefly.Signal.JobSearch.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Firefly.Signal.JobSearch.Endpoints;

public static class JobSearchEndpoints
{
    public static IEndpointRouteBuilder MapJobSearchEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/job-search/jobs").RequireAuthorization();
        var adminGroup = group.MapGroup(string.Empty)
            .RequireAuthorization(new AuthorizeAttribute { Roles = "admin" });

        group.MapGet("/", GetPageAsync);
        group.MapGet("/{id:long}", GetByIdAsync);
        adminGroup.MapPost("/", CreateAsync);
        adminGroup.MapPut("/{id:long}", UpdateAsync);
        adminGroup.MapDelete("/{id:long}", DeleteByIdAsync);
        adminGroup.MapDelete("/", DeleteManyAsync);
        adminGroup.MapPost("/{id:long}/hide", HideByIdAsync);
        adminGroup.MapPost("/hide", HideManyAsync);

        return endpoints;
    }

    private static async Task<Ok<PagedJobsResponse>> GetPageAsync(
        [FromQuery] int pageIndex,
        [FromQuery] int pageSize,
        [FromQuery] string? keyword,
        [FromQuery] string? company,
        [FromQuery] string? postcode,
        [FromQuery] string? location,
        [FromQuery] string? sourceName,
        [FromQuery] string? categoryTag,
        [FromQuery] bool? isHidden,
        [FromServices] IJobSearchService service,
        CancellationToken cancellationToken)
        => TypedResults.Ok(await service.GetPageAsync(
            new GetJobsPageRequest(
                Math.Max(pageIndex, 0),
                pageSize <= 0 ? 20 : pageSize,
                keyword,
                company,
                postcode,
                location,
                sourceName,
                categoryTag,
                isHidden),
            cancellationToken));

    private static async Task<Results<Ok<JobDetailsResponse>, NotFound>> GetByIdAsync(
        long id,
        [FromServices] IJobSearchService service,
        CancellationToken cancellationToken)
    {
        var job = await service.GetByIdAsync(id, cancellationToken);
        return job is null ? TypedResults.NotFound() : TypedResults.Ok(job);
    }

    private static async Task<Created<JobDetailsResponse>> CreateAsync(
        [FromBody] CreateJobRequest request,
        [FromServices] IJobSearchService service,
        CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(request, cancellationToken);
        return TypedResults.Created($"/api/job-search/jobs/{created.Id}", created);
    }

    private static async Task<Results<Ok<JobDetailsResponse>, NotFound>> UpdateAsync(
        long id,
        [FromBody] UpdateJobRequest request,
        [FromServices] IJobSearchService service,
        CancellationToken cancellationToken)
    {
        var updated = await service.UpdateAsync(id, request, cancellationToken);
        return updated is null ? TypedResults.NotFound() : TypedResults.Ok(updated);
    }

    private static async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> DeleteByIdAsync(
        long id,
        [FromServices] IJobSearchService service,
        CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync([id], cancellationToken);
        if (result.MissingIds.Count == 1)
        {
            return TypedResults.NotFound();
        }

        if (result.NotHiddenIds.Count > 0)
        {
            return TypedResults.Conflict(new ProblemDetails
            {
                Title = "Job must be hidden before deletion",
                Detail = $"Job {id} must be hidden before it can be deleted."
            });
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<DeleteJobsResponse>, Conflict<ProblemDetails>>> DeleteManyAsync(
        [FromBody] BulkJobIdsRequest request,
        [FromServices] IJobSearchService service,
        CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(request.Ids, cancellationToken);
        if (result.NotHiddenIds.Count > 0)
        {
            return TypedResults.Conflict(new ProblemDetails
            {
                Title = "Some jobs must be hidden before deletion",
                Detail = $"These jobs are not hidden: {string.Join(", ", result.NotHiddenIds)}"
            });
        }

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<HideJobsResponse>> HideByIdAsync(
        long id,
        [FromServices] IJobSearchService service,
        CancellationToken cancellationToken)
        => TypedResults.Ok(await service.HideAsync([id], cancellationToken));

    private static async Task<Ok<HideJobsResponse>> HideManyAsync(
        [FromBody] BulkJobIdsRequest request,
        [FromServices] IJobSearchService service,
        CancellationToken cancellationToken)
        => TypedResults.Ok(await service.HideAsync(request.Ids, cancellationToken));
}
