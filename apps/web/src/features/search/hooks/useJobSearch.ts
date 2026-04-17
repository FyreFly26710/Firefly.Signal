import { useQuery } from "@tanstack/react-query";
import { searchJobsPage } from "@/api/jobs/jobs.api";
import type { SearchCriteria, SearchStatus, SearchViewModel } from "@/features/search/types/search.types";
import { mapSearchResponse } from "@/features/search/mappers/search.mappers";

export function useJobSearch({ keyword, where, salaryMin, salaryMax, datePosted, sortBy, isAsc, pageIndex, pageSize }: SearchCriteria) {
  const { data, isPending, isError, error } = useQuery<SearchViewModel>({
    queryKey: ["job-search", { keyword, where, salaryMin, salaryMax, datePosted, sortBy, isAsc, pageIndex, pageSize }],
    queryFn: async () => {
      const response = await searchJobsPage({
        pageIndex,
        pageSize,
        keyword: keyword || undefined,
        where: where || undefined,
        salaryMin: salaryMin ?? undefined,
        salaryMax: salaryMax ?? undefined,
        datePosted: datePosted ?? undefined,
        sortBy,
        isAsc
      });
      return mapSearchResponse(response, { keyword });
    },
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
