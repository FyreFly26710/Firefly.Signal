import WorkOutlineRoundedIcon from "@mui/icons-material/WorkOutlineRounded";
import { Alert } from "@mui/material";
import { useQueryClient } from "@tanstack/react-query";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { SectionCard } from "@/components/SectionCard";
import {
  catalogHideJobs,
  deleteJobs,
  exportJobs,
  getJobsPage,
  importJobsFromJson,
  importJobsFromProvider
} from "@/api/jobs/jobs.api";
import type {
  DeleteJobsResponseDto,
  JobImportRunResponseDto,
  ImportJobsFromProviderRequestDto
} from "@/api/jobs/jobs.types";
import { JobsManagementHeader } from "@/features/jobs/components/JobsManagementHeader";
import { JobsImportPanel } from "@/features/jobs/components/JobsImportPanel";
import {
  JobsImportProviderDialog,
  type JobsImportProviderFormValues
} from "@/features/jobs/components/JobsImportProviderDialog";
import { useJobImportRuns } from "@/features/jobs/hooks/useJobImportRuns";
import {
  JobsManagementToolbar,
  type JobsListFilters,
  type VisibilityFilter
} from "@/features/jobs/components/JobsManagementToolbar";
import { JobsManagementTable } from "@/features/jobs/components/JobsManagementTable";
import { useAsyncTask } from "@/lib/async/useAsyncTask";
import { useSessionStore } from "@/features/auth/store/session.store";

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
const defaultImportForm: JobsImportProviderFormValues = {
  where: "london",
  keyword: "",
  pageIndex: "1",
  pageSize: "50",
  provider: "Adzuna",
  excludedKeyword: "",
  distanceKilometers: "5",
  maxDaysOld: "30",
  category: "it-jobs",
  salaryMin: "",
  salaryMax: ""
};

