import { getJson } from "@/lib/http/client";
import type { SearchJobsResponseDto } from "@/api/job-search/job-search.types";

export async function searchJobs(postcode: string, keyword: string): Promise<SearchJobsResponseDto> {
  const query = new URLSearchParams({
    postcode,
    keyword,
    pageIndex: "0",
    pageSize: "20"
  });

  return getJson<SearchJobsResponseDto>(`/api/job-search/search?${query.toString()}`);
}
