import { getJobsPage } from "@/api/jobs/jobs.api";
import { useCallback, useEffect } from "react";
import type { SearchCriteria, SearchStatus, SearchViewModel } from "@/features/search/types/search.types";
import { mapSearchResponse } from "@/features/search/mappers/search.mappers";
import { createAsyncState, type AsyncState } from "@/lib/async/async-state";
import { useAsyncTask } from "@/lib/async/useAsyncTask";

type SearchState = AsyncState<SearchViewModel, SearchStatus>;

export const initialSearchState: SearchState = createAsyncState("idle");

export function useJobSearch({ postcode, keyword, pageIndex, pageSize }: SearchCriteria) {
  const runSearch = useCallback(
    async (nextPostcode: string, nextKeyword: string, nextPageIndex: number, nextPageSize: number) =>
      mapSearchResponse(
        await getJobsPage({
          pageIndex: nextPageIndex,
          pageSize: nextPageSize,
          postcode: nextPostcode || undefined,
          keyword: nextKeyword || undefined,
          isHidden: false
        }),
        { postcode: nextPostcode, keyword: nextKeyword }
      ),
    []
  );
  const { status, data, errorMessage, execute } = useAsyncTask(runSearch);

  useEffect(() => {
    void execute(postcode, keyword, pageIndex, pageSize);
  }, [execute, pageIndex, pageSize, postcode, keyword]);

  return {
    status: status === "success" && data?.jobs.length === 0 ? "empty" : status,
    data,
    errorMessage
  } satisfies SearchState;
}
