using Firefly.Signal.JobSearch.Application.Commands;
using Firefly.Signal.JobSearch.Contracts.Requests;

namespace Firefly.Signal.JobSearch.Api.Apis;

internal static class JobApiMappers
{
    public static CreateJobCommand ToCreateCommand(CreateJobRequest request)
        => new(
            JobRefreshRunId: request.JobRefreshRunId,
            SourceName: request.SourceName,
            SourceJobId: request.SourceJobId,
            SourceAdReference: request.SourceAdReference,
            Title: request.Title,
            Description: request.Description,
            Summary: request.Summary,
            Url: request.Url,
            Company: request.Company,
            CompanyDisplayName: request.CompanyDisplayName,
            CompanyCanonicalName: request.CompanyCanonicalName,
            Postcode: request.Postcode,
            LocationName: request.LocationName,
            LocationDisplayName: request.LocationDisplayName,
            LocationAreaJson: request.LocationAreaJson,
            Latitude: request.Latitude,
            Longitude: request.Longitude,
            CategoryTag: request.CategoryTag,
            CategoryLabel: request.CategoryLabel,
            SalaryMin: request.SalaryMin,
            SalaryMax: request.SalaryMax,
            SalaryCurrency: request.SalaryCurrency,
            SalaryIsPredicted: request.SalaryIsPredicted,
            ContractTime: request.ContractTime,
            ContractType: request.ContractType,
            IsFullTime: request.IsFullTime,
            IsPartTime: request.IsPartTime,
            IsPermanent: request.IsPermanent,
            IsContract: request.IsContract,
            IsRemote: request.IsRemote,
            PostedAtUtc: request.PostedAtUtc,
            ImportedAtUtc: request.ImportedAtUtc,
            LastSeenAtUtc: request.LastSeenAtUtc,
            IsHidden: request.IsHidden,
            RawPayloadJson: request.RawPayloadJson);

    public static UpdateJobCommand ToUpdateCommand(long id, UpdateJobRequest request)
        => new(
            Id: id,
            JobRefreshRunId: request.JobRefreshRunId,
            SourceName: request.SourceName,
            SourceJobId: request.SourceJobId,
            SourceAdReference: request.SourceAdReference,
            Title: request.Title,
            Description: request.Description,
            Summary: request.Summary,
            Url: request.Url,
            Company: request.Company,
            CompanyDisplayName: request.CompanyDisplayName,
            CompanyCanonicalName: request.CompanyCanonicalName,
            Postcode: request.Postcode,
            LocationName: request.LocationName,
            LocationDisplayName: request.LocationDisplayName,
            LocationAreaJson: request.LocationAreaJson,
            Latitude: request.Latitude,
            Longitude: request.Longitude,
            CategoryTag: request.CategoryTag,
            CategoryLabel: request.CategoryLabel,
            SalaryMin: request.SalaryMin,
            SalaryMax: request.SalaryMax,
            SalaryCurrency: request.SalaryCurrency,
            SalaryIsPredicted: request.SalaryIsPredicted,
            ContractTime: request.ContractTime,
            ContractType: request.ContractType,
            IsFullTime: request.IsFullTime,
            IsPartTime: request.IsPartTime,
            IsPermanent: request.IsPermanent,
            IsContract: request.IsContract,
            IsRemote: request.IsRemote,
            PostedAtUtc: request.PostedAtUtc,
            ImportedAtUtc: request.ImportedAtUtc,
            LastSeenAtUtc: request.LastSeenAtUtc,
            IsHidden: request.IsHidden,
            RawPayloadJson: request.RawPayloadJson);

    public static ImportJobsFromProviderCommand ToImportFromProviderCommand(ImportJobsFromProviderRequest request)
        => new(
            PageIndex: request.PageIndex,
            PageSize: request.PageSize,
            Where: request.Where,
            Keyword: request.Keyword,
            DistanceKilometers: request.DistanceKilometers,
            MaxDaysOld: request.MaxDaysOld,
            Category: request.Category,
            Provider: request.Provider,
            ExcludedKeyword: request.ExcludedKeyword,
            SalaryMin: request.SalaryMin,
            SalaryMax: request.SalaryMax);

    public static ImportJobsFromJsonCommand ToImportFromJsonCommand(Stream jsonStream, string fileName)
        => new(JsonStream: jsonStream, FileName: fileName);

    public static HideJobsCommand ToHideCommand(IEnumerable<long> ids)
        => new(Ids: ids.Distinct().ToArray());

    public static DeleteJobsCommand ToDeleteCommand(IEnumerable<long> ids)
        => new(Ids: ids.Distinct().ToArray());
}