export function JobsListView() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
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
  const [isImportDialogOpen, setIsImportDialogOpen] = useState(false);
  const [historyPageIndex, setHistoryPageIndex] = useState(0);
  const [importForm, setImportForm] = useState<JobsImportProviderFormValues>(defaultImportForm);
  const fileInputRef = useRef<HTMLInputElement | null>(null);
  const historyPageSize = 4;
  const { data: importRunHistory = [], isPending: isHistoryLoading, error: historyError } = useJobImportRuns(
    historyPageIndex,
    isImportDialogOpen && isAdmin,
    historyPageSize
  );

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
      const result = await catalogHideJobs(selectedIds);
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

  async function handleProviderImport() {
    if (!isAdmin) {
      return;
    }

    const where = importForm.where.trim();
    if (!where) {
      setActionMessage(null);
      setActionError("Enter a location or postcode before importing from the provider.");
      return;
    }

    setIsProcessing(true);
    setActionMessage(null);
    setActionError(null);

    try {
      const result = await importJobsFromProvider(buildImportRequest(importForm, where));
      await queryClient.invalidateQueries({ queryKey: ["job-import-runs"] });
      setActionMessage(`Imported ${result.importedCount} jobs from ${result.source}.`);
      setIsImportDialogOpen(false);
      await execute(pageIndex, pageSize, filters);
    } catch (error) {
      setActionError(error instanceof Error ? error.message : "Unable to import jobs from the provider.");
    } finally {
      setIsProcessing(false);
    }
  }

  async function handleJsonImport(file: File) {
    if (!isAdmin) {
      return;
    }

    setIsProcessing(true);
    setActionMessage(null);
    setActionError(null);

    try {
      const result = await importJobsFromJson(file);
      setActionMessage(`Imported ${result.importedCount} jobs from ${file.name}.`);
      await execute(pageIndex, pageSize, filters);
    } catch (error) {
      setActionError(error instanceof Error ? error.message : "Unable to import jobs from JSON.");
    } finally {
      setIsProcessing(false);
      if (fileInputRef.current) {
        fileInputRef.current.value = "";
      }
    }
  }

  async function handleExport() {
    if (!isAdmin || selectedIds.length === 0) {
      return;
    }

    setIsProcessing(true);
    setActionMessage(null);
    setActionError(null);

    try {
      const exportResult = await exportJobs({ jobIds: selectedIds });

      const blob = new Blob([JSON.stringify(exportResult, null, 2)], {
        type: "application/json"
      });
      const url = URL.createObjectURL(blob);
      const anchor = document.createElement("a");
      anchor.href = url;
      anchor.download = `jobs-export-${new Date().toISOString().replaceAll(":", "-")}.json`;
      anchor.click();
      URL.revokeObjectURL(url);

      setActionMessage(`Exported ${exportResult.count} jobs to JSON.`);
    } catch (error) {
      setActionError(error instanceof Error ? error.message : "Unable to export jobs to JSON.");
    } finally {
      setIsProcessing(false);
    }
  }

  return (
    <div className="min-h-screen bg-background">
      <AppHeader variant="authenticated" />

      <div className="mx-auto max-w-[1400px] px-5 py-8 sm:px-8">
        <JobsManagementHeader totalCount={totalCount} />

        {isAdmin ? (
          <>
            <JobsImportPanel
              isProcessing={isProcessing}
              canExport={selectedIds.length > 0}
              onCreateJob={() => void navigate("/admin/manage-jobs/new")}
              onExportJson={() => void handleExport()}
              onImportJson={() => fileInputRef.current?.click()}
              onImportProvider={() => {
                setHistoryPageIndex(0);
                setIsImportDialogOpen(true);
              }}
            />
            <JobsImportProviderDialog
              isOpen={isImportDialogOpen}
              isSubmitting={isProcessing}
              history={sliceHistoryPage(importRunHistory, historyPageIndex, historyPageSize)}
              historyError={historyError instanceof Error ? historyError.message : null}
              historyHasNextPage={importRunHistory.length === (historyPageIndex + 1) * historyPageSize}
              historyIsLoading={isHistoryLoading}
              historyPageIndex={historyPageIndex}
              historyRowsPerPage={historyPageSize}
              values={importForm}
              onChange={setImportForm}
              onClose={() => {
                if (!isProcessing) {
                  setIsImportDialogOpen(false);
                }
              }}
              onHistoryPageChange={setHistoryPageIndex}
              onSubmit={() => void handleProviderImport()}
            />
            <input
              ref={fileInputRef}
              type="file"
              accept=".json,application/json"
              className="hidden"
              onChange={(event) => {
                const file = event.target.files?.[0];
                if (file) {
                  void handleJsonImport(file);
                }
              }}
            />
          </>
        ) : null}

        {!isAdmin ? (
          <Alert severity="warning" sx={{ mb: 4 }}>
            This route is intended for admin job management. Your session can read jobs, but batch
            hide, batch delete, edit, import, and export actions require the admin role from the
            backend API.
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

function sliceHistoryPage(
  history: readonly JobImportRunResponseDto[],
  pageIndex: number,
  pageSize: number
) {
  const start = pageIndex * pageSize;
  return history.slice(start, start + pageSize);
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

function parseOptionalNumber(value: string): number | undefined {
  const trimmed = value.trim();
  if (!trimmed) {
    return undefined;
  }

  const parsed = Number(trimmed);
  return Number.isFinite(parsed) ? parsed : undefined;
}

function buildImportRequest(
  values: JobsImportProviderFormValues,
  where: string
): ImportJobsFromProviderRequestDto {
  return {
    pageIndex: parseOptionalNumber(values.pageIndex) ?? 1,
    pageSize: parseOptionalNumber(values.pageSize) ?? 50,
    where,
    keyword: normalizeText(values.keyword),
    distanceKilometers: parseOptionalNumber(values.distanceKilometers) ?? 5,
    maxDaysOld: parseOptionalNumber(values.maxDaysOld) ?? 30,
    category: normalizeText(values.category) ?? "it-jobs",
    provider: values.provider,
    excludedKeyword: normalizeText(values.excludedKeyword),
    salaryMin: parseOptionalNumber(values.salaryMin),
    salaryMax: parseOptionalNumber(values.salaryMax)
  };
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
