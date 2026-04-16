import { useQuery } from "@tanstack/react-query";
import { getJobsPage } from "@/api/jobs/jobs.api";
import type { SearchCriteria, SearchStatus, SearchViewModel } from "@/features/search/types/search.types";
import { mapSearchResponse } from "@/features/search/mappers/search.mappers";

export function useJobSearch({ postcode, keyword, pageIndex, pageSize }: SearchCriteria) {
  const { data, isPending, isError, error } = useQuery<SearchViewModel>({
    queryKey: ["job-search", { postcode, keyword, pageIndex, pageSize }],
    queryFn: async () =>
      mapSearchResponse(
        await getJobsPage({
          pageIndex,
          pageSize,
          postcode: postcode || undefined,
          keyword: keyword || undefined,
          isHidden: false
        }),
        { postcode, keyword }
      ),
    staleTime: 30_000
  });

  const status: SearchStatus = isPending
    ? "loading"
    : isError
      ? "error"
      : (data?.jobs.length ?? 0) === 0
        ? "empty"
        : "success";

  return {
    status,
    data: data ?? null,
    errorMessage: isError ? (error instanceof Error ? error.message : "Search failed.") : null
  };
}
