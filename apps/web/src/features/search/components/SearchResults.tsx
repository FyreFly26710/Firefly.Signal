import type { SearchStatus } from "@/features/search/types/search.types";
import { JobCard } from "@/features/jobs/components/JobCard";
import type { JobCardModel } from "@/features/jobs/types/job.types";

type SearchResultsProps = {
  status: SearchStatus;
  errorMessage: string | null;
  results: JobCardModel[];
  totalCount: number;
  keyword: string;
  postcode: string;
};

export function SearchResults({
  status,
  errorMessage,
  results,
  totalCount,
  keyword,
  postcode
}: SearchResultsProps) {
  if (status === "idle") {
    return (
      <div className="rounded-lg border border-border bg-background-elevated px-6 py-14 text-center">
        <h3 className="font-serif text-2xl font-semibold text-foreground">Start a search</h3>
        <p className="mx-auto mt-3 max-w-xl text-foreground-secondary">
          Enter a role, company, skill, or postcode to explore available opportunities.
        </p>
      </div>
    );
  }

  if (status === "loading") {
    return (
      <div className="flex flex-col items-center justify-center rounded-lg border border-border bg-background-elevated py-20">
        <div className="h-8 w-8 animate-spin rounded-full border-2 border-accent-primary border-t-transparent" />
        <p className="mt-4 text-foreground-secondary">Searching across job sources...</p>
      </div>
    );
  }

  if (errorMessage) {
    return (
      <div className="rounded-lg border border-destructive/20 bg-background-elevated px-6 py-10 text-center">
        <h3 className="font-serif text-2xl font-semibold text-foreground">Search failed</h3>
        <p className="mt-3 text-foreground-secondary">{errorMessage}</p>
      </div>
    );
  }

  if (!results.length) {
    return (
      <div className="rounded-lg border border-border bg-background-elevated px-6 py-14 text-center">
        <h3 className="font-serif text-2xl font-semibold text-foreground">No results found</h3>
        <p className="mx-auto mt-3 max-w-xl text-foreground-secondary">
          Try adjusting your search terms or filters. You can search by role, company, skill, or
          location using the mock data set.
        </p>
      </div>
    );
  }

  return (
    <>
      <div className="mb-6">
        <h2 className="font-serif text-3xl font-semibold text-foreground">
          {keyword || postcode ? (
            <>
              {keyword || "All roles"}
              {keyword && postcode ? <span className="text-foreground-tertiary"> in </span> : null}
              {postcode || null}
            </>
          ) : (
            "All jobs"
          )}
        </h2>
        <p className="mt-2 text-sm text-foreground-secondary">
          <span className="font-medium text-foreground">{totalCount}</span> opportunities found
          {keyword || postcode ? " matching your search" : ""}
        </p>
      </div>

      <div className="overflow-hidden rounded-lg border border-border bg-background-elevated">
        {results.map((job) => (
          <JobCard key={job.id} job={job} />
        ))}
      </div>
    </>
  );
}
