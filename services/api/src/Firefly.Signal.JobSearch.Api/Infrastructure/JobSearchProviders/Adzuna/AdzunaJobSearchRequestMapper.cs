using Firefly.Signal.JobSearch.Application;

namespace Firefly.Signal.JobSearch.Infrastructure.JobSearchProviders.Adzuna;

public sealed class AdzunaJobSearchRequestMapper
{
    public AdzunaJobSearchRequest Map(SearchJobsRequest request)
        => new(
            request.PageIndex + 1,
            request.PageSize,
            request.Keyword,
            request.ExcludedKeyword,
            request.Location,
            request.DistanceKilometers,
            request.Category,
            request.SalaryMin,
            request.SalaryMax,
            request.FullTime,
            request.PartTime,
            request.Permanent,
            request.Contract,
            request.SortBy,
            request.MaxDaysOld,
            request.Company,
            request.TitleOnly,
            request.Location0,
            request.Location1,
            request.Location2);
}
