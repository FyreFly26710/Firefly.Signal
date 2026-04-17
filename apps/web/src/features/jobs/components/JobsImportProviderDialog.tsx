import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle
} from "@mui/material";
import type { JobImportRunResponseDto } from "@/api/jobs/jobs.types";
import { JobImportRunsTable } from "@/features/jobs/components/JobImportRunsTable";
import { JobsImportProviderFilters } from "@/features/jobs/components/JobsImportProviderFilters";

export type JobsImportProviderFormValues = {
  where: string;
  keyword: string;
  pageIndex: string;
  pageSize: string;
  provider: "Adzuna";
  excludedKeyword: string;
  distanceKilometers: string;
  maxDaysOld: string;
  category: string;
  salaryMin: string;
  salaryMax: string;
};

type JobsImportProviderDialogProps = {
  isOpen: boolean;
  isSubmitting: boolean;
  history: JobImportRunResponseDto[];
  historyError: string | null;
  historyHasNextPage: boolean;
  historyIsLoading: boolean;
  historyPageIndex: number;
  values: JobsImportProviderFormValues;
  onChange: (values: JobsImportProviderFormValues) => void;
  onClose: () => void;
  onHistoryPageChange: (pageIndex: number) => void;
  onSubmit: () => void;
};

export function JobsImportProviderDialog({
  isOpen,
  isSubmitting,
  history,
  historyError,
  historyHasNextPage,
  historyIsLoading,
  historyPageIndex,
  values,
  onChange,
  onClose,
  onHistoryPageChange,
  onSubmit
}: JobsImportProviderDialogProps) {
  return (
    <Dialog open={isOpen} onClose={isSubmitting ? undefined : onClose} fullWidth maxWidth="lg">
      <DialogTitle>Import jobs from provider</DialogTitle>
      <DialogContent>
        <div className="space-y-5 pt-2">
          <JobsImportProviderFilters values={values} onChange={onChange} />
          <JobImportRunsTable
            history={history}
            historyError={historyError}
            historyHasNextPage={historyHasNextPage}
            historyIsLoading={historyIsLoading}
            historyPageIndex={historyPageIndex}
            onHistoryPageChange={onHistoryPageChange}
          />
        </div>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 3 }}>
        <Button onClick={onClose} color="inherit" disabled={isSubmitting}>
          Cancel
        </Button>
        <Button
          onClick={onSubmit}
          variant="contained"
          disabled={isSubmitting}
          sx={{
            bgcolor: "accent.main",
            "&:hover": { bgcolor: "accent.dark" }
          }}
        >
          {isSubmitting ? "Importing..." : "Run import"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
