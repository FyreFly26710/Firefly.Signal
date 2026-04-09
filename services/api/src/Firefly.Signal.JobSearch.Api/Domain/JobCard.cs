namespace Firefly.Signal.JobSearch.Domain;

public sealed record JobCard(
    string Id,
    string Title,
    string Company,
    string LocationName,
    string Summary,
    string Url,
    string SourceName,
    bool IsRemote,
    DateTime PostedAtUtc);
