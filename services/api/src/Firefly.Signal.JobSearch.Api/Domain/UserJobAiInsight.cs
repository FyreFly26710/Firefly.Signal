using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.JobSearch.Domain;

/// <summary>
/// Persisted AI result for one user and one job, generated from a specific analysis request.
/// </summary>
public sealed class UserJobAiInsight : AuditableEntity, IAggregateRoot
{
    private UserJobAiInsight()
    {
    }

    public long UserAccountId { get; private set; }
    public long JobPostingId { get; private set; }
    /// <summary>
    /// Admin or privileged user who initiated the analysis for auditability.
    /// </summary>
    public long GeneratedByUserAccountId { get; private set; }
    public long? AiAnalysisRunId { get; private set; }
    /// <summary>
    /// Fit rating returned by AI, constrained to a 1-5 star scale.
    /// </summary>
    public int Rating { get; private set; }
    public string? Summary { get; private set; }
    public string? DetailedExplanation { get; private set; }
    public string? CvImprovementSuggestions { get; private set; }
    public string? PromptVersion { get; private set; }
    public DateTime GeneratedAtUtc { get; private set; }

    public static UserJobAiInsight Create(
        long userAccountId,
        long jobPostingId,
        long generatedByUserAccountId,
        int rating,
        string? summary,
        string? detailedExplanation,
        string? cvImprovementSuggestions,
        string? promptVersion,
        long? aiAnalysisRunId = null)
    {
        ValidateRating(rating);

        return new UserJobAiInsight
        {
            UserAccountId = userAccountId,
            JobPostingId = jobPostingId,
            GeneratedByUserAccountId = generatedByUserAccountId,
            AiAnalysisRunId = aiAnalysisRunId,
            Rating = rating,
            Summary = Normalize(summary),
            DetailedExplanation = Normalize(detailedExplanation),
            CvImprovementSuggestions = Normalize(cvImprovementSuggestions),
            PromptVersion = Normalize(promptVersion),
            GeneratedAtUtc = DateTime.UtcNow
        };
    }

    public void UpdateResult(
        int rating,
        string? summary,
        string? detailedExplanation,
        string? cvImprovementSuggestions,
        string? promptVersion,
        long? aiAnalysisRunId = null)
    {
        ValidateRating(rating);
        Rating = rating;
        Summary = Normalize(summary);
        DetailedExplanation = Normalize(detailedExplanation);
        CvImprovementSuggestions = Normalize(cvImprovementSuggestions);
        PromptVersion = Normalize(promptVersion);
        AiAnalysisRunId = aiAnalysisRunId;
        GeneratedAtUtc = DateTime.UtcNow;
        Touch();
    }

    private static void ValidateRating(int rating)
    {
        if (rating is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(rating), "AI rating must be between 1 and 5.");
        }
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
