using Firefly.Signal.JobSearch.Contracts.Responses;
using MediatR;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed record ImportJobsFromJsonCommand(Stream JsonStream, string FileName) : IRequest<ImportJobsResponse>;
