namespace Firefly.Signal.JobSearch.Contracts.Responses;

public sealed record UserJobStateResponse(
    long JobPostingId,
    bool IsSaved,
    bool IsHidden,
    bool IsApplied);

public sealed record JobApplicationStatusEntryResponse(
    string Status,
    int? RoundNumber,
    DateTime StatusAtUtc);

public sealed record JobApplicationResponse(
    long Id,
    long JobPostingId,
    string? Note,
    string CurrentStatus,
    DateTime AppliedAtUtc,
    DateTime LatestStatusAtUtc,
    IReadOnlyList<JobApplicationStatusEntryResponse> StatusHistory);

public sealed record AppliedJobSummaryResponse(
    long ApplicationId,
    long JobPostingId,
    string Title,
    string Company,
    string CurrentStatus,
    DateTime AppliedAtUtc,
    DateTime? LatestStatusAtUtc);

public sealed record AppliedJobDetailResponse(
    long ApplicationId,
    long JobPostingId,
    string Title,
    string Company,
    string? Note,
    string CurrentStatus,
    DateTime AppliedAtUtc,
    DateTime LatestStatusAtUtc,
    IReadOnlyList<JobApplicationStatusEntryResponse> StatusHistory);
