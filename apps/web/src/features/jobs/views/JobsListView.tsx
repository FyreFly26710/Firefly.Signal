import AddRoundedIcon from "@mui/icons-material/AddRounded";
import WorkOutlineRoundedIcon from "@mui/icons-material/WorkOutlineRounded";
import { Alert, Button } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { SectionCard } from "@/components/SectionCard";
import { deleteJobs, getJobsPage, hideJobs } from "@/api/jobs/jobs.api";
import type { DeleteJobsResponseDto } from "@/api/jobs/jobs.types";
import { JobsManagementHeader } from "@/features/jobs/components/JobsManagementHeader";
import {
  JobsManagementToolbar,
  type JobsListFilters,
  type VisibilityFilter
} from "@/features/jobs/components/JobsManagementToolbar";
import { JobsManagementTable } from "@/features/jobs/components/JobsManagementTable";
import { useAsyncTask } from "@/lib/async/useAsyncTask";
import { useSessionStore } from "@/store/session.store";

const emptyFilters: JobsListFilters = {
  keyword: "",
  company: "",
  postcode: "",
  location: "",
  sourceName: "",
  categoryTag: "",
  visibility: "visible"
};

const defaultPageSize = 20;

export function JobsListView() {
  const navigate = useNavigate();
  const user = useSessionStore((state) => state.user);
  const isAdmin = user?.role === "admin";
  const [draftFilters, setDraftFilters] = useState<JobsListFilters>(emptyFilters);
  const [filters, setFilters] = useState<JobsListFilters>(emptyFilters);
  const [pageIndex, setPageIndex] = useState(0);
  const [pageSize, setPageSize] = useState(defaultPageSize);
  const [selectedIds, setSelectedIds] = useState<number[]>([]);
  const [actionMessage, setActionMessage] = useState<string | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);
  const [isProcessing, setIsProcessing] = useState(false);

  const loadJobs = useCallback(async (
    nextPageIndex: number,
    nextPageSize: number,
    nextFilters: JobsListFilters
  ) => {
    return getJobsPage({
      pageIndex: nextPageIndex,
      pageSize: nextPageSize,
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
    void execute(pageIndex, pageSize, filters);
  }, [execute, filters, pageIndex, pageSize]);

  useEffect(() => {
    const visibleIds = new Set((data?.items ?? []).map((job) => job.id));
    setSelectedIds((current) => current.filter((id) => visibleIds.has(id)));
  }, [data]);

  const jobs = data?.items ?? [];
  const viewStatus = status === "success" && jobs.length === 0 ? "empty" : status;
  const totalCount = data?.totalCount ?? 0;

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
      await execute(pageIndex, pageSize, filters);
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
      await execute(pageIndex, pageSize, filters);
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
        <JobsManagementHeader totalCount={totalCount} />

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
          <JobsManagementToolbar
            draftFilters={draftFilters}
            isAdmin={isAdmin}
            isProcessing={isProcessing}
            selectionSummary={selectionSummary}
            selectedCount={selectedIds.length}
            onApplyFilters={() => {
              setPageIndex(0);
              setFilters(draftFilters);
            }}
            onDeleteSelected={() => void handleDeleteSelected()}
            onFiltersChange={setDraftFilters}
            onHideSelected={() => void handleHideSelected()}
            onResetFilters={() => {
              setDraftFilters(emptyFilters);
              setPageIndex(0);
              setFilters(emptyFilters);
            }}
          />

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
            <JobsManagementTable
              jobs={jobs}
              pageIndex={pageIndex}
              pageSize={pageSize}
              selectedIds={selectedIds}
              totalCount={totalCount}
              onOpenJob={(jobId) => void navigate(`/admin/manage-jobs/${jobId}`)}
              onPageChange={setPageIndex}
              onPageSizeChange={(nextPageSize) => {
                setPageSize(nextPageSize);
                setPageIndex(0);
              }}
              onSelectAll={(checked) => {
                setSelectedIds(checked ? jobs.map((job) => job.id) : []);
              }}
              onSelectJob={(jobId, checked) => {
                setSelectedIds((current) =>
                  checked ? [...current, jobId] : current.filter((id) => id !== jobId)
                );
              }}
            />
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
