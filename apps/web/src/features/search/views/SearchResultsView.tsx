import { useCallback, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { SearchResults } from "@/features/search/components/SearchResults";
import { SearchResultsToolbar } from "@/features/search/components/SearchResultsToolbar";
import { useJobSearch } from "@/features/search/hooks/useJobSearch";
import {
  createSearchParams,
  readSearchCriteria
} from "@/features/search/lib/search-query";
import type { SearchSortBy, SearchViewMode } from "@/features/search/types/search.types";

export function SearchResultsView() {
  const [searchParams, setSearchParams] = useSearchParams();
  const criteria = readSearchCriteria(searchParams);
  const { status, data, errorMessage } = useJobSearch(criteria);
  const [viewMode, setViewModeState] = useState<SearchViewMode>(() => {
    const stored = localStorage.getItem("search:viewMode");
    return stored === "table" ? "table" : "card";
  });

  const setViewMode = useCallback((mode: SearchViewMode) => {
    localStorage.setItem("search:viewMode", mode);
    setViewModeState(mode);
  }, []);

  function updateCriteria(nextCriteria: typeof criteria) {
    setSearchParams(createSearchParams(nextCriteria));
  }

  return (
    <div className="min-h-screen bg-background">
      <AppHeader />

      <div className="sticky top-[73px] z-40 border-b border-divider bg-background">
        <div className="mx-auto max-w-7xl px-5 py-4 sm:px-8">
          <SearchResultsToolbar
            key={`${criteria.keyword}|${criteria.postcode}|${criteria.company}`}
            initialKeyword={criteria.keyword}
            initialPostcode={criteria.postcode}
            initialCompany={criteria.company}
            sortBy={criteria.sortBy}
            viewMode={viewMode}
            onSearch={(nextKeyword, nextPostcode, nextCompany) => {
              updateCriteria({
                ...criteria,
                keyword: nextKeyword,
                postcode: nextPostcode,
                company: nextCompany,
                pageIndex: 0
              });
            }}
            onSortChange={(sortBy: SearchSortBy) => {
              updateCriteria({ ...criteria, sortBy, pageIndex: 0 });
            }}
            onViewModeChange={setViewMode}
          />
        </div>
      </div>

      <div className="mx-auto max-w-7xl px-5 py-8 sm:px-8">
        <main>
          <SearchResults
            status={status}
            errorMessage={errorMessage}
            results={data?.jobs ?? []}
            totalCount={data?.totalCount ?? 0}
            keyword={criteria.keyword}
            postcode={criteria.postcode}
            viewMode={viewMode}
            pageIndex={data?.pageIndex ?? criteria.pageIndex}
            pageSize={data?.pageSize ?? criteria.pageSize}
            onPageChange={(pageIndex) => {
              updateCriteria({
                ...criteria,
                pageIndex
              });
            }}
            onPageSizeChange={(pageSize) => {
              updateCriteria({
                ...criteria,
                pageSize,
                pageIndex: 0
              });
            }}
          />
        </main>
      </div>
    </div>
  );
}
