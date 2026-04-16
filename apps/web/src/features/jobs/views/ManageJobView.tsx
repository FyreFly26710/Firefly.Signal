import { Alert } from "@mui/material";
import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { SectionCard } from "@/components/SectionCard";
import {
  catalogHideJob,
  createJob,
  deleteJob,
  getJobById,
  updateJob
} from "@/api/jobs/jobs.api";
import type {
  CreateJobRequestDto,
  JobDetailsResponseDto
} from "@/api/jobs/jobs.types";
import { JobEditorAlerts } from "@/features/jobs/components/JobEditorAlerts";
import { JobEditorForm } from "@/features/jobs/components/JobEditorForm";
import { JobEditorHeader } from "@/features/jobs/components/JobEditorHeader";
import { createAsyncState, type AsyncState } from "@/lib/async/async-state";
import { useSessionStore } from "@/features/auth/store/session.store";

type ManageJobViewProps = {
  jobId?: string;
};

export function ManageJobView({ jobId }: ManageJobViewProps) {
  const navigate = useNavigate();
  const user = useSessionStore((state) => state.user);
  const isAdmin = user?.role === "admin";
  const numericJobId = jobId ? Number(jobId) : null;
  const isCreateMode = numericJobId === null;
  const [jobState, setJobState] = useState<AsyncState<JobDetailsResponseDto, "idle" | "loading" | "success" | "error">>(
    createAsyncState<JobDetailsResponseDto, "idle" | "loading" | "success" | "error">(
      isCreateMode ? "success" : "idle",
      null
    )
  );
  const [formValues, setFormValues] = useState<CreateJobRequestDto>(createDefaultJobFormValues());
  const [saveMessage, setSaveMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);
  const [isHiding, setIsHiding] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);

  useEffect(() => {
    if (isCreateMode) {
      return;
    }

    if (numericJobId === null || Number.isNaN(numericJobId)) {
      setJobState(
        createAsyncState<JobDetailsResponseDto, "idle" | "loading" | "success" | "error">(
          "error",
          null,
          "That job id is invalid."
        )
      );
      return;
    }

    let isMounted = true;
    const nextJobId = numericJobId;

    async function loadJob() {
      setJobState(
        createAsyncState<JobDetailsResponseDto, "idle" | "loading" | "success" | "error">(
          "loading",
          null
        )
      );

      try {
        const response = await getJobById(nextJobId);
        if (!isMounted) {
          return;
        }

        setJobState(
          createAsyncState<JobDetailsResponseDto, "idle" | "loading" | "success" | "error">(
            "success",
            response
          )
        );
        setFormValues(mapJobToFormValues(response));
      } catch (error) {
        if (!isMounted) {
          return;
        }

        setJobState(
          createAsyncState<JobDetailsResponseDto, "idle" | "loading" | "success" | "error">(
            "error",
            null,
            error instanceof Error ? error.message : "Unable to load this job."
          )
        );
      }
    }

    void loadJob();

    return () => {
      isMounted = false;
    };
  }, [isCreateMode, numericJobId]);

  const pageTitle = isCreateMode ? "Create job resource" : `Manage job #${numericJobId}`;

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    void submitForm();
  }

  async function submitForm() {
    if (!isAdmin) {
      setErrorMessage("Only admin sessions can save job resources.");
      return;
    }

    setIsSaving(true);
    setSaveMessage(null);
    setErrorMessage(null);

    try {
      const payload = normalizeFormValues(formValues);

      if (isCreateMode) {
        const created = await createJob(payload);
        setSaveMessage("Job created successfully.");
        void navigate(`/admin/manage-jobs/${created.id}`, { replace: true });
        return;
      }

      if (numericJobId === null) {
        throw new Error("This job id is missing.");
      }

      const updated = await updateJob(numericJobId, payload);
      setFormValues(mapJobToFormValues(updated));
      setJobState(
        createAsyncState<JobDetailsResponseDto, "idle" | "loading" | "success" | "error">(
          "success",
          updated
        )
      );
      setSaveMessage("Job updated successfully.");
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : "Unable to save this job.");
    } finally {
      setIsSaving(false);
    }
  }

  async function handleHide() {
    if (!isAdmin || numericJobId === null) {
      setErrorMessage("Only admin sessions can hide job resources.");
      return;
    }

    setIsHiding(true);
    setSaveMessage(null);
    setErrorMessage(null);

    try {
      await catalogHideJob(numericJobId);
      setFormValues((current) => ({ ...current, isHidden: true }));
      setSaveMessage("Job hidden successfully.");
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : "Unable to hide this job.");
    } finally {
      setIsHiding(false);
    }
  }

  async function handleDelete() {
    if (!isAdmin || numericJobId === null) {
      setErrorMessage("Only admin sessions can delete job resources.");
      return;
    }

    if (!window.confirm("Delete this job resource? The backend will reject this if the job is not hidden.")) {
      return;
    }

    setIsDeleting(true);
    setSaveMessage(null);
    setErrorMessage(null);

    try {
      await deleteJob(numericJobId);
      void navigate("/admin/manage-jobs", { replace: true });
    } catch (error) {
      setErrorMessage(
        error instanceof Error ? error.message : "Unable to delete this job."
      );
    } finally {
      setIsDeleting(false);
    }
  }

  if (jobState.status === "loading") {
    return (
      <div className="min-h-screen bg-background">
        <AppHeader variant="authenticated" />
        <div className="mx-auto max-w-5xl px-5 py-8 sm:px-8">
          <SectionCard className="p-6">
            <p className="text-sm text-foreground-secondary">Loading job resource...</p>
          </SectionCard>
        </div>
      </div>
    );
  }

  if (jobState.status === "error") {
    return (
      <div className="min-h-screen bg-background">
        <AppHeader variant="authenticated" />
        <div className="mx-auto max-w-5xl px-5 py-8 sm:px-8">
          <Alert severity="error">{jobState.errorMessage ?? "Unable to load this job."}</Alert>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-background">
      <AppHeader variant="authenticated" />

      <div className="mx-auto max-w-5xl px-5 py-8 sm:px-8">
        <JobEditorHeader
          isCreateMode={isCreateMode}
          jobId={numericJobId}
          pageTitle={pageTitle}
        />
        <JobEditorAlerts
          errorMessage={errorMessage}
          isAdmin={isAdmin}
          saveMessage={saveMessage}
        />
        <JobEditorForm
          formValues={formValues}
          isAdmin={isAdmin}
          isCreateMode={isCreateMode}
          isDeleting={isDeleting}
          isHiding={isHiding}
          isSaving={isSaving}
          onDelete={() => void handleDelete()}
          onHide={() => void handleHide()}
          onSubmit={handleSubmit}
          setFormValues={setFormValues}
        />
      </div>
    </div>
  );
}

