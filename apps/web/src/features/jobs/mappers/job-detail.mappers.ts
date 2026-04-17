import type { JobDetailsResponseDto } from "@/api/jobs/jobs.types";
import type { JobDetailModel } from "@/features/jobs/types/job.types";

export function mapJobDetail(job: JobDetailsResponseDto): JobDetailModel {
  return {
    id: String(job.id),
    title: job.title,
    employer: job.companyDisplayName ?? job.company,
    location: job.isRemote
      ? `${job.locationDisplayName ?? job.locationName} · Remote`
      : (job.locationDisplayName ?? job.locationName),
    postcode: job.postcode,
    source: job.sourceName,
    summary: job.summary,
    description: job.description.trim() || job.summary,
    url: job.url,
    postedDate: formatPostedDate(job.postedAtUtc),
    salary: formatSalary(job.salaryMin, job.salaryMax, job.salaryCurrency),
    type: formatJobType(job.contractType, job.isPermanent, job.isContract, job.isFullTime, job.isPartTime),
    isApplied: job.isApplied,
    applicationId: job.applicationId
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
