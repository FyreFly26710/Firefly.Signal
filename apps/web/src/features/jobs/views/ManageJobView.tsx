import ArrowBackRoundedIcon from "@mui/icons-material/ArrowBackRounded";
import DeleteOutlineRoundedIcon from "@mui/icons-material/DeleteOutlineRounded";
import SaveRoundedIcon from "@mui/icons-material/SaveRounded";
import VisibilityOffRoundedIcon from "@mui/icons-material/VisibilityOffRounded";
import {
  Alert,
  Button,
  Checkbox,
  FormControlLabel,
  TextField
} from "@mui/material";
import { useEffect, useState } from "react";
import type { Dispatch, FormEvent, ReactNode, SetStateAction } from "react";
import { Link, useNavigate } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { SectionCard } from "@/components/SectionCard";
import {
  createJob,
  deleteJob,
  getJobById,
  hideJob,
  updateJob
} from "@/api/jobs/jobs.api";
import type {
  CreateJobRequestDto,
  JobDetailsResponseDto
} from "@/api/jobs/jobs.types";
import { createAsyncState, type AsyncState } from "@/lib/async/async-state";
import { useSessionStore } from "@/store/session.store";

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
      await hideJob(numericJobId);
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
        <Link
          to="/admin/manage-jobs"
          className="inline-flex items-center gap-2 text-sm text-foreground-secondary transition-colors hover:text-accent-primary"
        >
          <ArrowBackRoundedIcon sx={{ fontSize: 18 }} />
          Back to jobs
        </Link>

        <div className="mt-6 flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
          <div>
            <p className="font-mono text-xs tracking-[0.2em] text-foreground-tertiary">
              BACKEND-ALIGNED JOB CONTRACT
            </p>
            <h1 className="mt-2 font-serif text-4xl font-semibold text-foreground">{pageTitle}</h1>
            <p className="mt-3 max-w-3xl text-sm text-foreground-secondary">
              This editor mirrors the backend jobs contract directly, including hide and delete
              behavior enforced by the API.
            </p>
          </div>
          {!isCreateMode ? (
            <div className="rounded-lg border border-border bg-background-elevated px-4 py-3 text-sm text-foreground-secondary">
              Resource ID <span className="font-medium text-foreground">{numericJobId}</span>
            </div>
          ) : null}
        </div>

        {!isAdmin ? (
          <Alert severity="info" sx={{ mt: 4 }}>
            You can inspect the job resource, but save, hide, and delete actions require an admin
            role from the backend API.
          </Alert>
        ) : null}

        {saveMessage ? <Alert severity="success" sx={{ mt: 4 }}>{saveMessage}</Alert> : null}
        {errorMessage ? <Alert severity="error" sx={{ mt: 4 }}>{errorMessage}</Alert> : null}

        <form onSubmit={handleSubmit} className="mt-6 space-y-6">
          <JobFieldSection title="Core">
            <TextField
              label="Title"
              value={formValues.title}
              onChange={(event) => setStringValue(setFormValues, "title", event.target.value)}
              fullWidth
              required
            />
            <TextField
              label="Company"
              value={formValues.company}
              onChange={(event) => setStringValue(setFormValues, "company", event.target.value)}
              fullWidth
              required
            />
            <TextField
              label="Job URL"
              value={formValues.url}
              onChange={(event) => setStringValue(setFormValues, "url", event.target.value)}
              fullWidth
              required
            />
            <TextField
              label="Summary"
              value={formValues.summary}
              onChange={(event) => setStringValue(setFormValues, "summary", event.target.value)}
              fullWidth
              required
              multiline
              minRows={3}
            />
            <TextField
              label="Description"
              value={formValues.description}
              onChange={(event) => setStringValue(setFormValues, "description", event.target.value)}
              fullWidth
              required
              multiline
              minRows={5}
            />
          </JobFieldSection>

          <JobFieldSection title="Source">
            <TextField
              label="Source name"
              value={formValues.sourceName}
              onChange={(event) => setStringValue(setFormValues, "sourceName", event.target.value)}
              fullWidth
              required
            />
            <TextField
              label="Source job ID"
              value={formValues.sourceJobId}
              onChange={(event) => setStringValue(setFormValues, "sourceJobId", event.target.value)}
              fullWidth
              required
            />
            <TextField
              label="Source ad reference"
              value={formValues.sourceAdReference ?? ""}
              onChange={(event) =>
                setNullableStringValue(setFormValues, "sourceAdReference", event.target.value)
              }
              fullWidth
            />
            <TextField
              label="Job refresh run ID"
              value={formValues.jobRefreshRunId ?? ""}
              onChange={(event) =>
                setNullableNumberValue(setFormValues, "jobRefreshRunId", event.target.value)
              }
              fullWidth
            />
            <TextField
              label="Raw payload JSON"
              value={formValues.rawPayloadJson}
              onChange={(event) =>
                setStringValue(setFormValues, "rawPayloadJson", event.target.value)
              }
              fullWidth
              required
              multiline
              minRows={4}
            />
          </JobFieldSection>

          <JobFieldSection title="Location">
            <TextField
              label="Postcode"
              value={formValues.postcode}
              onChange={(event) => setStringValue(setFormValues, "postcode", event.target.value)}
              fullWidth
              required
            />
            <TextField
              label="Location name"
              value={formValues.locationName}
              onChange={(event) =>
                setStringValue(setFormValues, "locationName", event.target.value)
              }
              fullWidth
              required
            />
            <TextField
              label="Location display name"
              value={formValues.locationDisplayName ?? ""}
              onChange={(event) =>
                setNullableStringValue(setFormValues, "locationDisplayName", event.target.value)
              }
              fullWidth
            />
            <TextField
              label="Location area JSON"
              value={formValues.locationAreaJson ?? ""}
              onChange={(event) =>
                setNullableStringValue(setFormValues, "locationAreaJson", event.target.value)
              }
              fullWidth
            />
            <TextField
              label="Latitude"
              value={formValues.latitude ?? ""}
              onChange={(event) =>
                setNullableNumberValue(setFormValues, "latitude", event.target.value)
              }
              fullWidth
            />
            <TextField
              label="Longitude"
              value={formValues.longitude ?? ""}
              onChange={(event) =>
                setNullableNumberValue(setFormValues, "longitude", event.target.value)
              }
              fullWidth
            />
          </JobFieldSection>

          <JobFieldSection title="Classification and salary">
            <TextField
              label="Category tag"
              value={formValues.categoryTag ?? ""}
              onChange={(event) =>
                setNullableStringValue(setFormValues, "categoryTag", event.target.value)
              }
              fullWidth
            />
            <TextField
              label="Category label"
              value={formValues.categoryLabel ?? ""}
              onChange={(event) =>
                setNullableStringValue(setFormValues, "categoryLabel", event.target.value)
              }
              fullWidth
            />
            <TextField
              label="Salary min"
              value={formValues.salaryMin ?? ""}
              onChange={(event) =>
                setNullableNumberValue(setFormValues, "salaryMin", event.target.value)
              }
              fullWidth
            />
            <TextField
              label="Salary max"
              value={formValues.salaryMax ?? ""}
              onChange={(event) =>
                setNullableNumberValue(setFormValues, "salaryMax", event.target.value)
              }
              fullWidth
            />
            <TextField
              label="Salary currency"
              value={formValues.salaryCurrency ?? ""}
              onChange={(event) =>
                setNullableStringValue(setFormValues, "salaryCurrency", event.target.value)
              }
              fullWidth
            />
            <TextField
              label="Contract time"
              value={formValues.contractTime ?? ""}
              onChange={(event) =>
                setNullableStringValue(setFormValues, "contractTime", event.target.value)
              }
              fullWidth
            />
            <TextField
              label="Contract type"
              value={formValues.contractType ?? ""}
              onChange={(event) =>
                setNullableStringValue(setFormValues, "contractType", event.target.value)
              }
              fullWidth
            />
          </JobFieldSection>

          <JobFieldSection title="Flags and timestamps">
            <div className="grid gap-2 md:grid-cols-3">
              <FormControlLabel
                control={
                  <Checkbox
                    checked={formValues.isFullTime}
                    onChange={(event) =>
                      setBooleanValue(setFormValues, "isFullTime", event.target.checked)
                    }
                  />
                }
                label="Full time"
              />
              <FormControlLabel
                control={
                  <Checkbox
                    checked={formValues.isPartTime}
                    onChange={(event) =>
                      setBooleanValue(setFormValues, "isPartTime", event.target.checked)
                    }
                  />
                }
                label="Part time"
              />
              <FormControlLabel
                control={
                  <Checkbox
                    checked={formValues.isPermanent}
                    onChange={(event) =>
                      setBooleanValue(setFormValues, "isPermanent", event.target.checked)
                    }
                  />
                }
                label="Permanent"
              />
              <FormControlLabel
                control={
                  <Checkbox
                    checked={formValues.isContract}
                    onChange={(event) =>
                      setBooleanValue(setFormValues, "isContract", event.target.checked)
                    }
                  />
                }
                label="Contract"
              />
              <FormControlLabel
                control={
                  <Checkbox
                    checked={formValues.isRemote}
                    onChange={(event) =>
                      setBooleanValue(setFormValues, "isRemote", event.target.checked)
                    }
                  />
                }
                label="Remote"
              />
              <FormControlLabel
                control={
                  <Checkbox
                    checked={formValues.isHidden}
                    onChange={(event) =>
                      setBooleanValue(setFormValues, "isHidden", event.target.checked)
                    }
                  />
                }
                label="Hidden"
              />
              <FormControlLabel
                control={
                  <Checkbox
                    checked={formValues.salaryIsPredicted ?? false}
                    onChange={(event) =>
                      setNullableBooleanValue(
                        setFormValues,
                        "salaryIsPredicted",
                        event.target.checked
                      )
                    }
                  />
                }
                label="Salary predicted"
              />
            </div>
            <TextField
              label="Posted at UTC"
              value={formValues.postedAtUtc}
              onChange={(event) =>
                setStringValue(setFormValues, "postedAtUtc", event.target.value)
              }
              fullWidth
              required
            />
            <TextField
              label="Imported at UTC"
              value={formValues.importedAtUtc}
              onChange={(event) =>
                setStringValue(setFormValues, "importedAtUtc", event.target.value)
              }
              fullWidth
              required
            />
            <TextField
              label="Last seen at UTC"
              value={formValues.lastSeenAtUtc}
              onChange={(event) =>
                setStringValue(setFormValues, "lastSeenAtUtc", event.target.value)
              }
              fullWidth
              required
            />
          </JobFieldSection>

          <SectionCard className="flex flex-col gap-4 p-6 sm:flex-row sm:items-center sm:justify-between">
            <div className="text-sm text-foreground-secondary">
              {isCreateMode
                ? "Create a new job resource in the backend table."
                : "Save changes, hide the job, or delete it after it has been hidden."}
            </div>
            <div className="flex flex-wrap gap-3">
              {!isCreateMode ? (
                <Button
                  type="button"
                  variant="outlined"
                  color="inherit"
                  startIcon={<VisibilityOffRoundedIcon />}
                  disabled={!isAdmin || isHiding || isSaving || isDeleting}
                  onClick={() => void handleHide()}
                >
                  {isHiding ? "Hiding..." : "Hide job"}
                </Button>
              ) : null}
              {!isCreateMode ? (
                <Button
                  type="button"
                  color="error"
                  variant="outlined"
                  startIcon={<DeleteOutlineRoundedIcon />}
                  disabled={!isAdmin || isDeleting || isSaving || isHiding}
                  onClick={() => void handleDelete()}
                >
                  {isDeleting ? "Deleting..." : "Delete job"}
                </Button>
              ) : null}
              <Button
                type="submit"
                variant="contained"
                startIcon={<SaveRoundedIcon />}
                disabled={!isAdmin || isSaving || isHiding || isDeleting}
                sx={{
                  bgcolor: "accent.main",
                  "&:hover": { bgcolor: "accent.dark" }
                }}
              >
                {isSaving ? "Saving..." : isCreateMode ? "Create job" : "Save changes"}
              </Button>
            </div>
          </SectionCard>
        </form>
      </div>
    </div>
  );
}