function createDefaultJobFormValues(): CreateJobRequestDto {
  const now = new Date().toISOString();

  return {
    jobRefreshRunId: null,
    sourceName: "",
    sourceJobId: "",
    sourceAdReference: null,
    title: "",
    description: "",
    summary: "",
    url: "",
    company: "",
    companyDisplayName: null,
    companyCanonicalName: null,
    postcode: "",
    locationName: "",
    locationDisplayName: null,
    locationAreaJson: null,
    latitude: null,
    longitude: null,
    categoryTag: null,
    categoryLabel: null,
    salaryMin: null,
    salaryMax: null,
    salaryCurrency: "GBP",
    salaryIsPredicted: false,
    contractTime: null,
    contractType: null,
    isFullTime: false,
    isPartTime: false,
    isPermanent: false,
    isContract: false,
    isRemote: false,
    postedAtUtc: now,
    importedAtUtc: now,
    lastSeenAtUtc: now,
    isHidden: false,
    rawPayloadJson: "{}"
  };
}

function mapJobToFormValues(job: JobDetailsResponseDto): CreateJobRequestDto {
  return {
    jobRefreshRunId: job.jobRefreshRunId,
    sourceName: job.sourceName,
    sourceJobId: job.sourceJobId,
    sourceAdReference: job.sourceAdReference,
    title: job.title,
    description: job.description,
    summary: job.summary,
    url: job.url,
    company: job.company,
    companyDisplayName: job.companyDisplayName,
    companyCanonicalName: job.companyCanonicalName,
    postcode: job.postcode,
    locationName: job.locationName,
    locationDisplayName: job.locationDisplayName,
    locationAreaJson: job.locationAreaJson,
    latitude: job.latitude,
    longitude: job.longitude,
    categoryTag: job.categoryTag,
    categoryLabel: job.categoryLabel,
    salaryMin: job.salaryMin,
    salaryMax: job.salaryMax,
    salaryCurrency: job.salaryCurrency,
    salaryIsPredicted: job.salaryIsPredicted,
    contractTime: job.contractTime,
    contractType: job.contractType,
    isFullTime: job.isFullTime,
    isPartTime: job.isPartTime,
    isPermanent: job.isPermanent,
    isContract: job.isContract,
    isRemote: job.isRemote,
    postedAtUtc: job.postedAtUtc,
    importedAtUtc: job.importedAtUtc,
    lastSeenAtUtc: job.lastSeenAtUtc,
    isHidden: job.isHidden,
    rawPayloadJson: job.rawPayloadJson
  };
}

function normalizeFormValues(values: CreateJobRequestDto): CreateJobRequestDto {
  return {
    ...values,
    sourceName: values.sourceName.trim(),
    sourceJobId: values.sourceJobId.trim(),
    title: values.title.trim(),
    description: values.description.trim(),
    summary: values.summary.trim(),
    url: values.url.trim(),
    company: values.company.trim(),
    postcode: values.postcode.trim(),
    locationName: values.locationName.trim(),
    rawPayloadJson: values.rawPayloadJson.trim() || "{}",
    sourceAdReference: normalizeNullableString(values.sourceAdReference),
    companyDisplayName: normalizeNullableString(values.companyDisplayName),
    companyCanonicalName: normalizeNullableString(values.companyCanonicalName),
    locationDisplayName: normalizeNullableString(values.locationDisplayName),
    locationAreaJson: normalizeNullableString(values.locationAreaJson),
    categoryTag: normalizeNullableString(values.categoryTag),
    categoryLabel: normalizeNullableString(values.categoryLabel),
    salaryCurrency: normalizeNullableString(values.salaryCurrency),
    contractTime: normalizeNullableString(values.contractTime),
    contractType: normalizeNullableString(values.contractType)
  };
}

function normalizeNullableString(value: string | null): string | null {
  if (value === null) {
    return null;
  }

  const trimmed = value.trim();
  return trimmed ? trimmed : null;
}
