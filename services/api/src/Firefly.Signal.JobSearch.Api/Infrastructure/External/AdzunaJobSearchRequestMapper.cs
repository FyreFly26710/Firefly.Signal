using Firefly.Signal.JobSearch.Application;

namespace Firefly.Signal.JobSearch.Infrastructure.External;

public sealed class AdzunaJobSearchRequestMapper
{
    public AdzunaJobSearchRequest Map(SearchJobsRequest request)
        => new(
            request.PageIndex + 1,
            request.PageSize,
            request.Keyword,
            request.Postcode);
}
