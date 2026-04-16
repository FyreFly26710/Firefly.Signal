using Firefly.Signal.JobSearch.Contracts.Responses;
using MediatR;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed record HideJobsCommand(IReadOnlyCollection<long> Ids) : IRequest<HideJobsResponse>;
