namespace Firefly.Signal.SharedKernel.Models;

public sealed record Paged<TItem>(
    int PageIndex,
    int PageSize,
    long TotalCount,
    IReadOnlyList<TItem> Items);
