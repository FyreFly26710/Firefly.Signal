using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;

namespace Firefly.Signal.JobSearch.Application.Mappers;

internal static class JobApplicationResponseMappers
{
    public static UserJobStateResponse ToUserJobStateResponse(UserJobState state)
        => new(JobPostingId: state.JobPostingId, IsSaved: state.IsSaved, IsHidden: state.IsHidden);

    public static JobApplicationResponse ToJobApplicationResponse(
        long applicationId,
        long jobPostingId,
        string? note,
        IReadOnlyList<JobApplicationStatusEntry> entries)
    {
        var currentStatus = entries.Count > 0
            ? entries[0].Status.ToString()
            : JobApplicationStatus.Applied.ToString();

        var history = entries
            .OrderBy(entry => entry.StatusAtUtc)
            .Select(entry => new JobApplicationStatusEntryResponse(Status: entry.Status.ToString(), StatusAtUtc: entry.StatusAtUtc))
            .ToList();

        return new JobApplicationResponse(
            Id: applicationId,
            JobPostingId: jobPostingId,
            Note: note,
            CurrentStatus: currentStatus,
            StatusHistory: history);
    }
}
