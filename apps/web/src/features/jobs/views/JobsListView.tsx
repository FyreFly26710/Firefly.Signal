import AddRoundedIcon from "@mui/icons-material/AddRounded";
import DeleteOutlineRoundedIcon from "@mui/icons-material/DeleteOutlineRounded";
import FilterListRoundedIcon from "@mui/icons-material/FilterListRounded";
import VisibilityOffRoundedIcon from "@mui/icons-material/VisibilityOffRounded";
import WorkOutlineRoundedIcon from "@mui/icons-material/WorkOutlineRounded";
import {
  Alert,
  Button,
  Checkbox,
  Chip,
  MenuItem,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  TextField
} from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { SectionCard } from "@/components/SectionCard";
import { deleteJobs, getJobsPage, hideJobs } from "@/api/jobs/jobs.api";
import type { DeleteJobsResponseDto, JobDetailsResponseDto } from "@/api/jobs/jobs.types";
import { useAsyncTask } from "@/lib/async/useAsyncTask";
import { useSessionStore } from "@/store/session.store";

type VisibilityFilter = "visible" | "hidden" | "all";

type JobsListFilters = {
  keyword: string;
  company: string;
  postcode: string;
  location: string;
  sourceName: string;
  categoryTag: string;
  visibility: VisibilityFilter;
};

const emptyFilters: JobsListFilters = {
  keyword: "",
  company: "",
  postcode: "",
  location: "",
  sourceName: "",
  categoryTag: "",
  visibility: "visible"
};

const pageSize = 20;

