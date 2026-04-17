import {
  Alert,
  Button,
  Chip,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow
} from "@mui/material";
import type { JobImportRunResponseDto } from "@/api/jobs/jobs.types";

type JobImportRunsTableProps = {
  history: JobImportRunResponseDto[];
  historyError: string | null;
  historyHasNextPage: boolean;
  historyIsLoading: boolean;
  historyPageIndex: number;
  onHistoryPageChange: (pageIndex: number) => void;
};

const rowsPerPage = 4;

export function JobImportRunsTable({
  history,
  historyError,
  historyHasNextPage,
  historyIsLoading,
  historyPageIndex,
  onHistoryPageChange
}: JobImportRunsTableProps) {
  return (
    <div className="rounded-2xl border border-divider bg-background-subtle p-4">
      <div className="flex items-start justify-between gap-3">
        <div>
          <p className="font-mono text-[11px] tracking-[0.18em] text-foreground-tertiary">
            RECENT IMPORTS
          </p>
        </div>
        <Chip size="small" variant="outlined" label={`Page ${historyPageIndex + 1}`} />
      </div>

      <div className="mt-4">
        {historyIsLoading ? (
          <div className="rounded-xl border border-dashed border-divider px-4 py-5 text-sm text-foreground-secondary">
            Loading recent provider runs...
          </div>
        ) : null}

        {historyError ? <Alert severity="error">{historyError}</Alert> : null}

        {!historyIsLoading && !historyError && history.length === 0 ? (
          <div className="rounded-xl border border-dashed border-divider px-4 py-5 text-sm text-foreground-secondary">
            No recent provider runs yet.
          </div>
        ) : null}

        {!historyIsLoading && !historyError && history.length > 0 ? (
          <>
            <TableContainer className="rounded-xl border border-divider bg-background">
              <Table size="small">
                <TableHead>
                  <TableRow>
                    <TableCell>Provider</TableCell>
                    <TableCell>Status</TableCell>
                    <TableCell align="right">Inserted</TableCell>
                    <TableCell align="right">Received</TableCell>
                    <TableCell align="right">Failed</TableCell>
                    <TableCell>Started</TableCell>
                    <TableCell>Failure</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {history.map((run) => (
                    <TableRow key={run.id} hover>
                      <TableCell>
                        <div className="font-medium text-foreground">{run.providerName}</div>
                        <div className="mt-1 text-xs text-foreground-tertiary">#{run.id}</div>
                      </TableCell>
                      <TableCell>
                        <Chip
                          size="small"
                          label={formatStatus(run.status)}
                          color={mapStatusColor(run.status)}
                          variant="outlined"
                        />
                      </TableCell>
                      <TableCell align="right">{run.recordsInserted}</TableCell>
                      <TableCell align="right">{run.recordsReceived}</TableCell>
                      <TableCell align="right">{run.recordsFailed}</TableCell>
                      <TableCell>{formatDateTime(run.startedAtUtc)}</TableCell>
                      <TableCell>
                        <div className="max-w-[260px] text-xs text-foreground-secondary">
                          {run.failureSummary ?? "None"}
                        </div>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>

            <div className="flex items-center justify-between gap-3 border-t border-divider px-1 pt-3">
              <TablePagination
                component="div"
                count={historyHasNextPage ? -1 : historyPageIndex * rowsPerPage + history.length}
                page={historyPageIndex}
                onPageChange={(_, nextPage) => onHistoryPageChange(nextPage)}
                rowsPerPage={rowsPerPage}
                rowsPerPageOptions={[rowsPerPage]}
                labelDisplayedRows={({ from, to, count }) =>
                  count === -1 ? `${from}-${to} of more than ${to}` : `${from}-${to} of ${count}`
                }
                backIconButtonProps={{ disabled: historyPageIndex === 0 || historyIsLoading }}
                nextIconButtonProps={{ disabled: !historyHasNextPage || historyIsLoading }}
              />
              <div className="flex gap-2">
                <Button
                  variant="outlined"
                  color="inherit"
                  size="small"
                  disabled={historyPageIndex === 0 || historyIsLoading}
                  onClick={() => onHistoryPageChange(historyPageIndex - 1)}
                >
                  Previous
                </Button>
                <Button
                  variant="outlined"
                  color="inherit"
                  size="small"
                  disabled={!historyHasNextPage || historyIsLoading}
                  onClick={() => onHistoryPageChange(historyPageIndex + 1)}
                >
                  Next
                </Button>
              </div>
            </div>
          </>
        ) : null}
      </div>
    </div>
  );
}

function formatStatus(value: string) {
  return value.replace(/([a-z])([A-Z])/g, "$1 $2");
}

function mapStatusColor(value: string): "success" | "warning" | "error" | "default" {
  if (value === "Completed") {
    return "success";
  }

  if (value === "PartiallyCompleted" || value === "Running") {
    return "warning";
  }

  if (value === "Failed") {
    return "error";
  }

  return "default";
}

function formatDateTime(value: string) {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    return "Unknown";
  }

  return new Intl.DateTimeFormat("en-GB", {
    day: "numeric",
    month: "short",
    hour: "2-digit",
    minute: "2-digit"
  }).format(date);
}
