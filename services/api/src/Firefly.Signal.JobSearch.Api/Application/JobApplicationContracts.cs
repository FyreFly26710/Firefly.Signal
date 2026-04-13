namespace Firefly.Signal.JobSearch.Application;

public sealed record UserJobStateResponse(
    long JobPostingId,
    bool IsSaved,
    bool IsHidden);

public sealed record JobApplicationStatusEntryResponse(
    string Status,
    DateTime StatusAtUtc);

public sealed record JobApplicationResponse(
    long Id,
    long JobPostingId,
    string? Note,
    string CurrentStatus,
    IReadOnlyList<JobApplicationStatusEntryResponse> StatusHistory);

public sealed record AppliedJobSummaryResponse(
    long ApplicationId,
    long JobPostingId,
    string Title,
    string Company,
    string CurrentStatus,
    DateTime? LatestStatusAtUtc);

public sealed record ApplyJobRequest(string? Note = null);

public sealed record AdvanceApplicationStatusRequest(string Status);
