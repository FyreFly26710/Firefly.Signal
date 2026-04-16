using Firefly.Signal.JobSearch.Application.Mappers;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed class UpdateJobCommandHandler(JobSearchDbContext dbContext) : IRequestHandler<UpdateJobCommand, JobDetailsResponse?>
{
    public async Task<JobDetailsResponse?> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
    {
        var job = await dbContext.Jobs.SingleOrDefaultAsync(existingJob => existingJob.Id == request.Id, cancellationToken);
        if (job is null)
        {
            return null;
        }

        job.Update(
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

        await dbContext.SaveChangesAsync(cancellationToken);
        return JobResponseMappers.ToDetailsResponse(job);
    }
}