export function JobsListView() {
  const navigate = useNavigate();
  const user = useSessionStore((state) => state.user);
  const isAdmin = user?.role === "admin";
  const [draftFilters, setDraftFilters] = useState<JobsListFilters>(emptyFilters);
  const [filters, setFilters] = useState<JobsListFilters>(emptyFilters);
  const [pageIndex, setPageIndex] = useState(0);
  const [selectedIds, setSelectedIds] = useState<number[]>([]);
  const [actionMessage, setActionMessage] = useState<string | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);
  const [isProcessing, setIsProcessing] = useState(false);

  const loadJobs = useCallback(async (nextPageIndex: number, nextFilters: JobsListFilters) => {
    return getJobsPage({
      pageIndex: nextPageIndex,
      pageSize,
      keyword: normalizeText(nextFilters.keyword),
      company: normalizeText(nextFilters.company),
      postcode: normalizeText(nextFilters.postcode),
      location: normalizeText(nextFilters.location),
      sourceName: normalizeText(nextFilters.sourceName),
      categoryTag: normalizeText(nextFilters.categoryTag),
      isHidden: mapVisibilityToHiddenFlag(nextFilters.visibility)
    });
  }, []);

  const { status, data, errorMessage, execute } = useAsyncTask(loadJobs);

  useEffect(() => {
    void execute(pageIndex, filters);
  }, [execute, filters, pageIndex]);

  useEffect(() => {
    const visibleIds = new Set((data?.items ?? []).map((job) => job.id));
    setSelectedIds((current) => current.filter((id) => visibleIds.has(id)));
  }, [data]);

  const jobs = data?.items ?? [];
  const viewStatus = status === "success" && jobs.length === 0 ? "empty" : status;
  const totalCount = data?.totalCount ?? 0;
  const selectedVisibleCount = jobs.filter((job) => selectedIds.includes(job.id)).length;
  const allVisibleSelected = jobs.length > 0 && selectedVisibleCount === jobs.length;

  const selectionSummary = useMemo(() => {
    if (selectedIds.length === 0) {
      return "No jobs selected";
    }

    return `${selectedIds.length} selected`;
  }, [selectedIds]);

  async function handleHideSelected() {
    if (!isAdmin || selectedIds.length === 0) {
      return;
    }

    setIsProcessing(true);
    setActionMessage(null);
    setActionError(null);

    try {
      const result = await hideJobs(selectedIds);
      setActionMessage(buildHideSummary(result.hiddenCount, result.missingIds.length));
      setSelectedIds([]);
      await execute(pageIndex, filters);
    } catch (error) {
      setActionError(error instanceof Error ? error.message : "Unable to hide the selected jobs.");
    } finally {
      setIsProcessing(false);
    }
  }

  async function handleDeleteSelected() {
    if (!isAdmin || selectedIds.length === 0) {
      return;
    }

    if (!window.confirm("Delete the selected jobs? Any visible jobs will be rejected by the backend until they are hidden.")) {
      return;
    }

    setIsProcessing(true);
    setActionMessage(null);
    setActionError(null);

    try {
      const result = await deleteJobs(selectedIds);
      setActionMessage(buildDeleteSummary(result));
      setSelectedIds([]);
      await execute(pageIndex, filters);
    } catch (error) {
      setActionError(error instanceof Error ? error.message : "Unable to delete the selected jobs.");
    } finally {
      setIsProcessing(false);
    }
  }

  return (
    <div className="min-h-screen bg-background">
      <AppHeader
        variant="authenticated"
        actions={
          isAdmin ? (
            <Button
              onClick={() => void navigate("/admin/manage-jobs/new")}
              variant="contained"
              startIcon={<AddRoundedIcon />}
              sx={{
                bgcolor: "accent.main",
                "&:hover": { bgcolor: "accent.dark" }
              }}
            >
              New job
            </Button>
          ) : null
        }
      />

      <div className="mx-auto max-w-[1400px] px-5 py-8 sm:px-8">
        <div className="mb-6 flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
          <div>
            <p className="font-mono text-xs tracking-[0.2em] text-foreground-tertiary">
              ADMIN JOB MANAGEMENT
            </p>
            <h1 className="mt-2 font-serif text-4xl font-semibold text-foreground">
              Manage jobs
            </h1>
            <p className="mt-3 max-w-3xl text-base text-foreground-secondary">
              Browse more jobs in one screen, filter from the top toolbar, and use bulk hide or
              delete actions backed by the batch admin APIs.
            </p>
          </div>
          <div className="rounded-lg border border-border bg-background-elevated px-4 py-3 text-sm text-foreground-secondary">
            <span className="font-medium text-foreground">{totalCount}</span> total jobs
          </div>
        </div>

        {!isAdmin ? (
          <Alert severity="warning" sx={{ mb: 4 }}>
            This route is intended for admin job management. Your session can read jobs, but batch
            hide, batch delete, and edit actions require the admin role from the backend API.
          </Alert>
        ) : null}

        {actionMessage ? <Alert severity="success" sx={{ mb: 3 }}>{actionMessage}</Alert> : null}
        {actionError ? <Alert severity="error" sx={{ mb: 3 }}>{actionError}</Alert> : null}
        {viewStatus === "error" ? (
          <Alert severity="error" sx={{ mb: 3 }}>
            {errorMessage ?? "Unable to load jobs."}
          </Alert>
        ) : null}

        <SectionCard className="overflow-hidden">
          <div className="border-b border-divider p-5">
            <div className="flex flex-col gap-4">
              <div className="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
                <div className="flex items-center gap-2">
                  <FilterListRoundedIcon className="text-foreground-tertiary" />
                  <h2 className="font-serif text-2xl font-semibold text-foreground">Job table</h2>
                </div>
                <div className="flex flex-wrap items-center gap-3">
                  <span className="text-sm text-foreground-secondary">{selectionSummary}</span>
                  <Button
                    variant="outlined"
                    color="inherit"
                    startIcon={<VisibilityOffRoundedIcon />}
                    disabled={!isAdmin || selectedIds.length === 0 || isProcessing}
                    onClick={() => void handleHideSelected()}
                  >
                    {isProcessing ? "Working..." : "Hide selected"}
                  </Button>
                  <Button
                    variant="outlined"
                    color="error"
                    startIcon={<DeleteOutlineRoundedIcon />}
                    disabled={!isAdmin || selectedIds.length === 0 || isProcessing}
                    onClick={() => void handleDeleteSelected()}
                  >
                    {isProcessing ? "Working..." : "Delete selected"}
                  </Button>
                </div>
              </div>

              <div className="grid gap-3 md:grid-cols-3 xl:grid-cols-7">
                <TextField
                  label="Keyword"
                  size="small"
                  value={draftFilters.keyword}
                  onChange={(event) =>
                    setDraftFilters((current) => ({ ...current, keyword: event.target.value }))
                  }
                />
                <TextField
                  label="Company"
                  size="small"
                  value={draftFilters.company}
                  onChange={(event) =>
                    setDraftFilters((current) => ({ ...current, company: event.target.value }))
                  }
                />
                <TextField
                  label="Postcode"
                  size="small"
                  value={draftFilters.postcode}
                  onChange={(event) =>
                    setDraftFilters((current) => ({ ...current, postcode: event.target.value }))
                  }
                />
                <TextField
                  label="Location"
                  size="small"
                  value={draftFilters.location}
                  onChange={(event) =>
                    setDraftFilters((current) => ({ ...current, location: event.target.value }))
                  }
                />
                <TextField
                  label="Source"
                  size="small"
                  value={draftFilters.sourceName}
                  onChange={(event) =>
                    setDraftFilters((current) => ({ ...current, sourceName: event.target.value }))
                  }
                />
                <TextField
                  label="Category"
                  size="small"
                  value={draftFilters.categoryTag}
                  onChange={(event) =>
                    setDraftFilters((current) => ({ ...current, categoryTag: event.target.value }))
                  }
                />
                <TextField
                  select
                  label="Visibility"
                  size="small"
                  value={draftFilters.visibility}
                  onChange={(event) =>
                    setDraftFilters((current) => ({
                      ...current,
                      visibility: event.target.value as VisibilityFilter
                    }))
                  }
                >
                  <MenuItem value="visible">Visible</MenuItem>
                  <MenuItem value="hidden">Hidden</MenuItem>
                  <MenuItem value="all">All</MenuItem>
                </TextField>
              </div>

              <div className="flex flex-wrap gap-3">
                <Button
                  variant="contained"
                  onClick={() => {
                    setPageIndex(0);
                    setFilters(draftFilters);
                  }}
                  sx={{
                    bgcolor: "accent.main",
                    "&:hover": { bgcolor: "accent.dark" }
                  }}
                >
                  Apply filters
                </Button>
                <Button
                  variant="outlined"
                  color="inherit"
                  onClick={() => {
                    setDraftFilters(emptyFilters);
                    setPageIndex(0);
                    setFilters(emptyFilters);
                  }}
                >
                  Reset
                </Button>
              </div>
            </div>
          </div>

          {viewStatus === "loading" ? (
            <div className="p-6 text-sm text-foreground-secondary">Loading jobs from the API...</div>
          ) : null}

          {viewStatus === "empty" ? (
            <div className="p-10 text-center">
              <WorkOutlineRoundedIcon className="mx-auto text-4xl text-foreground-tertiary" />
              <h3 className="mt-4 font-serif text-2xl font-semibold text-foreground">
                No jobs matched those filters
              </h3>
              <p className="mt-3 text-sm text-foreground-secondary">
                Broaden the filters or add a new job resource.
              </p>
            </div>
          ) : null}

          {jobs.length > 0 ? (
            <>
              <TableContainer>
                <Table size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell padding="checkbox">
                        <Checkbox
                          checked={allVisibleSelected}
                          indeterminate={selectedVisibleCount > 0 && !allVisibleSelected}
                          onChange={(event) =>
                            setSelectedIds(event.target.checked ? jobs.map((job) => job.id) : [])
                          }
                          inputProps={{ "aria-label": "Select all jobs" }}
                        />
                      </TableCell>
                      <TableCell>Title</TableCell>
                      <TableCell>Company</TableCell>
                      <TableCell>Location</TableCell>
                      <TableCell>Source</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell>Posted</TableCell>
                      <TableCell>Type</TableCell>
                      <TableCell align="right">Salary</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {jobs.map((job) => {
                      const isSelected = selectedIds.includes(job.id);

                      return (
                        <TableRow
                          key={job.id}
                          hover
                          selected={isSelected}
                          onClick={() => void navigate(`/admin/manage-jobs/${job.id}`)}
                          sx={{ cursor: "pointer" }}
                        >
                          <TableCell
                            padding="checkbox"
                            onClick={(event) => {
                              event.stopPropagation();
                            }}
                          >
                            <Checkbox
                              checked={isSelected}
                              onChange={(event) => {
                                setSelectedIds((current) =>
                                  event.target.checked
                                    ? [...current, job.id]
                                    : current.filter((id) => id !== job.id)
                                );
                              }}
                              inputProps={{ "aria-label": `Select job ${job.id}` }}
                            />
                          </TableCell>
                          <TableCell>
                            <div className="max-w-[280px]">
                              <div className="font-medium text-foreground">{job.title}</div>
                              <div className="mt-1 truncate text-xs text-foreground-tertiary">
                                #{job.id} · {job.sourceJobId}
                              </div>
                            </div>
                          </TableCell>
                          <TableCell>{job.companyDisplayName ?? job.company}</TableCell>
                          <TableCell>
                            <div className="max-w-[180px] truncate">
                              {job.locationDisplayName ?? job.locationName}
                            </div>
                          </TableCell>
                          <TableCell>{job.sourceName}</TableCell>
                          <TableCell>
                            <div className="flex flex-wrap gap-1">
                              {job.isHidden ? (
                                <Chip size="small" label="Hidden" color="warning" variant="outlined" />
                              ) : (
                                <Chip size="small" label="Visible" color="success" variant="outlined" />
                              )}
                              {job.isRemote ? <Chip size="small" label="Remote" variant="outlined" /> : null}
                            </div>
                          </TableCell>
                          <TableCell>{formatDate(job.postedAtUtc)}</TableCell>
                          <TableCell>{formatJobType(job)}</TableCell>
                          <TableCell align="right">{formatSalary(job)}</TableCell>
                        </TableRow>
                      );
                    })}
                  </TableBody>
                </Table>
              </TableContainer>

              <TablePagination
                component="div"
                count={totalCount}
                page={pageIndex}
                onPageChange={(_, nextPage) => setPageIndex(nextPage)}
                rowsPerPage={pageSize}
                rowsPerPageOptions={[pageSize]}
              />
            </>
          ) : null}
        </SectionCard>
      </div>
    </div>
  );
}

