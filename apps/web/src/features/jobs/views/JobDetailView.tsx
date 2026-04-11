import ArrowBackRoundedIcon from "@mui/icons-material/ArrowBackRounded";
import { Alert } from "@mui/material";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { getJobById } from "@/api/jobs/jobs.api";
import { JobDetailContentPanel } from "@/features/jobs/components/JobDetailContentPanel";
import { JobDetailHeroCard } from "@/features/jobs/components/JobDetailHeroCard";
import { JobDetailNotFound } from "@/features/jobs/components/JobDetailNotFound";
import { JobInsightCard } from "@/features/jobs/components/JobInsightCard";
import { mapJobDetail } from "@/features/jobs/mappers/job-detail.mappers";
import type { JobDetailModel } from "@/features/jobs/types/job.types";
import { ApiError } from "@/lib/http/api-error";

type JobDetailViewProps = {
  jobId: string | undefined;
};

export function JobDetailView({ jobId }: JobDetailViewProps) {
  const numericJobId = Number(jobId);
  const hasValidJobId = Boolean(jobId) && !Number.isNaN(numericJobId);
  const [job, setJob] = useState<JobDetailModel | null>(null);
  const [status, setStatus] = useState<"idle" | "loading" | "success" | "not-found" | "error">(
    "idle"
  );
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  useEffect(() => {
    if (!hasValidJobId) {
      return;
    }

    let cancelled = false;

    async function loadJob() {
      setStatus("loading");
      setErrorMessage(null);

      try {
        const response = await getJobById(numericJobId);

        if (cancelled) {
          return;
        }

        setJob(mapJobDetail(response));
        setStatus("success");
      } catch (error) {
        if (cancelled) {
          return;
        }

        setJob(null);

        if (error instanceof ApiError && error.status === 404) {
          setStatus("not-found");
          return;
        }

        setStatus("error");
        setErrorMessage(error instanceof Error ? error.message : "Unable to load this job.");
      }
    }

    void loadJob();

    return () => {
      cancelled = true;
    };
  }, [hasValidJobId, numericJobId]);

  if (!hasValidJobId || status === "not-found") {
    return <JobDetailNotFound />;
  }

  return (
    <div className="min-h-screen bg-background">
      <AppHeader />

      <div className="border-b border-divider bg-background-elevated">
        <div className="mx-auto max-w-7xl px-5 py-4 sm:px-8">
          <Link
            to="/search"
            className="inline-flex items-center gap-2 text-sm text-foreground-secondary transition-colors hover:text-accent-primary"
          >
            <ArrowBackRoundedIcon sx={{ fontSize: 18 }} />
            Back to search results
          </Link>
        </div>
      </div>

      <div className="mx-auto grid max-w-7xl gap-8 px-5 py-8 sm:px-8 lg:grid-cols-[minmax(0,1fr)_320px]">
        <main className="space-y-8">
          {status === "loading" ? (
            <JobDetailContentPanel title="Loading job">
              <p className="text-foreground-secondary">Fetching the latest job details...</p>
            </JobDetailContentPanel>
          ) : null}

          {status === "error" ? (
            <Alert severity="error">{errorMessage ?? "Unable to load this job."}</Alert>
          ) : null}

          {status === "success" && job ? (
            <>
              <JobDetailHeroCard job={job} />
              <JobDetailContentPanel title="About the role">
                {getDescriptionParagraphs(job).map((paragraph) => (
                  <p key={paragraph} className="text-foreground-secondary">
                    {paragraph}
                  </p>
                ))}
              </JobDetailContentPanel>
            </>
          ) : null}
        </main>

        {status === "success" && job ? (
          <aside className="space-y-6">
            <JobInsightCard />
          </aside>
        ) : null}
      </div>
    </div>
  );
}

function getDescriptionParagraphs(job: JobDetailModel): string[] {
  const paragraphs = job.description
    .split(/\n\s*\n/g)
    .map((paragraph) => paragraph.trim())
    .filter(Boolean);

  if (paragraphs.length > 0) {
    return paragraphs;
  }

  return [job.summary];
}
