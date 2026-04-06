import ArrowBackRoundedIcon from "@mui/icons-material/ArrowBackRounded";
import CheckCircleRoundedIcon from "@mui/icons-material/CheckCircleRounded";
import ErrorOutlineRoundedIcon from "@mui/icons-material/ErrorOutlineRounded";
import { Link } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { JobDetailContentPanel } from "@/features/jobs/components/JobDetailContentPanel";
import { JobDetailHeroCard } from "@/features/jobs/components/JobDetailHeroCard";
import { JobDetailListPanel } from "@/features/jobs/components/JobDetailListPanel";
import { JobDetailNotFound } from "@/features/jobs/components/JobDetailNotFound";
import { JobInsightCard } from "@/features/jobs/components/JobInsightCard";
import { getMockJobById } from "@/features/jobs/data/mockJobs";

type JobDetailViewProps = {
  jobId: string | undefined;
};

const responsibilities = [
  "Lead design for new features from concept to launch.",
  "Conduct user research and usability testing.",
  "Collaborate with product managers and engineers.",
  "Contribute to and maintain our design system.",
  "Mentor junior teammates and raise quality across the team."
];

const requirements = [
  "5+ years of relevant experience in the discipline.",
  "Strong portfolio or track record showing thoughtful execution.",
  "Comfortable working inside a modern product team.",
  "Able to communicate clearly with technical and non-technical partners.",
  "Experience with iterative delivery and feedback loops."
];

const benefits = [
  "Competitive salary and equity package.",
  "Flexible hybrid working arrangements.",
  "Learning and development budget.",
  "Private health cover.",
  "Generous annual leave."
];

export function JobDetailView({ jobId }: JobDetailViewProps) {
  const job = jobId ? getMockJobById(jobId) : undefined;

  if (!job) {
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
          <JobDetailHeroCard job={job} />
          <JobDetailContentPanel title="About the role">
            <p className="text-foreground-secondary">{job.summary}</p>
            <p className="mt-4 text-foreground-secondary">
              This mock page shows how Firefly Signal can present an individual opportunity with
              enough context to support quick judgment today and richer AI-assisted review later.
            </p>
          </JobDetailContentPanel>
          <JobDetailListPanel
            title="What you'll do"
            items={responsibilities}
            icon={<CheckCircleRoundedIcon className="text-accent-primary" />}
          />
          <JobDetailListPanel
            title="What we're looking for"
            items={requirements}
            icon={<ErrorOutlineRoundedIcon className="text-foreground-tertiary" />}
          />
          <JobDetailListPanel
            title="What we offer"
            items={benefits}
            icon={<CheckCircleRoundedIcon className="text-signal-fresh" />}
          />
        </main>

        <aside className="space-y-6">
          <JobInsightCard />
        </aside>
      </div>
    </div>
  );
}
