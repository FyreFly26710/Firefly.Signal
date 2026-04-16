using Firefly.Signal.JobSearch.Contracts.Responses;
using MediatR;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed record UnsaveJobCommand(long JobId, long UserAccountId) : IRequest<UserJobStateResponse?>;
