using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.JobSearch.Domain;

public sealed class JobPosting : AuditableEntity, IAggregateRoot
{
    private JobPosting()
    {
    }

    public string Title { get; private set; } = string.Empty;
    public string Company { get; private set; } = string.Empty;
    public string Postcode { get; private set; } = string.Empty;
    public string LocationName { get; private set; } = string.Empty;
    public string Summary { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public string SourceName { get; private set; } = string.Empty;
    public bool IsRemote { get; private set; }
    public DateTime PostedAtUtc { get; private set; }

    public static JobPosting Create(
        string title,
        string company,
        string postcode,
        string locationName,
        string summary,
        string url,
        string sourceName,
        bool isRemote,
        DateTime postedAtUtc)
    {
        return new JobPosting
        {
            Title = title.Trim(),
            Company = company.Trim(),
            Postcode = postcode.Trim().ToUpperInvariant(),
            LocationName = locationName.Trim(),
            Summary = summary.Trim(),
            Url = url.Trim(),
            SourceName = sourceName.Trim(),
            IsRemote = isRemote,
            PostedAtUtc = postedAtUtc
        };
    }
}
