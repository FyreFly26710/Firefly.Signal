import BookmarkBorderRoundedIcon from "@mui/icons-material/BookmarkBorderRounded";
import BookmarkRoundedIcon from "@mui/icons-material/BookmarkRounded";
import LaunchRoundedIcon from "@mui/icons-material/LaunchRounded";
import TaskAltRoundedIcon from "@mui/icons-material/TaskAltRounded";
import VisibilityOffRoundedIcon from "@mui/icons-material/VisibilityOffRounded";
import { IconButton, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Tooltip } from "@mui/material";
import { Link } from "react-router-dom";
import type { JobCardModel } from "@/features/jobs/types/job.types";

type JobSearchCompactTableProps = {
  jobs: JobCardModel[];
  savedIds: Set<string>;
  hiddenIds: Set<string>;
  appliedIds: Set<string>;
  onApply: (jobId: string) => void;
  onToggleSave: (jobId: string) => void;
  onToggleHide: (jobId: string) => void;
};

export function JobSearchCompactTable({
  jobs,
  savedIds,
  hiddenIds,
  appliedIds,
  onApply,
  onToggleSave,
  onToggleHide
}: JobSearchCompactTableProps) {
  const visible = jobs.filter((j) => !hiddenIds.has(j.id));

  return (
    <TableContainer className="rounded-lg border border-border bg-background-elevated">
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>Title</TableCell>
            <TableCell>Company</TableCell>
            <TableCell>Location</TableCell>
            <TableCell>Salary</TableCell>
            <TableCell>Posted</TableCell>
            <TableCell align="right" padding="checkbox" />
          </TableRow>
        </TableHead>
        <TableBody>
          {visible.map((job) => (
            <CompactRow
              key={job.id}
              job={job}
              isApplied={appliedIds.has(job.id)}
              onApply={() => onApply(job.id)}
              isSaved={savedIds.has(job.id)}
              onToggleSave={() => onToggleSave(job.id)}
              onToggleHide={() => onToggleHide(job.id)}
            />
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}

function CompactRow({
  job,
  isApplied,
  onApply,
  isSaved,
  onToggleSave,
  onToggleHide
}: {
  job: JobCardModel;
  isApplied: boolean;
  onApply: () => void;
  isSaved: boolean;
  onToggleSave: () => void;
  onToggleHide: () => void;
}) {
  return (
    <TableRow hover>
      <TableCell>
        <Link
          to={`/jobs/${job.id}`}
          className="font-medium text-foreground transition-colors hover:text-accent-primary"
        >
          <span className="line-clamp-1 max-w-[260px] block">{job.title}</span>
        </Link>
      </TableCell>
      <TableCell>
        <span className="line-clamp-1 max-w-[160px] block text-foreground-secondary">{job.employer}</span>
      </TableCell>
      <TableCell>
        <span className="line-clamp-1 max-w-[140px] block text-foreground-secondary">{job.location}</span>
      </TableCell>
      <TableCell>
        <span className="font-mono text-foreground-secondary">{job.salary ?? "—"}</span>
      </TableCell>
      <TableCell>
        <span className="text-foreground-secondary">{formatShortDate(job.postedDate)}</span>
      </TableCell>
      <TableCell align="right" padding="none">
        <div className="flex items-center justify-end gap-0.5 pr-1">
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
              <TaskAltRoundedIcon sx={{ fontSize: 16 }} />
            </IconButton>
          </Tooltip>
          <Tooltip title={isSaved ? "Unsave job" : "Save job"}>
            <IconButton
              size="small"
              onClick={onToggleSave}
              aria-label={isSaved ? "Unsave job" : "Save job"}
              sx={{ color: isSaved ? "var(--color-accent-primary)" : "var(--color-foreground-tertiary)" }}
            >
              {isSaved ? (
                <BookmarkRoundedIcon sx={{ fontSize: 16 }} />
              ) : (
                <BookmarkBorderRoundedIcon sx={{ fontSize: 16 }} />
              )}
            </IconButton>
          </Tooltip>
          <Tooltip title="Hide job">
            <IconButton
              size="small"
              onClick={onToggleHide}
              aria-label="Hide job"
              sx={{ color: "var(--color-foreground-tertiary)" }}
            >
              <VisibilityOffRoundedIcon sx={{ fontSize: 16 }} />
            </IconButton>
          </Tooltip>
          <Tooltip title={`Apply on ${job.source}`}>
            <IconButton
              size="small"
              component="a"
              href={job.url}
              target="_blank"
              rel="noreferrer"
              aria-label={`Apply on ${job.source}`}
              sx={{ color: "var(--color-foreground-tertiary)" }}
            >
              <LaunchRoundedIcon sx={{ fontSize: 16 }} />
            </IconButton>
          </Tooltip>
        </div>
      </TableCell>
    </TableRow>
  );
}

function formatShortDate(value: string): string {
  // value is already formatted by the mapper, e.g. "2 Jan 2025"
  // Strip the year part: "2 Jan 2025" → "2 Jan"
  return value.replace(/\s+\d{4}$/, "");
}
