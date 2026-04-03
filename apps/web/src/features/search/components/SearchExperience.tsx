import { useState } from "react";
import { SearchForm } from "@/features/search/components/SearchForm";
import { SearchResults } from "@/features/search/components/SearchResults";
import { useJobSearch } from "@/features/search/hooks/useJobSearch";

export function SearchExperience() {
  const [lastSubmittedPostcode, setLastSubmittedPostcode] = useState("SW1A");
  const [lastSubmittedKeyword, setLastSubmittedKeyword] = useState(".NET");
  const { status, data, errorMessage, runSearch } = useJobSearch();

  async function handleSearch(postcode: string, keyword: string) {
    setLastSubmittedPostcode(postcode);
    setLastSubmittedKeyword(keyword);
    await runSearch(postcode, keyword);
  }

  return (
    <div className="grid gap-6 lg:grid-cols-[minmax(0,420px)_minmax(0,1fr)]">
      <SearchForm
        initialKeyword={lastSubmittedKeyword}
        initialPostcode={lastSubmittedPostcode}
        isSubmitting={status === "loading"}
        onSubmit={handleSearch}
      />
      <SearchResults
        data={data}
        errorMessage={errorMessage}
        keyword={lastSubmittedKeyword}
        postcode={lastSubmittedPostcode}
        status={status}
      />
    </div>
  );
}
