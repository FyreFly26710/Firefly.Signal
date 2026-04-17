namespace Firefly.Signal.SharedKernel.Models;

public sealed record PagedRequest(
    int PageIndex = 0,
    int PageSize = 20);
