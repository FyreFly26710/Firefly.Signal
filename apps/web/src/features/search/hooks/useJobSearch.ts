import { useState } from "react";
import type { SearchStatus, SearchViewModel } from "@/features/search/types/search.types";
import { searchJobs } from "@/features/search/api/search.api";
import { ApiError } from "@/lib/http/api-error";

type SearchState = {
  status: SearchStatus;
  data: SearchViewModel | null;
  errorMessage: string | null;
};

const initialState: SearchState = {
  status: "idle",
  data: null,
  errorMessage: null
};

export function useJobSearch() {
  const [state, setState] = useState<SearchState>(initialState);

  async function runSearch(postcode: string, keyword: string) {
    setState((current) => ({
      ...current,
      status: "loading",
      errorMessage: null
    }));

    try {
      const data = await searchJobs(postcode, keyword);
      setState({
        status: data.jobs.length === 0 ? "empty" : "success",
        data,
        errorMessage: null
      });
    } catch (error) {
      setState({
        status: "error",
        data: null,
        errorMessage: error instanceof ApiError ? error.message : "Something went wrong while searching."
      });
    }
  }

  return {
    ...state,
    runSearch
  };
}