function JobFieldSection({
  children,
  title
}: {
  children: ReactNode;
  title: string;
}) {
  return (
    <SectionCard className="p-6">
      <h2 className="font-serif text-2xl font-semibold text-foreground">{title}</h2>
      <div className="mt-5 grid gap-4 md:grid-cols-2">{children}</div>
    </SectionCard>
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

function setStringValue<TKey extends keyof CreateJobRequestDto>(
  setFormValues: Dispatch<SetStateAction<CreateJobRequestDto>>,
  key: TKey,
  value: CreateJobRequestDto[TKey]
) {
  setFormValues((current) => ({
    ...current,
    [key]: value
  }));
}

function setNullableStringValue<TKey extends keyof CreateJobRequestDto>(
  setFormValues: Dispatch<SetStateAction<CreateJobRequestDto>>,
  key: TKey,
  value: string
) {
  setFormValues((current) => ({
    ...current,
    [key]: value === "" ? null : value
  }));
}

function setNullableNumberValue<TKey extends keyof CreateJobRequestDto>(
  setFormValues: Dispatch<SetStateAction<CreateJobRequestDto>>,
  key: TKey,
  value: string
) {
  setFormValues((current) => ({
    ...current,
    [key]: value === "" ? null : Number(value)
  }));
}

function setBooleanValue<TKey extends keyof CreateJobRequestDto>(
  setFormValues: Dispatch<SetStateAction<CreateJobRequestDto>>,
  key: TKey,
  value: boolean
) {
  setFormValues((current) => ({
    ...current,
    [key]: value
  }));
}

function setNullableBooleanValue<TKey extends keyof CreateJobRequestDto>(
  setFormValues: Dispatch<SetStateAction<CreateJobRequestDto>>,
  key: TKey,
  value: boolean
) {
  setFormValues((current) => ({
    ...current,
    [key]: value
  }));
}
