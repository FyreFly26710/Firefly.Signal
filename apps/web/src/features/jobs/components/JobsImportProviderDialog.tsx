import { Button, Dialog, DialogActions, DialogContent, DialogTitle, MenuItem, TextField } from "@mui/material";

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
  values: JobsImportProviderFormValues;
  onChange: (values: JobsImportProviderFormValues) => void;
  onClose: () => void;
  onSubmit: () => void;
};

export function JobsImportProviderDialog({
  isOpen,
  isSubmitting,
  values,
  onChange,
  onClose,
  onSubmit
}: JobsImportProviderDialogProps) {
  return (
    <Dialog open={isOpen} onClose={isSubmitting ? undefined : onClose} fullWidth maxWidth="md">
      <DialogTitle>Import jobs from provider</DialogTitle>
      <DialogContent>
        <div className="grid gap-4 pt-2 md:grid-cols-2">
          <TextField
            label="Where"
            required
            size="small"
            helperText="Location or postcode, for example London or SW1A 1AA."
            value={values.where}
            onChange={(event) => onChange({ ...values, where: event.target.value })}
          />
          <TextField
            label="Keyword"
            size="small"
            value={values.keyword}
            onChange={(event) => onChange({ ...values, keyword: event.target.value })}
          />
          <TextField
            select
            label="Provider"
            size="small"
            value={values.provider}
            onChange={(event) =>
              onChange({ ...values, provider: event.target.value as "Adzuna" })
            }
          >
            <MenuItem value="Adzuna">Adzuna</MenuItem>
          </TextField>
          <TextField
            label="Excluded keyword"
            size="small"
            value={values.excludedKeyword}
            onChange={(event) => onChange({ ...values, excludedKeyword: event.target.value })}
          />
          <TextField
            label="Page index"
            size="small"
            type="number"
            value={values.pageIndex}
            onChange={(event) => onChange({ ...values, pageIndex: event.target.value })}
          />
          <TextField
            label="Page size"
            size="small"
            type="number"
            value={values.pageSize}
            onChange={(event) => onChange({ ...values, pageSize: event.target.value })}
          />
          <TextField
            label="Distance (km)"
            size="small"
            type="number"
            value={values.distanceKilometers}
            onChange={(event) => onChange({ ...values, distanceKilometers: event.target.value })}
          />
          <TextField
            label="Max days old"
            size="small"
            type="number"
            value={values.maxDaysOld}
            onChange={(event) => onChange({ ...values, maxDaysOld: event.target.value })}
          />
          <TextField
            label="Category"
            size="small"
            value={values.category}
            onChange={(event) => onChange({ ...values, category: event.target.value })}
          />
          <TextField
            label="Salary min"
            size="small"
            type="number"
            value={values.salaryMin}
            onChange={(event) => onChange({ ...values, salaryMin: event.target.value })}
          />
          <TextField
            label="Salary max"
            size="small"
            type="number"
            value={values.salaryMax}
            onChange={(event) => onChange({ ...values, salaryMax: event.target.value })}
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
