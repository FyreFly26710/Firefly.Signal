import ChevronLeftRoundedIcon from "@mui/icons-material/ChevronLeftRounded";
import ChevronRightRoundedIcon from "@mui/icons-material/ChevronRightRounded";
import { Button, MenuItem, TextField } from "@mui/material";
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
  pageIndex: number;
  pageSize: number;
  onPageChange: (pageIndex: number) => void;
  onPageSizeChange: (pageSize: number) => void;
};

export function SearchResults({
  status,
  errorMessage,
  results,
  totalCount,
  keyword,
  postcode,
  pageIndex,
  pageSize,
  onPageChange,
  onPageSizeChange
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

      <div className="mt-6 flex flex-col gap-4 rounded-lg border border-border bg-background-elevated px-5 py-4 sm:flex-row sm:items-center sm:justify-between">
        <div className="text-sm text-foreground-secondary">
          Page <span className="font-medium text-foreground">{pageIndex + 1}</span> of{" "}
          <span className="font-medium text-foreground">{Math.max(1, Math.ceil(totalCount / pageSize))}</span>
        </div>
        <div className="flex flex-wrap items-center gap-3">
          <TextField
            select
            size="small"
            label="Per page"
            value={String(pageSize)}
            onChange={(event) => onPageSizeChange(Number(event.target.value))}
            sx={{ minWidth: 120 }}
          >
            <MenuItem value="20">20</MenuItem>
            <MenuItem value="50">50</MenuItem>
            <MenuItem value="100">100</MenuItem>
          </TextField>
          <Button
            variant="outlined"
            color="inherit"
            startIcon={<ChevronLeftRoundedIcon />}
            disabled={pageIndex === 0}
            onClick={() => onPageChange(pageIndex - 1)}
          >
            Previous
          </Button>
          <Button
            variant="outlined"
            color="inherit"
            endIcon={<ChevronRightRoundedIcon />}
            disabled={pageIndex + 1 >= Math.max(1, Math.ceil(totalCount / pageSize))}
            onClick={() => onPageChange(pageIndex + 1)}
          >
            Next
          </Button>
        </div>
      </div>
    </>
  );
}
