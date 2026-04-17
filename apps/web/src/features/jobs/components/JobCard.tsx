import AccessTimeRoundedIcon from "@mui/icons-material/AccessTimeRounded";
import BookmarkBorderRoundedIcon from "@mui/icons-material/BookmarkBorderRounded";
import BookmarkRoundedIcon from "@mui/icons-material/BookmarkRounded";
import BusinessRoundedIcon from "@mui/icons-material/BusinessRounded";
import LaunchRoundedIcon from "@mui/icons-material/LaunchRounded";
import PlaceRoundedIcon from "@mui/icons-material/PlaceRounded";
import TaskAltRoundedIcon from "@mui/icons-material/TaskAltRounded";
import VisibilityOffRoundedIcon from "@mui/icons-material/VisibilityOffRounded";
import { IconButton, Tooltip } from "@mui/material";
import { Link } from "react-router-dom";
import type { JobCardModel } from "@/features/jobs/types/job.types";

type JobCardProps = {
  job: JobCardModel;
  isSaved?: boolean;
  isHidden?: boolean;
  isApplied?: boolean;
  onApply?: () => void;
  onToggleSave?: () => void;
  onToggleHide?: () => void;
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

export function JobCard({
  job,
  isSaved = false,
  isHidden = false,
  isApplied = false,
  onApply,
  onToggleSave,
  onToggleHide
}: JobCardProps) {
  if (isHidden) return null;

  const freshnessLabel = job.freshness ? freshnessLabels[job.freshness] : "";
  const freshnessClassName = job.freshness ? freshnessStyles[job.freshness] : "";

  return (
    <article className="group border-b border-divider bg-card transition-colors hover:bg-card-hover">
      <div className="px-5 py-6 sm:px-8">
        <JobCardHeader
          source={job.source}
          postedDate={job.postedDate}
          freshnessLabel={freshnessLabel}
          freshnessClassName={freshnessClassName}
          featured={Boolean(job.featured)}
          isApplied={isApplied}
          onApply={onApply}
          isSaved={isSaved}
          onToggleSave={onToggleSave}
          onToggleHide={onToggleHide}
        />
        <JobCardTitle jobId={job.id} title={job.title} />
        <JobCardMeta
          employer={job.employer}
          location={job.location}
          salary={job.salary}
          type={job.type}
        />
        <JobCardSummary summary={job.summary} />
        <JobCardActions jobId={job.id} source={job.source} url={job.url} />
      </div>
    </article>
  );
}

function JobCardHeader({
  source,
  postedDate,
  freshnessLabel,
  freshnessClassName,
  featured,
  isApplied,
  onApply,
  isSaved,
  onToggleSave,
  onToggleHide
}: {
  source: string;
  postedDate: string;
  freshnessLabel: string;
  freshnessClassName: string;
  featured: boolean;
  isApplied: boolean;
  onApply?: () => void;
  isSaved: boolean;
  onToggleSave?: () => void;
  onToggleHide?: () => void;
}) {
  return (
    <div className="mb-3 flex items-start justify-between gap-4">
      <div className="flex flex-wrap items-center gap-3 font-mono text-xs text-metadata">
        <span>{source}</span>
        <span className="text-divider">·</span>
        <span className="inline-flex items-center gap-1">
          <AccessTimeRoundedIcon sx={{ fontSize: 14 }} />
          {postedDate}
        </span>
        {freshnessLabel ? (
          <>
            <span className="text-divider">·</span>
            <span className={`rounded px-2 py-0.5 font-medium ${freshnessClassName}`}>
              {freshnessLabel}
            </span>
          </>
        ) : null}
        {featured ? (
          <>
            <span className="text-divider">·</span>
            <span className="rounded bg-signal-featured-bg px-2 py-0.5 font-medium text-signal-featured">
              Featured
            </span>
          </>
        ) : null}
      </div>

      <div className="flex items-center gap-1">
        {onApply && (
          <Tooltip title={isApplied ? "Applied" : "Mark as applied"}>
            <IconButton
              size="small"
              onClick={onApply}
              disabled={isApplied}
              aria-label={isApplied ? "Applied" : "Mark as applied"}
              sx={{
                color: isApplied ? "var(--color-signal-fresh)" : "var(--color-accent-primary)",
                bgcolor: isApplied ? "var(--color-signal-fresh-bg)" : "rgba(217,119,6,0.12)",
                "&:hover": {
                  bgcolor: isApplied ? "var(--color-signal-fresh-bg)" : "rgba(217,119,6,0.2)"
                }
              }}
            >
              <TaskAltRoundedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        )}
        {onToggleSave && (
          <Tooltip title={isSaved ? "Unsave job" : "Save job"}>
            <IconButton
              size="small"
              onClick={onToggleSave}
              aria-label={isSaved ? "Unsave job" : "Save job"}
              sx={{ color: isSaved ? "var(--color-accent-primary)" : "var(--color-foreground-tertiary)" }}
            >
              {isSaved ? (
                <BookmarkRoundedIcon fontSize="small" />
              ) : (
                <BookmarkBorderRoundedIcon fontSize="small" />
              )}
            </IconButton>
          </Tooltip>
        )}
        {onToggleHide && (
          <Tooltip title="Hide job">
            <IconButton
              size="small"
              onClick={onToggleHide}
              aria-label="Hide job"
              sx={{ color: "var(--color-foreground-tertiary)" }}
            >
              <VisibilityOffRoundedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        )}
      </div>
    </div>
  );
}

function JobCardTitle({ jobId, title }: { jobId: string; title: string }) {
  return (
    <Link to={`/jobs/${jobId}`}>
      <h2 className="font-serif text-2xl font-semibold text-foreground transition-colors group-hover:text-accent-primary">
        {title}
      </h2>
    </Link>
  );
}

function JobCardMeta({
  employer,
  location,
  salary,
  type
}: {
  employer: string;
  location: string;
  salary?: string;
  type?: string;
}) {
  return (
    <div className="mt-3 flex flex-wrap items-center gap-4 text-sm text-foreground-secondary">
      <span className="inline-flex items-center gap-1.5">
        <BusinessRoundedIcon sx={{ fontSize: 18 }} />
        {employer}
      </span>
      <span className="text-divider">·</span>
      <span className="inline-flex items-center gap-1.5">
        <PlaceRoundedIcon sx={{ fontSize: 18 }} />
        {location}
      </span>
      {salary ? (
        <>
          <span className="text-divider">·</span>
          <span className="font-mono">{salary}</span>
        </>
      ) : null}
      {type ? (
        <>
          <span className="text-divider">·</span>
          <span>{type}</span>
        </>
      ) : null}
    </div>
  );
}

function JobCardSummary({ summary }: { summary: string }) {
  return <p className="mt-4 max-w-3xl text-sm leading-7 text-foreground-secondary">{summary}</p>;
}

function JobCardActions({
  jobId,
  source,
  url
}: {
  jobId: string;
  source: string;
  url: string;
}) {
  return (
    <div className="mt-5 flex flex-wrap items-center gap-3 text-sm">
      <Link
        className="font-medium text-foreground-secondary transition-colors hover:text-accent-primary"
        to={`/jobs/${jobId}`}
      >
        View details
      </Link>
      <span className="text-divider">·</span>
      <a
        className="inline-flex items-center gap-1 font-medium text-foreground-secondary transition-colors hover:text-accent-primary"
        href={url}
        target="_blank"
        rel="noreferrer"
      >
        Apply on {source}
        <LaunchRoundedIcon sx={{ fontSize: 16 }} />
      </a>
    </div>
  );
}
