using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.JobSearch.Domain;

/// <summary>
/// Cached or imported postcode reference data used for distance-based filtering.
/// </summary>
public sealed class PostcodeLookup : AuditableEntity
{
    private PostcodeLookup()
    {
    }

    public string Postcode { get; private set; } = string.Empty;
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }
    /// <summary>
    /// Source identifier for the postcode lookup record, such as a local dataset or external API.
    /// </summary>
    public string Source { get; private set; } = string.Empty;
    public DateTime LastVerifiedAtUtc { get; private set; }

    public static PostcodeLookup Create(string postcode, decimal latitude, decimal longitude, string source)
    {
        return new PostcodeLookup
        {
            Postcode = postcode.Trim().ToUpperInvariant(),
            Latitude = latitude,
            Longitude = longitude,
            Source = source.Trim(),
            LastVerifiedAtUtc = DateTime.UtcNow
        };
    }

    public void Refresh(decimal latitude, decimal longitude, string source, DateTime? verifiedAtUtc = null)
    {
        Latitude = latitude;
        Longitude = longitude;
        Source = source.Trim();
        LastVerifiedAtUtc = verifiedAtUtc ?? DateTime.UtcNow;
        Touch();
    }
}
