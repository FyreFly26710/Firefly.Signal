using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;

namespace Firefly.Signal.JobSearch.Application.Mappers;

internal static class JobApplicationResponseMappers
{
    public static UserJobStateResponse ToUserJobStateResponse(UserJobState state)
        => new(
            JobPostingId: state.JobPostingId,
            IsSaved: state.IsSaved,
            IsHidden: state.IsHidden,
            IsApplied: state.IsApplied);

    public static JobApplicationResponse ToJobApplicationResponse(
        long applicationId,
        long jobPostingId,
        string? note,
        IReadOnlyList<JobApplicationStatusEntry> entries)
    {
        var orderedDescending = entries
            .OrderByDescending(entry => entry.StatusAtUtc)
            .ToList();

        var currentStatus = orderedDescending.Count > 0
            ? orderedDescending[0].Status.ToString()
            : JobApplicationStatus.Applied.ToString();

        var appliedAtUtc = entries
            .Where(entry => entry.Status == JobApplicationStatus.Applied)
            .OrderBy(entry => entry.StatusAtUtc)
            .Select(entry => entry.StatusAtUtc)
            .FirstOrDefault();

        var latestStatusAtUtc = orderedDescending.Count > 0
            ? orderedDescending[0].StatusAtUtc
            : appliedAtUtc;

        var history = entries
            .OrderBy(entry => entry.StatusAtUtc)
            .Select(entry => new JobApplicationStatusEntryResponse(
                Status: entry.Status.ToString(),
                RoundNumber: entry.RoundNumber,
                StatusAtUtc: entry.StatusAtUtc))
            .ToList();

        return new JobApplicationResponse(
            Id: applicationId,
            JobPostingId: jobPostingId,
            Note: note,
            CurrentStatus: currentStatus,
            AppliedAtUtc: appliedAtUtc,
            LatestStatusAtUtc: latestStatusAtUtc,
            StatusHistory: history);
    }
}
