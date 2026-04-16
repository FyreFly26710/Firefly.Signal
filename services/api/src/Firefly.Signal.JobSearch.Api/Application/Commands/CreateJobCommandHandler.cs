using Firefly.Signal.JobSearch.Application.Mappers;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using MediatR;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed class CreateJobCommandHandler(JobSearchDbContext dbContext) : IRequestHandler<CreateJobCommand, JobDetailsResponse>
{
    public async Task<JobDetailsResponse> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
        var job = JobPosting.Create(
            request.JobRefreshRunId,
            request.SourceName,
            request.SourceJobId,
            request.SourceAdReference,
            request.Title,
            request.Description,
            request.Summary,
            request.Url,
            request.Company,
            request.CompanyDisplayName,
            request.CompanyCanonicalName,
            request.Postcode,
            request.LocationName,
            request.LocationDisplayName,
            request.LocationAreaJson,
            request.Latitude,
            request.Longitude,
            request.CategoryTag,
            request.CategoryLabel,
            request.SalaryMin,
            request.SalaryMax,
            request.SalaryCurrency,
            request.SalaryIsPredicted,
            request.ContractTime,
            request.ContractType,
            request.IsFullTime,
            request.IsPartTime,
            request.IsPermanent,
            request.IsContract,
            request.IsRemote,
            request.PostedAtUtc,
            request.ImportedAtUtc,
            request.LastSeenAtUtc,
            request.IsHidden,
            request.RawPayloadJson);

        dbContext.Jobs.Add(job);
        await dbContext.SaveChangesAsync(cancellationToken);

        return JobResponseMappers.ToDetailsResponse(job);
    }
}
