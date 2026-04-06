import { useSearchParams } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { SearchFiltersPanel } from "@/features/search/components/SearchFiltersPanel";
import { SearchResults } from "@/features/search/components/SearchResults";
import { SearchResultsToolbar } from "@/features/search/components/SearchResultsToolbar";
import {
  createSearchParams,
  hasSearchCriteria,
  readSearchCriteria
} from "@/features/search/lib/search-query";
import { useJobSearch } from "@/features/search/hooks/useJobSearch";

export function SearchResultsExperience() {
  const [searchParams, setSearchParams] = useSearchParams();
  const criteria = readSearchCriteria(searchParams);
  const { status, data, errorMessage } = useJobSearch(criteria);

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
              setSearchParams(createSearchParams({ keyword: nextKeyword, postcode: nextPostcode }));
            }}
          />
        </div>
      </div>

      <div className="mx-auto grid max-w-7xl gap-8 px-5 py-8 sm:px-8 lg:grid-cols-[240px_minmax(0,1fr)]">
        <SearchFiltersPanel />

        <main>
          <SearchResults
            status={hasSearchCriteria(criteria) ? status : "idle"}
            errorMessage={errorMessage}
            results={data?.jobs ?? []}
            totalCount={data?.totalCount ?? 0}
            keyword={criteria.keyword}
            postcode={criteria.postcode}
          />
        </main>
      </div>
    </div>
  );
}
