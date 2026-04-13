using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Infrastructure.External;
using Firefly.Signal.SharedKernel.Models;
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
        adminGroup.MapPost("/{id:long}/catalog-hide", HideByIdAsync);
        adminGroup.MapPost("/catalog-hide", HideManyAsync);
        adminGroup.MapPost("/import/provider", ImportFromProviderAsync);
        adminGroup.MapPost("/import/json", ImportFromJsonAsync)
            .DisableAntiforgery();
        adminGroup.MapPost("/export", ExportAsync);

        return endpoints;
    }

    private static async Task<Ok<Paged<JobSearchResultResponse>>> GetPageAsync(
        [FromQuery] int pageIndex,
        [FromQuery] int pageSize,
        [FromQuery] string? keyword,
        [FromQuery] string? company,
        [FromQuery] string? postcode,
        [FromQuery] string? location,
        [FromQuery] string? sourceName,
        [FromQuery] string? categoryTag,
        [FromQuery] bool? isHidden,
        ClaimsPrincipal claimsPrincipal,
        [FromServices] IJobSearchService service,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId(claimsPrincipal);
        return TypedResults.Ok(await service.SearchPageAsync(
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
            userId,
            cancellationToken));
    }

    private static long? GetCurrentUserId(ClaimsPrincipal claimsPrincipal)
    {
        var subject = claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        return long.TryParse(subject, out var id) ? id : null;
    }

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
        [FromBody] IdBatchRequest<long> request,
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
        [FromBody] IdBatchRequest<long> request,
        [FromServices] IJobSearchService service,
        CancellationToken cancellationToken)
        => TypedResults.Ok(await service.HideAsync(request.Ids, cancellationToken));

    private static async Task<Results<Ok<ImportJobsResponse>, BadRequest<ProblemDetails>>> ImportFromProviderAsync(
        [FromBody] ImportJobsFromProviderRequest request,
        [FromServices] IJobSearchService service,
        CancellationToken cancellationToken)
    {
        try
        {
            return TypedResults.Ok(await service.ImportFromProviderAsync(request, cancellationToken));
        }
        catch (JobSearchProviderException exception)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Provider import failed",
                Detail = exception.Message
            });
        }
    }

    private static async Task<Results<Ok<ImportJobsResponse>, BadRequest<ProblemDetails>>> ImportFromJsonAsync(
        IFormFile? file,
        [FromServices] IJobSearchService service,
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

        try
        {
            await using var stream = file.OpenReadStream();
            return TypedResults.Ok(await service.ImportFromJsonAsync(stream, file.FileName, cancellationToken));
        }
        catch (InvalidDataException exception)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "JSON import failed",
                Detail = exception.Message
            });
        }
        catch (JsonException)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "JSON import failed",
                Detail = "The uploaded file is not valid job export JSON."
            });
        }
    }

    private static async Task<FileContentHttpResult> ExportAsync(
        [FromBody] ExportJobsRequest request,
        [FromServices] IJobSearchService service,
        CancellationToken cancellationToken)
    {
        var export = await service.ExportAsync(request, cancellationToken);

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
