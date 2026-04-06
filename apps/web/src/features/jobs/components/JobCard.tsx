import AccessTimeRoundedIcon from "@mui/icons-material/AccessTimeRounded";
import BookmarkBorderRoundedIcon from "@mui/icons-material/BookmarkBorderRounded";
import BusinessRoundedIcon from "@mui/icons-material/BusinessRounded";
import LaunchRoundedIcon from "@mui/icons-material/LaunchRounded";
import PlaceRoundedIcon from "@mui/icons-material/PlaceRounded";
import { IconButton } from "@mui/material";
import { Link } from "react-router-dom";
import type { JobCardModel } from "@/features/jobs/types/job.types";

type JobCardProps = {
  job: JobCardModel;
};

const freshnessStyles = {
  fresh: "bg-signal-fresh-bg text-signal-fresh",
  recent: "bg-accent-secondary text-accent-secondary-foreground",
  older: ""
} as const;

const freshnessLabels = {
  fresh: "New",
  recent: "Recent",
  older: ""
} as const;

export function JobCard({ job }: JobCardProps) {
  const freshnessLabel = job.freshness ? freshnessLabels[job.freshness] : "";
  const freshnessClassName = job.freshness ? freshnessStyles[job.freshness] : "";

  return (
    <article className="group border-b border-divider bg-card transition-colors hover:bg-card-hover">
      <div className="px-5 py-6 sm:px-8">
        <div className="mb-3 flex items-start justify-between gap-4">
          <div className="flex flex-wrap items-center gap-3 font-mono text-xs text-metadata">
            <span>{job.source}</span>
            <span className="text-divider">·</span>
            <span className="inline-flex items-center gap-1">
              <AccessTimeRoundedIcon sx={{ fontSize: 14 }} />
              {job.postedDate}
            </span>
            {freshnessLabel ? (
              <>
                <span className="text-divider">·</span>
                <span className={`rounded px-2 py-0.5 font-medium ${freshnessClassName}`}>
                  {freshnessLabel}
                </span>
              </>
            ) : null}
            {job.featured ? (
              <>
                <span className="text-divider">·</span>
                <span className="rounded bg-signal-featured-bg px-2 py-0.5 font-medium text-signal-featured">
                  Featured
                </span>
              </>
            ) : null}
          </div>

          <IconButton size="small" sx={{ color: "var(--color-foreground-tertiary)" }}>
            <BookmarkBorderRoundedIcon fontSize="small" />
          </IconButton>
        </div>

        <Link to={`/jobs/${job.id}`}>
          <h2 className="font-serif text-2xl font-semibold text-foreground transition-colors group-hover:text-accent-primary">
            {job.title}
          </h2>
        </Link>

        <div className="mt-3 flex flex-wrap items-center gap-4 text-sm text-foreground-secondary">
          <span className="inline-flex items-center gap-1.5">
            <BusinessRoundedIcon sx={{ fontSize: 18 }} />
            {job.employer}
          </span>
          <span className="text-divider">·</span>
          <span className="inline-flex items-center gap-1.5">
            <PlaceRoundedIcon sx={{ fontSize: 18 }} />
            {job.location}
          </span>
          {job.salary ? (
            <>
              <span className="text-divider">·</span>
              <span className="font-mono">{job.salary}</span>
            </>
          ) : null}
          {job.type ? (
            <>
              <span className="text-divider">·</span>
              <span>{job.type}</span>
            </>
          ) : null}
        </div>

        <p className="mt-4 max-w-3xl text-sm leading-7 text-foreground-secondary">{job.summary}</p>

        <div className="mt-5 flex flex-wrap items-center gap-3 text-sm">
          <Link className="font-medium text-foreground-secondary transition-colors hover:text-accent-primary" to={`/jobs/${job.id}`}>
            View details
          </Link>
          <span className="text-divider">·</span>
          <a
            className="inline-flex items-center gap-1 font-medium text-foreground-secondary transition-colors hover:text-accent-primary"
            href={job.url}
            target="_blank"
            rel="noreferrer"
          >
            Apply on {job.source}
            <LaunchRoundedIcon sx={{ fontSize: 16 }} />
          </a>
        </div>
      </div>
    </article>
  );
}
