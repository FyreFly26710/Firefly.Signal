import type { JobSearchResultDto, JobsPageResponseDto } from "@/api/jobs/jobs.types";
import type { JobCardModel } from "@/features/jobs/types/job.types";
import type { SearchViewModel } from "@/features/search/types/search.types";

export function mapSearchResponse(
  response: JobsPageResponseDto,
  criteria: { postcode: string; keyword: string }
): SearchViewModel {
  return {
    postcode: criteria.postcode,
    keyword: criteria.keyword,
    pageIndex: response.pageIndex,
    pageSize: response.pageSize,
    totalCount: response.totalCount,
    jobs: response.items.map(mapJobCard)
  };
}

function mapJobCard(job: JobSearchResultDto): JobCardModel {
  return {
    id: String(job.id),
    title: job.title,
    employer: job.companyDisplayName ?? job.company,
    location: job.isRemote
      ? `${job.locationDisplayName ?? job.locationName} · Remote`
      : (job.locationDisplayName ?? job.locationName),
    summary: job.summary,
    url: job.url,
    source: job.sourceName,
    postedDate: formatPostedDate(job.postedAtUtc),
    salary: formatSalary(job.salaryMin, job.salaryMax, job.salaryCurrency),
    type: formatJobType(job.contractType, job.isPermanent, job.isContract, job.isFullTime, job.isPartTime),
    isSaved: job.isSaved,
    isHidden: job.isUserHidden
  };
}

function formatPostedDate(value: string): string {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    return "Date unavailable";
  }

  return new Intl.DateTimeFormat("en-GB", {
    day: "numeric",
    month: "short",
    year: "numeric"
  }).format(date);
}

function formatSalary(
  salaryMin: number | null,
  salaryMax: number | null,
  salaryCurrency: string | null
): string | undefined {
  if (salaryMin === null && salaryMax === null) {
    return undefined;
  }

  const formatter = new Intl.NumberFormat("en-GB", {
    style: "currency",
    currency: salaryCurrency ?? "GBP",
    maximumFractionDigits: 0
  });

  if (salaryMin !== null && salaryMax !== null) {
    return `${formatter.format(salaryMin)} - ${formatter.format(salaryMax)}`;
  }

  return formatter.format(salaryMin ?? salaryMax ?? 0);
}

function formatJobType(
  contractType: string | null,
  isPermanent: boolean,
  isContract: boolean,
  isFullTime: boolean,
  isPartTime: boolean
): string | undefined {
  if (contractType) {
    return contractType.replaceAll("_", " ");
  }

  if (isPermanent) {
    return "permanent";
  }

  if (isContract) {
    return "contract";
  }

  if (isFullTime) {
    return "full time";
  }

  if (isPartTime) {
    return "part time";
  }

  return undefined;
}
