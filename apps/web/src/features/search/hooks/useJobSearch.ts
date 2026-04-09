import { getJobsPage } from "@/api/jobs/jobs.api";
import { useCallback, useEffect } from "react";
import type { SearchCriteria, SearchStatus, SearchViewModel } from "@/features/search/types/search.types";
import { mapSearchResponse } from "@/features/search/mappers/search.mappers";
import { createAsyncState, type AsyncState } from "@/lib/async/async-state";
import { useAsyncTask } from "@/lib/async/useAsyncTask";

type SearchState = AsyncState<SearchViewModel, SearchStatus>;

export const initialSearchState: SearchState = createAsyncState("idle");

export function useJobSearch({ postcode, keyword }: SearchCriteria) {
  const runSearch = useCallback(
    async (nextPostcode: string, nextKeyword: string) =>
      mapSearchResponse(
        await getJobsPage({
          pageIndex: 0,
          pageSize: 20,
          postcode: nextPostcode || undefined,
          keyword: nextKeyword || undefined,
          isHidden: false
        }),
        { postcode: nextPostcode, keyword: nextKeyword }
      ),
    []
  );
  const { status, data, errorMessage, execute } = useAsyncTask(runSearch);
  const hasCriteria = Boolean(postcode || keyword);

  useEffect(() => {
    if (!hasCriteria) {
      return;
    }

    void execute(postcode, keyword);
  }, [execute, hasCriteria, postcode, keyword]);

  if (!hasCriteria) {
    return initialSearchState;
  }

  return {
    status: status === "success" && data?.jobs.length === 0 ? "empty" : status,
    data,
    errorMessage
  } satisfies SearchState;
}
