using Firefly.Signal.JobSearch.Contracts.Responses;
using MediatR;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed record UpdateApplicationNoteCommand(long JobId, long UserAccountId, string? Note) : IRequest<JobApplicationResponse?>;
