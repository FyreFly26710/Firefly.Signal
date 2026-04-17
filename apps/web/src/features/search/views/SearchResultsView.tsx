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
            key={`${criteria.keyword}|${criteria.where}|${criteria.salaryMin}|${criteria.salaryMax}|${criteria.datePosted}|${criteria.sortBy}|${String(criteria.isAsc)}`}
            initialKeyword={criteria.keyword}
            initialWhere={criteria.where}
            initialSalaryMin={criteria.salaryMin}
            initialSalaryMax={criteria.salaryMax}
            datePosted={criteria.datePosted}
            sortBy={criteria.sortBy}
            isAsc={criteria.isAsc}
            viewMode={viewMode}
            onSearch={(keyword, where, salaryMin, salaryMax) => {
              updateCriteria({ ...criteria, keyword, where, salaryMin, salaryMax, pageIndex: 0 });
            }}
            onClear={() => {
              updateCriteria({ keyword: "", where: "", salaryMin: null, salaryMax: null, datePosted: null, sortBy: "date", isAsc: false, pageIndex: 0, pageSize: criteria.pageSize });
            }}
            onDatePostedChange={(datePosted) => {
              updateCriteria({ ...criteria, datePosted, pageIndex: 0 });
            }}
            onSortChange={(sortBy: SearchSortBy) => {
              updateCriteria({ ...criteria, sortBy, pageIndex: 0 });
            }}
            onIsAscChange={(isAsc) => {
              updateCriteria({ ...criteria, isAsc, pageIndex: 0 });
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
            viewMode={viewMode}
            pageIndex={data?.pageIndex ?? criteria.pageIndex}
            pageSize={data?.pageSize ?? criteria.pageSize}
            onPageChange={(pageIndex) => {
              updateCriteria({ ...criteria, pageIndex });
            }}
            onPageSizeChange={(pageSize) => {
              updateCriteria({ ...criteria, pageSize, pageIndex: 0 });
            }}
          />
        </main>
      </div>
    </div>
  );
}
