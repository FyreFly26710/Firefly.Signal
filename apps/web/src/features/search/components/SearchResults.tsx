import ChevronLeftRoundedIcon from "@mui/icons-material/ChevronLeftRounded";
import ChevronRightRoundedIcon from "@mui/icons-material/ChevronRightRounded";
import { Button, MenuItem, TextField } from "@mui/material";
import type { SearchStatus, SearchViewMode } from "@/features/search/types/search.types";
import { JobCard } from "@/features/jobs/components/JobCard";
import type { JobCardModel } from "@/features/jobs/types/job.types";
import { useJobState } from "@/features/search/hooks/useJobState";
import { JobSearchCompactTable } from "@/features/search/components/JobSearchCompactTable";

function JobCardWithState({ job }: { job: JobCardModel }) {
  const { isSaved, isHidden, toggleSave, toggleHide } = useJobState(job.id, {
    isSaved: job.isSaved,
    isHidden: job.isHidden
  });
  return (
    <JobCard
      job={job}
      isSaved={isSaved}
      isHidden={isHidden}
      onToggleSave={() => { void toggleSave(); }}
      onToggleHide={() => { void toggleHide(); }}
    />
  );
}

function JobTableWithState({ jobs }: { jobs: JobCardModel[] }) {
  // Collect per-job state. We use a map of id -> hook result by rendering one
  // stateful row component per job so hooks are called at the top level.
  return <JobTableRows jobs={jobs} />;
}

function JobTableRows({ jobs }: { jobs: JobCardModel[] }) {
  const states = jobs.map((job) => ({
    job,
    // eslint-disable-next-line react-hooks/rules-of-hooks
    state: useJobState(job.id, { isSaved: job.isSaved, isHidden: job.isHidden })
  }));

  const savedIds = new Set(states.filter((s) => s.state.isSaved).map((s) => s.job.id));
  const hiddenIds = new Set(states.filter((s) => s.state.isHidden).map((s) => s.job.id));

  return (
    <JobSearchCompactTable
      jobs={jobs}
      savedIds={savedIds}
      hiddenIds={hiddenIds}
      onToggleSave={(id) => {
        const entry = states.find((s) => s.job.id === id);
        if (entry) void entry.state.toggleSave();
      }}
      onToggleHide={(id) => {
        const entry = states.find((s) => s.job.id === id);
        if (entry) void entry.state.toggleHide();
      }}
    />
  );
}

type SearchResultsProps = {
  status: SearchStatus;
  errorMessage: string | null;
  results: JobCardModel[];
  totalCount: number;
  keyword: string;
  postcode: string;
  viewMode: SearchViewMode;
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
  viewMode,
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
          Try adjusting your keyword or postcode to broaden the search.
        </p>
      </div>
    );
  }

  return (
    <>

      {viewMode === "table" ? (
        <JobTableWithState jobs={results} />
      ) : (
        <div className="overflow-hidden rounded-lg border border-border bg-background-elevated">
          {results.map((job) => (
            <JobCardWithState key={job.id} job={job} />
          ))}
        </div>
      )}

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
