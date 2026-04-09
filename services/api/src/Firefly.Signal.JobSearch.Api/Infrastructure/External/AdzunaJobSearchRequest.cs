namespace Firefly.Signal.JobSearch.Infrastructure.External;

public sealed record AdzunaJobSearchRequest(
    int PageNumber,
    int ResultsPerPage,
    string What,
    string Where);
