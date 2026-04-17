import { useQuery } from "@tanstack/react-query";
import { getJobsPage } from "@/api/jobs/jobs.api";
import type { JobCardModel } from "@/features/jobs/types/job.types";
import type { SearchCriteria, SearchSortBy, SearchStatus, SearchViewModel } from "@/features/search/types/search.types";
import { mapSearchResponse } from "@/features/search/mappers/search.mappers";

function sortJobs(jobs: JobCardModel[], sortBy: SearchSortBy): JobCardModel[] {
  const sorted = [...jobs];

  if (sortBy === "date-asc") {
    return sorted.sort((a, b) => a.postedDate.localeCompare(b.postedDate));
  }

  if (sortBy === "salary-desc") {
    return sorted.sort((a, b) => extractSalaryMax(b.salary) - extractSalaryMax(a.salary));
  }

  if (sortBy === "salary-asc") {
    return sorted.sort((a, b) => extractSalaryMax(a.salary) - extractSalaryMax(b.salary));
  }

  // date-desc (default): newest first — preserve API order which is already newest first
  return sorted;
}

function extractSalaryMax(salary: string | undefined): number {
  if (!salary) return -1;
  const numbers = salary.replace(/[^0-9]/g, " ").trim().split(/\s+/).map(Number).filter(Boolean);
  return numbers.length > 0 ? Math.max(...numbers) : -1;
}

export function useJobSearch({ postcode, keyword, company, sortBy, pageIndex, pageSize }: SearchCriteria) {
  const { data, isPending, isError, error } = useQuery<SearchViewModel>({
    queryKey: ["job-search", { postcode, keyword, company, sortBy, pageIndex, pageSize }],
    queryFn: async () => {
      const response = await getJobsPage({
        pageIndex,
        pageSize,
        postcode: postcode || undefined,
        keyword: keyword || undefined,
        company: company || undefined,
        isHidden: false
      });
      const viewModel = mapSearchResponse(response, { postcode, keyword });
      return { ...viewModel, jobs: sortJobs(viewModel.jobs, sortBy) };
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
