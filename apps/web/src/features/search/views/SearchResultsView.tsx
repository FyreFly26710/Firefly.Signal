import { useSearchParams } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { SearchResults } from "@/features/search/components/SearchResults";
import { SearchResultsToolbar } from "@/features/search/components/SearchResultsToolbar";
import { useJobSearch } from "@/features/search/hooks/useJobSearch";
import {
  createSearchParams,
  readSearchCriteria
} from "@/features/search/lib/search-query";

export function SearchResultsView() {
  const [searchParams, setSearchParams] = useSearchParams();
  const criteria = readSearchCriteria(searchParams);
  const { status, data, errorMessage } = useJobSearch(criteria);

  function updateCriteria(nextCriteria: typeof criteria) {
    setSearchParams(createSearchParams(nextCriteria));
  }

  return (
    <div className="min-h-screen bg-background">
      <AppHeader />

      <div className="sticky top-[73px] z-40 border-b border-divider bg-background">
        <div className="mx-auto grid max-w-7xl gap-3 px-5 py-4 sm:px-8 lg:grid-cols-[minmax(0,1fr)_300px_auto]">
          <SearchResultsToolbar
            key={`${criteria.keyword}|${criteria.postcode}`}
            initialKeyword={criteria.keyword}
            initialPostcode={criteria.postcode}
            onSearch={(nextKeyword, nextPostcode) => {
              updateCriteria({
                ...criteria,
                keyword: nextKeyword,
                postcode: nextPostcode,
                pageIndex: 0
              });
            }}
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
