import ArrowBackRoundedIcon from "@mui/icons-material/ArrowBackRounded";
import CheckCircleRoundedIcon from "@mui/icons-material/CheckCircleRounded";
import ErrorOutlineRoundedIcon from "@mui/icons-material/ErrorOutlineRounded";
import LaunchRoundedIcon from "@mui/icons-material/LaunchRounded";
import { Button } from "@mui/material";
import type { ReactNode } from "react";
import { Link } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { getMockJobById } from "@/features/jobs/data/mockJobs";

type JobDetailExperienceProps = {
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

export function JobDetailExperience({ jobId }: JobDetailExperienceProps) {
  const job = jobId ? getMockJobById(jobId) : undefined;

  if (!job) {
    return (
      <div className="min-h-screen bg-background">
        <AppHeader />
        <div className="mx-auto max-w-7xl px-5 py-20 text-center sm:px-8">
          <h1 className="font-serif text-4xl font-semibold text-foreground">Job not found</h1>
          <p className="mt-4 text-foreground-secondary">
            This mock listing does not exist. Head back to the search results to continue browsing.
          </p>
          <Button component={Link} to="/search" sx={{ mt: 4 }}>
            Back to search
          </Button>
        </div>
      </div>
    );
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
          <section className="rounded-lg border border-border bg-background-elevated p-8">
            <div className="flex flex-wrap items-center gap-3 font-mono text-xs text-metadata">
              <span>{job.source}</span>
              <span className="text-divider">·</span>
              <span>Posted {job.postedDate}</span>
              {job.featured ? (
                <>
                  <span className="text-divider">·</span>
                  <span className="rounded bg-signal-featured-bg px-2 py-0.5 font-medium text-signal-featured">
                    Featured
                  </span>
                </>
              ) : null}
            </div>

            <h1 className="mt-4 font-serif text-4xl font-semibold text-foreground">{job.title}</h1>
            <p className="mt-3 text-lg text-foreground-secondary">
              {job.employer} · {job.location} · {job.postcode}
            </p>

            <div className="mt-5 flex flex-wrap gap-3 text-sm">
              {job.salary ? (
                <span className="rounded bg-muted px-3 py-1.5 font-mono text-foreground">{job.salary}</span>
              ) : null}
              {job.type ? (
                <span className="rounded bg-muted px-3 py-1.5 text-foreground">{job.type}</span>
              ) : null}
            </div>

            <Button
              href={job.url}
              target="_blank"
              rel="noreferrer"
              variant="contained"
              endIcon={<LaunchRoundedIcon />}
              sx={{
                mt: 4,
                bgcolor: "accent.main",
                "&:hover": { bgcolor: "accent.dark" }
              }}
            >
              Apply on {job.source}
            </Button>
          </section>

          <ContentPanel title="About the role">
            <p className="text-foreground-secondary">{job.summary}</p>
            <p className="mt-4 text-foreground-secondary">
              This mock page shows how Firefly Signal can present an individual opportunity with
              enough context to support quick judgment today and richer AI-assisted review later.
            </p>
          </ContentPanel>

          <ListPanel
            title="What you'll do"
            items={responsibilities}
            icon={<CheckCircleRoundedIcon className="text-accent-primary" />}
          />

          <ListPanel
            title="What we're looking for"
            items={requirements}
            icon={<ErrorOutlineRoundedIcon className="text-foreground-tertiary" />}
          />

          <ListPanel
            title="What we offer"
            items={benefits}
            icon={<CheckCircleRoundedIcon className="text-signal-fresh" />}
          />
        </main>

        <aside className="space-y-6">
          <section className="sticky top-24 rounded-lg border border-accent-secondary bg-accent-secondary p-6">
            <p className="font-mono text-xs tracking-[0.18em] text-accent-secondary-foreground/70">
              AI INSIGHT PREVIEW
            </p>
            <h2 className="mt-3 font-serif text-2xl font-semibold text-accent-secondary-foreground">
              Strong fit signal
            </h2>
            <p className="mt-3 text-sm leading-7 text-accent-secondary-foreground">
              The mock design reserves space for future guidance like fit notes, company signals,
              interview prep, and resume alignment without making the MVP dependent on AI.
            </p>
          </section>
        </aside>
      </div>
    </div>
  );
}

function ContentPanel({ title, children }: { title: string; children: ReactNode }) {
  return (
    <section className="rounded-lg border border-border bg-background-elevated p-8">
      <h2 className="font-serif text-2xl font-semibold text-foreground">{title}</h2>
      <div className="mt-4 text-sm leading-7">{children}</div>
    </section>
  );
}

function ListPanel({
  title,
  items,
  icon
}: {
  title: string;
  items: string[];
  icon: ReactNode;
}) {
  return (
    <section className="rounded-lg border border-border bg-background-elevated p-8">
      <h2 className="font-serif text-2xl font-semibold text-foreground">{title}</h2>
      <ul className="mt-5 space-y-3">
        {items.map((item) => (
          <li key={item} className="flex gap-3 text-sm leading-7 text-foreground-secondary">
            <span className="mt-0.5">{icon}</span>
            <span>{item}</span>
          </li>
        ))}
      </ul>
    </section>
  );
}
