import type { JobCardModel } from "@/features/jobs/types/job.types";
import type { JobCardDto, SearchJobsResponseDto, SearchViewModel } from "@/features/search/types/search.types";

export function mapSearchResponse(response: SearchJobsResponseDto): SearchViewModel {
  return {
    postcode: response.postcode,
    keyword: response.keyword,
    totalCount: response.totalCount,
    jobs: response.jobs.map(mapJobCard)
  };
}

function mapJobCard(job: JobCardDto): JobCardModel {
  return {
    id: String(job.id),
    title: job.title,
    employer: job.company,
    location: job.isRemote ? `${job.locationName} · Remote` : job.locationName,
    summary: job.summary,
    url: job.url,
    source: job.sourceName,
    postedDate: formatPostedDate(job.postedAtUtc)
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
