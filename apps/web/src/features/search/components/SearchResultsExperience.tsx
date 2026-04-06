import { useEffect, useState } from "react";
import { Button } from "@mui/material";
import { useSearchParams } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { searchMockJobs } from "@/features/jobs/data/mockJobs";
import type { MockJob } from "@/features/jobs/types/job.types";
import { SearchInput } from "@/features/search/components/SearchInput";
import { SearchResults } from "@/features/search/components/SearchResults";

export function SearchResultsExperience() {
  const [searchParams, setSearchParams] = useSearchParams();
  const keyword = (searchParams.get("keyword") ?? "").trim();
  const postcode = (searchParams.get("postcode") ?? "").trim();
  const [isLoading, setIsLoading] = useState(true);
  const [results, setResults] = useState<MockJob[]>([]);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  useEffect(() => {
    const timeoutId = window.setTimeout(() => {
      if (keyword.toLowerCase() === "error") {
        setResults([]);
        setErrorMessage("Mock provider failure. Try another search term.");
        setIsLoading(false);
        return;
      }

      setResults(searchMockJobs(keyword, postcode));
      setIsLoading(false);
    }, 450);

    return () => {
      window.clearTimeout(timeoutId);
    };
  }, [keyword, postcode]);

  return (
    <div className="min-h-screen bg-background">
      <AppHeader />

      <div className="sticky top-[73px] z-40 border-b border-divider bg-background">
        <div className="mx-auto grid max-w-7xl gap-3 px-5 py-4 sm:px-8 lg:grid-cols-[minmax(0,1fr)_300px_auto]">
          <SearchToolbar
            key={`${keyword}|${postcode}`}
            initialKeyword={keyword}
            initialPostcode={postcode}
            onSearch={(nextKeyword, nextPostcode) => {
              const params = new URLSearchParams();

              if (nextKeyword.trim()) {
                params.set("keyword", nextKeyword.trim());
              }

              if (nextPostcode.trim()) {
                params.set("postcode", nextPostcode.trim());
              }

              setSearchParams(params);
            }}
          />
        </div>
      </div>

      <div className="mx-auto grid max-w-7xl gap-8 px-5 py-8 sm:px-8 lg:grid-cols-[240px_minmax(0,1fr)]">
        <aside className="space-y-6">
          <div>
            <h3 className="mb-3 font-mono text-sm text-metadata">FILTERS</h3>
            <div className="space-y-4">
              <FilterGroup title="Posted" options={["Last 24 hours", "Last 3 days", "Last week", "Anytime"]} />
              <FilterGroup title="Job type" options={["Full-time", "Part-time", "Contract", "Freelance"]} />
              <FilterGroup title="Salary" options={["GBP 40k+", "GBP 60k+", "GBP 80k+", "GBP 100k+"]} />
              <FilterGroup title="Source" options={["LinkedIn", "Indeed", "Company Site", "Reed"]} />
            </div>
          </div>

          <div className="rounded-lg bg-accent-secondary p-4">
            <p className="text-sm font-medium text-accent-secondary-foreground">Save this search</p>
            <p className="mt-2 text-xs text-accent-secondary-foreground/80">
              This mock layout leaves room for saved searches and alerts without requiring them yet.
            </p>
          </div>
        </aside>

        <main>
          <SearchResults
            isLoading={isLoading}
            errorMessage={errorMessage}
            results={results}
            keyword={keyword}
            postcode={postcode}
          />
        </main>
      </div>
    </div>
  );
}

function SearchToolbar({
  initialKeyword,
  initialPostcode,
  onSearch
}: {
  initialKeyword: string;
  initialPostcode: string;
  onSearch: (keyword: string, postcode: string) => void;
}) {
  const [draftKeyword, setDraftKeyword] = useState(initialKeyword);
  const [draftPostcode, setDraftPostcode] = useState(initialPostcode);

  function handleSearch() {
    onSearch(draftKeyword, draftPostcode);
  }

  return (
    <>
      <SearchInput
        ariaLabel="Search roles, companies, skills"
        placeholder="Search roles, companies, skills..."
        value={draftKeyword}
        onChange={setDraftKeyword}
        onSubmit={handleSearch}
      />
      <SearchInput
        ariaLabel="Postcode or area"
        placeholder="Postcode or area"
        value={draftPostcode}
        onChange={setDraftPostcode}
        onSubmit={handleSearch}
      />
      <Button
        variant="contained"
        onClick={handleSearch}
        sx={{
          minHeight: 48,
          bgcolor: "accent.main",
          "&:hover": { bgcolor: "accent.dark" }
        }}
      >
        Search
      </Button>
    </>
  );
}

function FilterGroup({ title, options }: { title: string; options: string[] }) {
  return (
    <div className="border-t border-divider pt-4 first:border-t-0 first:pt-0">
      <p className="mb-2 text-sm font-medium text-foreground">{title}</p>
      <div className="space-y-2">
        {options.map((option) => (
          <label
            key={option}
            className="flex items-center gap-2 text-sm text-foreground-secondary transition-colors hover:text-foreground"
          >
            <input type="checkbox" className="rounded border-input-border" />
            {option}
          </label>
        ))}
      </div>
    </div>
  );
}
