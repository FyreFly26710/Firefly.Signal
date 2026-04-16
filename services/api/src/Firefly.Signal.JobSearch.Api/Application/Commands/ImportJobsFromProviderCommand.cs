using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Contracts.Responses;
using MediatR;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed record ImportJobsFromProviderCommand(
    int PageIndex,
    int PageSize,
    string Where,
    string? Keyword,
    int DistanceKilometers,
    int MaxDaysOld,
    string Category,
    JobSearchProviderKind Provider,
    string? ExcludedKeyword,
    decimal? SalaryMin,
    decimal? SalaryMax) : IRequest<ImportJobsResponse>;
