import ArrowBackRoundedIcon from "@mui/icons-material/ArrowBackRounded";
import { Alert } from "@mui/material";
import { Link } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { JobDetailContentPanel } from "@/features/jobs/components/JobDetailContentPanel";
import { JobDetailHeroCard } from "@/features/jobs/components/JobDetailHeroCard";
import { JobDetailNotFound } from "@/features/jobs/components/JobDetailNotFound";
import { JobInsightCard } from "@/features/jobs/components/JobInsightCard";
import { useJobDetail } from "@/features/jobs/hooks/useJobDetail";
import type { JobDetailModel } from "@/features/jobs/types/job.types";
import { ApiError } from "@/lib/http/api-error";

type JobDetailViewProps = {
  jobId: string | undefined;
};

export function JobDetailView({ jobId }: JobDetailViewProps) {
  const numericJobId = jobId && !Number.isNaN(Number(jobId)) ? Number(jobId) : null;
  const { data: job, isPending, isError, error } = useJobDetail(numericJobId);

  const isNotFound =
    numericJobId === null ||
    (isError && error instanceof ApiError && error.status === 404);
  if (isNotFound) {
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
          {isPending ? (
            <JobDetailContentPanel title="Loading job">
              <p className="text-foreground-secondary">Fetching the latest job details...</p>
            </JobDetailContentPanel>
          ) : null}

          {isError && !isNotFound ? (
            <Alert severity="error">
              {error instanceof Error ? error.message : "Unable to load this job."}
            </Alert>
          ) : null}

          {job ? (
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

        {job ? (
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
