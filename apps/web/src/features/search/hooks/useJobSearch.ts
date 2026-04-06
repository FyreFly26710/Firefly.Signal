import { useEffect, useState } from "react";
import type { SearchCriteria, SearchStatus, SearchViewModel } from "@/features/search/types/search.types";
import { searchJobs } from "@/features/search/api/search.api";
import { ApiError } from "@/lib/http/api-error";

type SearchState = {
  status: SearchStatus;
  data: SearchViewModel | null;
  errorMessage: string | null;
};

export const initialSearchState: SearchState = {
  status: "idle",
  data: null,
  errorMessage: null
};

export function useJobSearch({ postcode, keyword }: SearchCriteria) {
  const [state, setState] = useState<SearchState>(initialSearchState);

  useEffect(() => {
    if (!postcode && !keyword) {
      setState(initialSearchState);
      return;
    }

    let isCancelled = false;

    async function loadSearchResults() {
      setState((current) => ({
        ...current,
        status: "loading",
        errorMessage: null
      }));

      try {
        const data = await searchJobs(postcode, keyword);

        if (isCancelled) {
          return;
        }

        setState({
          status: data.jobs.length === 0 ? "empty" : "success",
          data,
          errorMessage: null
        });
      } catch (error) {
        if (isCancelled) {
          return;
        }

        setState({
          status: "error",
          data: null,
          errorMessage: error instanceof ApiError ? error.message : "Something went wrong while searching."
        });
      }
    }

    void loadSearchResults();

    return () => {
      isCancelled = true;
    };
  }, [postcode, keyword]);

  return state;
}
