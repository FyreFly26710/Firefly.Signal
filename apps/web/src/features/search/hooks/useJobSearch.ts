import { useEffect } from "react";
import type { SearchCriteria, SearchStatus, SearchViewModel } from "@/features/search/types/search.types";
import { searchJobs } from "@/features/search/api/search.api";
import { createAsyncState, type AsyncState } from "@/lib/async/async-state";
import { useAsyncTask } from "@/lib/async/useAsyncTask";

type SearchState = AsyncState<SearchViewModel, SearchStatus>;

export const initialSearchState: SearchState = createAsyncState("idle");

export function useJobSearch({ postcode, keyword }: SearchCriteria) {
  const { status, data, errorMessage, execute } = useAsyncTask(searchJobs);
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
