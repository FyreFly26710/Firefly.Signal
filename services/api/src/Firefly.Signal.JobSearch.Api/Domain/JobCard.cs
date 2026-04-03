namespace Firefly.Signal.JobSearch.Domain;

public sealed record JobCard(
    long Id,
    string Title,
    string Company,
    string Location,
    string Summary,
    string Url,
    string SourceName,
    bool IsRemote,
    DateTime PostedAtUtc);
