import ChevronLeftRoundedIcon from "@mui/icons-material/ChevronLeftRounded";
import ChevronRightRoundedIcon from "@mui/icons-material/ChevronRightRounded";
import { Button, MenuItem, TextField } from "@mui/material";
import { useState } from "react";
import type { SearchStatus, SearchViewMode } from "@/features/search/types/search.types";
import { JobCard } from "@/features/jobs/components/JobCard";
import type { JobCardModel } from "@/features/jobs/types/job.types";
import { useJobState } from "@/features/search/hooks/useJobState";
import { JobSearchCompactTable } from "@/features/search/components/JobSearchCompactTable";

function JobCardWithState({ job }: { job: JobCardModel }) {
  const { isSaved, isHidden, isApplied, toggleSave, toggleHide, apply } = useJobState(job.id, {
    isSaved: job.isSaved,
    isHidden: job.isHidden,
    isApplied: job.isApplied
  });
  return (
    <JobCard
      job={job}
      isApplied={isApplied}
      onApply={() => { void apply(); }}
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
    state: useJobState(job.id, {
      isSaved: job.isSaved,
      isHidden: job.isHidden,
      isApplied: job.isApplied
    })
  }));

  const savedIds = new Set(states.filter((s) => s.state.isSaved).map((s) => s.job.id));
  const hiddenIds = new Set(states.filter((s) => s.state.isHidden).map((s) => s.job.id));
  const appliedIds = new Set(states.filter((s) => s.state.isApplied).map((s) => s.job.id));

  return (
    <JobSearchCompactTable
      jobs={jobs}
      savedIds={savedIds}
      hiddenIds={hiddenIds}
      appliedIds={appliedIds}
      onApply={(id) => {
        const entry = states.find((s) => s.job.id === id);
        if (entry) void entry.state.apply();
      }}
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
          Enter a role, skill or keyword to explore available opportunities.
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
          Try adjusting your keyword or filters to broaden the search.
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

      <PaginationBar
        pageIndex={pageIndex}
        pageSize={pageSize}
        totalCount={totalCount}
        onPageChange={onPageChange}
        onPageSizeChange={onPageSizeChange}
      />
    </>
  );
}

function PaginationBar({
  pageIndex,
  pageSize,
  totalCount,
  onPageChange,
  onPageSizeChange
}: {
  pageIndex: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (pageIndex: number) => void;
  onPageSizeChange: (pageSize: number) => void;
}) {
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
  const [draft, setDraft] = useState("");

  function commitJump() {
    const n = parseInt(draft, 10);
    if (!Number.isNaN(n) && n >= 1 && n <= totalPages) {
      onPageChange(n - 1);
    }
    setDraft("");
  }

  return (
    <div className="mt-6 flex flex-col gap-4 rounded-lg border border-border bg-background-elevated px-5 py-4 sm:flex-row sm:items-center sm:justify-between">
      <div className="text-sm text-foreground-secondary">
        Page <span className="font-medium text-foreground">{pageIndex + 1}</span> of{" "}
        <span className="font-medium text-foreground">{totalPages}</span>
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
        <TextField
          size="small"
          type="number"
          label="Go to page"
          value={draft}
          onChange={(e) => setDraft(e.target.value)}
          onKeyDown={(e) => { if (e.key === "Enter") commitJump(); }}
          onBlur={commitJump}
          inputProps={{ inputMode: "numeric", pattern: "[0-9]*", "aria-label": "Go to page" }}
          sx={{ width: 130 }}
        />
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
          disabled={pageIndex + 1 >= totalPages}
          onClick={() => onPageChange(pageIndex + 1)}
        >
          Next
        </Button>
      </div>
    </div>
  );
}
