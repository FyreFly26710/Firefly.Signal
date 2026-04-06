import LaunchRoundedIcon from "@mui/icons-material/LaunchRounded";
import { Button } from "@mui/material";
import { SectionCard } from "@/components/SectionCard";
import type { MockJob } from "@/features/jobs/types/job.types";

type JobDetailHeroCardProps = {
  job: MockJob;
};

export function JobDetailHeroCard({ job }: JobDetailHeroCardProps) {
  return (
    <SectionCard className="p-8">
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
        {job.type ? <span className="rounded bg-muted px-3 py-1.5 text-foreground">{job.type}</span> : null}
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
    </SectionCard>
  );
}
