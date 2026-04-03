using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Firefly.Signal.JobSearch.Endpoints;

public static class JobSearchEndpoints
{
    public static IEndpointRouteBuilder MapJobSearchEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/job-search");

        group.MapGet("/demo", DemoAsync);
        group.MapGet("/", ListAsync);
        group.MapGet("/{id:long}", GetByIdAsync);
        group.MapGet("/search", SearchAsync);

        return endpoints;
    }

    private static async Task<Ok<SearchJobsResponse>> DemoAsync(IJobSearchService service, CancellationToken cancellationToken)
        => TypedResults.Ok(await service.SearchAsync(new SearchJobsRequest("SW1A", ".NET"), cancellationToken));

    private static async Task<Ok<IReadOnlyList<JobCard>>> ListAsync(IJobSearchService service, CancellationToken cancellationToken)
        => TypedResults.Ok(await service.ListAsync(cancellationToken));

    private static async Task<Results<Ok<JobCard>, NotFound>> GetByIdAsync(long id, IJobSearchService service, CancellationToken cancellationToken)
    {
        var job = await service.GetByIdAsync(id, cancellationToken);
        return job is null ? TypedResults.NotFound() : TypedResults.Ok(job);
    }

    private static async Task<Results<Ok<SearchJobsResponse>, BadRequest<ProblemDetails>>> SearchAsync(
        [FromQuery] string postcode,
        [FromQuery] string keyword,
        [FromQuery] int pageIndex,
        [FromQuery] int pageSize,
        IJobSearchService service,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(postcode))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid postcode",
                Detail = "A postcode is required."
            });
        }

        if (string.IsNullOrWhiteSpace(keyword))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid keyword",
                Detail = "A keyword is required."
            });
        }

        var request = new SearchJobsRequest(postcode, keyword, Math.Max(pageIndex, 0), pageSize <= 0 ? 20 : pageSize);
        return TypedResults.Ok(await service.SearchAsync(request, cancellationToken));
    }
}
