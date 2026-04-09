namespace Firefly.Signal.SharedKernel.Models;

public sealed record IdBatchRequest<TId>(IReadOnlyList<TId> Ids);
