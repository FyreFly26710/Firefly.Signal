namespace Firefly.Signal.JobSearch.Contracts.Requests;

public sealed record ApplyJobRequest(string? Note = null);

public sealed record AdvanceApplicationStatusRequest(string Status);

public sealed record UpdateApplicationNoteRequest(string? Note = null);
