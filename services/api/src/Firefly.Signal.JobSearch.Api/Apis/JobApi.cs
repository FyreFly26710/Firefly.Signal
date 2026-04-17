using System.Text.Json;
using Firefly.Signal.JobSearch.Application.Commands;
using Firefly.Signal.JobSearch.Application.Queries;
using Firefly.Signal.JobSearch.Contracts.Requests;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.SharedKernel.Models;
using Firefly.Signal.SharedKernel.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Firefly.Signal.JobSearch.Api.Apis;

public static class JobApi
{
    public static IEndpointRouteBuilder MapJobApi(this IEndpointRouteBuilder endpoints)
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
        adminGroup.MapPost("/{id:long}/catalog-hide", HideByIdAsync);
        adminGroup.MapPost("/catalog-hide", HideManyAsync);
        adminGroup.MapPost("/import/provider", ImportFromProviderAsync);
        adminGroup.MapPost("/import/json", ImportFromJsonAsync).DisableAntiforgery();
        adminGroup.MapGet("/import-runs", GetRecentImportRunsAsync);
        adminGroup.MapPost("/export", ExportAsync);

        return endpoints;
    }

    private static async Task<Ok<Paged<JobSearchResultResponse>>> GetPageAsync(
        [AsParameters] SearchJobsPageRequest request,
        [FromServices] IIdentityService identityService,
        [FromServices] IJobSearchQueries queries,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        var result = await queries.SearchPageAsync(request, userId, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<JobDetailsResponse>, NotFound>> GetByIdAsync(
        long id,
        [FromServices] IIdentityService identityService,
        [FromServices] IJobSearchQueries queries,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        var job = await queries.GetByIdAsync(id, userId, cancellationToken);
        return job is null ? TypedResults.NotFound() : TypedResults.Ok(job);
    }

    private static async Task<Created<JobDetailsResponse>> CreateAsync(
        [FromBody] CreateJobRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var created = await mediator.Send(JobApiMappers.ToCreateCommand(request), cancellationToken);
        return TypedResults.Created($"/api/job-search/jobs/{created.Id}", created);
    }

    private static async Task<Results<Ok<JobDetailsResponse>, NotFound>> UpdateAsync(
        long id,
        [FromBody] UpdateJobRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var updated = await mediator.Send(JobApiMappers.ToUpdateCommand(id, request), cancellationToken);
        return updated is null ? TypedResults.NotFound() : TypedResults.Ok(updated);
    }

    private static async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> DeleteByIdAsync(
        long id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(JobApiMappers.ToDeleteCommand([id]), cancellationToken);
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
        [FromBody] IdBatchRequest<long> request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(JobApiMappers.ToDeleteCommand(request.Ids), cancellationToken);
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
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
        => TypedResults.Ok(await mediator.Send(JobApiMappers.ToHideCommand([id]), cancellationToken));

    private static async Task<Ok<HideJobsResponse>> HideManyAsync(
        [FromBody] IdBatchRequest<long> request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
        => TypedResults.Ok(await mediator.Send(JobApiMappers.ToHideCommand(request.Ids), cancellationToken));

    private static async Task<Ok<ImportJobsResponse>> ImportFromProviderAsync(
        [FromBody] ImportJobsFromProviderRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
        => TypedResults.Ok(await mediator.Send(JobApiMappers.ToImportFromProviderCommand(request), cancellationToken));

    private static async Task<Ok<Paged<JobImportRunResponse>>> GetRecentImportRunsAsync(
        [AsParameters] PagedRequest request,
        [FromServices] IJobSearchQueries queries,
        CancellationToken cancellationToken)
        => TypedResults.Ok(await queries.GetRecentImportRunsAsync(request, cancellationToken));

    private static async Task<Results<Ok<ImportJobsResponse>, BadRequest<ProblemDetails>>> ImportFromJsonAsync(
        [FromForm] IFormFile? file,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "JSON file is required",
                Detail = "Upload a non-empty JSON file containing exported jobs."
            });
        }

        if (!file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "JSON file is required",
                Detail = "The uploaded file must use a .json extension."
            });
        }

        await using var stream = file.OpenReadStream();
        return TypedResults.Ok(await mediator.Send(JobApiMappers.ToImportFromJsonCommand(stream, file.FileName), cancellationToken));
    }

    private static async Task<FileContentHttpResult> ExportAsync(
        [FromBody] ExportJobsRequest request,
        [FromServices] IJobSearchQueries queries,
        CancellationToken cancellationToken)
    {
        var export = await queries.ExportAsync(request, cancellationToken);

        var content = JsonSerializer.SerializeToUtf8Bytes(export, new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            WriteIndented = true
        });

        return TypedResults.File(
            content,
            "application/json",
            $"jobs-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json");
    }
}
