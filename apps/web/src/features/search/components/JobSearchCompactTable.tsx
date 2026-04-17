import BookmarkBorderRoundedIcon from "@mui/icons-material/BookmarkBorderRounded";
import BookmarkRoundedIcon from "@mui/icons-material/BookmarkRounded";
import LaunchRoundedIcon from "@mui/icons-material/LaunchRounded";
import VisibilityOffRoundedIcon from "@mui/icons-material/VisibilityOffRounded";
import { IconButton, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Tooltip } from "@mui/material";
import { Link } from "react-router-dom";
import type { JobCardModel } from "@/features/jobs/types/job.types";

type JobSearchCompactTableProps = {
  jobs: JobCardModel[];
  savedIds: Set<string>;
  hiddenIds: Set<string>;
  onToggleSave: (jobId: string) => void;
  onToggleHide: (jobId: string) => void;
};

export function JobSearchCompactTable({
  jobs,
  savedIds,
  hiddenIds,
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
            <TableCell>Type</TableCell>
            <TableCell>Salary</TableCell>
            <TableCell>Posted</TableCell>
            <TableCell>Source</TableCell>
            <TableCell align="right" padding="checkbox" />
          </TableRow>
        </TableHead>
        <TableBody>
          {visible.map((job) => (
            <CompactRow
              key={job.id}
              job={job}
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
  isSaved,
  onToggleSave,
  onToggleHide
}: {
  job: JobCardModel;
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
        <span className="text-foreground-secondary">{job.type ?? "—"}</span>
      </TableCell>
      <TableCell>
        <span className="font-mono text-foreground-secondary">{job.salary ?? "—"}</span>
      </TableCell>
      <TableCell>
        <span className="text-foreground-secondary">{job.postedDate}</span>
      </TableCell>
      <TableCell>
        <span className="text-foreground-secondary">{job.source}</span>
      </TableCell>
      <TableCell align="right" padding="none">
        <div className="flex items-center justify-end gap-0.5 pr-1">
          <Tooltip title={isSaved ? "Unsave job" : "Save job"}>
            <IconButton
              size="small"
              onClick={onToggleSave}
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
