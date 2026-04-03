import { mapSearchResponse } from "@/features/search/mappers/search.mappers";
import type { SearchJobsResponseDto, SearchViewModel } from "@/features/search/types/search.types";
import { getJson } from "@/lib/http/client";

export async function searchJobs(postcode: string, keyword: string): Promise<SearchViewModel> {
  const query = new URLSearchParams({
    postcode,
    keyword,
    pageIndex: "0",
    pageSize: "20"
  });

  const response = await getJson<SearchJobsResponseDto>(`/api/job-search/search?${query.toString()}`);
  return mapSearchResponse(response);
}
