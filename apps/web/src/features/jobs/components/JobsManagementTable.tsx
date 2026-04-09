import { Checkbox, Chip, Table, TableBody, TableCell, TableContainer, TableHead, TablePagination, TableRow } from "@mui/material";
import type { JobDetailsResponseDto } from "@/api/jobs/jobs.types";

type JobsManagementTableProps = {
  jobs: JobDetailsResponseDto[];
  pageIndex: number;
  pageSize: number;
  selectedIds: number[];
  totalCount: number;
  onOpenJob: (jobId: number) => void;
  onPageChange: (pageIndex: number) => void;
  onPageSizeChange: (pageSize: number) => void;
  onSelectAll: (checked: boolean) => void;
  onSelectJob: (jobId: number, checked: boolean) => void;
};

export function JobsManagementTable({
  jobs,
  pageIndex,
  pageSize,
  selectedIds,
  totalCount,
  onOpenJob,
  onPageChange,
  onPageSizeChange,
  onSelectAll,
  onSelectJob
}: JobsManagementTableProps) {
  const selectedVisibleCount = jobs.filter((job) => selectedIds.includes(job.id)).length;
  const allVisibleSelected = jobs.length > 0 && selectedVisibleCount === jobs.length;

  return (
    <>
      <TableContainer>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell padding="checkbox">
                <Checkbox
                  checked={allVisibleSelected}
                  indeterminate={selectedVisibleCount > 0 && !allVisibleSelected}
                  onChange={(event) => onSelectAll(event.target.checked)}
                  inputProps={{ "aria-label": "Select all jobs" }}
                />
              </TableCell>
              <TableCell>Title</TableCell>
              <TableCell>Company</TableCell>
              <TableCell>Location</TableCell>
              <TableCell>Source</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Posted</TableCell>
              <TableCell>Type</TableCell>
              <TableCell align="right">Salary</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {jobs.map((job) => {
              const isSelected = selectedIds.includes(job.id);

              return (
                <TableRow
                  key={job.id}
                  hover
                  selected={isSelected}
                  onClick={() => onOpenJob(job.id)}
                  sx={{ cursor: "pointer" }}
                >
                  <TableCell
                    padding="checkbox"
                    onClick={(event) => {
                      event.stopPropagation();
                    }}
                  >
                    <Checkbox
                      checked={isSelected}
                      onChange={(event) => onSelectJob(job.id, event.target.checked)}
                      inputProps={{ "aria-label": `Select job ${job.id}` }}
                    />
                  </TableCell>
                  <TableCell>
                    <div className="max-w-[280px]">
                      <div className="font-medium text-foreground">{job.title}</div>
                      <div className="mt-1 truncate text-xs text-foreground-tertiary">
                        #{job.id} · {job.sourceJobId}
                      </div>
                    </div>
                  </TableCell>
                  <TableCell>{job.companyDisplayName ?? job.company}</TableCell>
                  <TableCell>
                    <div className="max-w-[180px] truncate">
                      {job.locationDisplayName ?? job.locationName}
                    </div>
                  </TableCell>
                  <TableCell>{job.sourceName}</TableCell>
                  <TableCell>
                    <div className="flex flex-wrap gap-1">
                      {job.isHidden ? (
                        <Chip size="small" label="Hidden" color="warning" variant="outlined" />
                      ) : (
                        <Chip size="small" label="Visible" color="success" variant="outlined" />
                      )}
                      {job.isRemote ? <Chip size="small" label="Remote" variant="outlined" /> : null}
                    </div>
                  </TableCell>
                  <TableCell>{formatDate(job.postedAtUtc)}</TableCell>
                  <TableCell>{formatJobType(job)}</TableCell>
                  <TableCell align="right">{formatSalary(job)}</TableCell>
                </TableRow>
              );
            })}
          </TableBody>
        </Table>
      </TableContainer>

      <TablePagination
        component="div"
        count={totalCount}
        page={pageIndex}
        onPageChange={(_, nextPage) => onPageChange(nextPage)}
        rowsPerPage={pageSize}
        onRowsPerPageChange={(event) => onPageSizeChange(Number(event.target.value))}
        rowsPerPageOptions={[20, 50, 100]}
      />
    </>
  );
}

function formatDate(value: string): string {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    return "Unknown";
  }

  return new Intl.DateTimeFormat("en-GB", {
    day: "numeric",
    month: "short",
    year: "numeric"
  }).format(date);
}

function formatSalary(job: JobDetailsResponseDto): string {
  if (job.salaryMin === null && job.salaryMax === null) {
    return "N/A";
  }

  const formatter = new Intl.NumberFormat("en-GB", {
    style: "currency",
    currency: job.salaryCurrency ?? "GBP",
    maximumFractionDigits: 0
  });

  if (job.salaryMin !== null && job.salaryMax !== null) {
    return `${formatter.format(job.salaryMin)} - ${formatter.format(job.salaryMax)}`;
  }

  return formatter.format(job.salaryMin ?? job.salaryMax ?? 0);
}

function formatJobType(job: JobDetailsResponseDto): string {
  if (job.contractType) {
    return job.contractType.replaceAll("_", " ");
  }

  if (job.isPermanent) {
    return "permanent";
  }

  if (job.isContract) {
    return "contract";
  }

  if (job.isFullTime) {
    return "full time";
  }

  if (job.isPartTime) {
    return "part time";
  }

  return "unspecified";
}
