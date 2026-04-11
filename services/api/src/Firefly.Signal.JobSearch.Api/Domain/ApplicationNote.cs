using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.JobSearch.Domain;

/// <summary>
/// Free-form note attached to a specific application record.
/// </summary>
public sealed class ApplicationNote : AuditableEntity
{
    private ApplicationNote()
    {
    }

    public long JobApplicationId { get; private set; }
    public long UserAccountId { get; private set; }
    public string Body { get; private set; } = string.Empty;

    public static ApplicationNote Create(long jobApplicationId, long userAccountId, string body)
    {
        return new ApplicationNote
        {
            JobApplicationId = jobApplicationId,
            UserAccountId = userAccountId,
            Body = body.Trim()
        };
    }

    public void UpdateBody(string body)
    {
        Body = body.Trim();
        Touch();
    }
}
