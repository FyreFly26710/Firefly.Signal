import { getJson } from "@/lib/http/client";
import type { SearchJobsResponseDto } from "@/api/job-search/job-search.types";

export type JobSearchProvider = "adzuna";

export async function searchJobs(postcode: string, keyword: string, provider: JobSearchProvider = "adzuna"): Promise<SearchJobsResponseDto> {
  const query = new URLSearchParams({
    postcode,
    keyword,
    pageIndex: "0",
    pageSize: "20",
    provider
  });

  return getJson<SearchJobsResponseDto>(`/api/job-search/search?${query.toString()}`);
}
