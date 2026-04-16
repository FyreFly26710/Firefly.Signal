using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using MediatR;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed record AdvanceApplicationStatusCommand(
    long JobId,
    long UserAccountId,
    JobApplicationStatus NewStatus) : IRequest<JobApplicationResponse?>;