function mapVisibilityToHiddenFlag(value: VisibilityFilter): boolean | undefined {
  if (value === "all") {
    return undefined;
  }

  return value === "hidden";
}

function normalizeText(value: string): string | undefined {
  const trimmed = value.trim();
  return trimmed ? trimmed : undefined;
}

function formatDate(value: string): string {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    return "Unknown";
  }

  return new Intl.DateTimeFormat("en-GB", {
    day: "numeric",
    month: "short",
    year: "numeric"
  }).format(date);
}

function formatSalary(job: JobDetailsResponseDto): string {
  if (job.salaryMin === null && job.salaryMax === null) {
    return "N/A";
  }

  const formatter = new Intl.NumberFormat("en-GB", {
    style: "currency",
    currency: job.salaryCurrency ?? "GBP",
    maximumFractionDigits: 0
  });

  if (job.salaryMin !== null && job.salaryMax !== null) {
    return `${formatter.format(job.salaryMin)} - ${formatter.format(job.salaryMax)}`;
  }

  return formatter.format(job.salaryMin ?? job.salaryMax ?? 0);
}

function formatJobType(job: JobDetailsResponseDto): string {
  if (job.contractType) {
    return job.contractType.replaceAll("_", " ");
  }

  if (job.isPermanent) {
    return "permanent";
  }

  if (job.isContract) {
    return "contract";
  }

  if (job.isFullTime) {
    return "full time";
  }

  if (job.isPartTime) {
    return "part time";
  }

  return "unspecified";
}

function buildHideSummary(hiddenCount: number, missingCount: number): string {
  if (missingCount > 0) {
    return `${hiddenCount} jobs hidden. ${missingCount} ids were missing.`;
  }

  return `${hiddenCount} jobs hidden successfully.`;
}

function buildDeleteSummary(result: DeleteJobsResponseDto): string {
  if (result.notHiddenIds.length > 0) {
    return `${result.deletedCount} jobs deleted. ${result.notHiddenIds.length} selected jobs still need to be hidden first.`;
  }

  if (result.missingIds.length > 0) {
    return `${result.deletedCount} jobs deleted. ${result.missingIds.length} ids were missing.`;
  }

  return `${result.deletedCount} jobs deleted successfully.`;
}
