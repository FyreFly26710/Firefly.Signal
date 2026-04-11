import { Checkbox, Dialog, DialogActions, DialogContent, DialogTitle, Button, FormControlLabel, MenuItem, TextField } from "@mui/material";

export type JobsImportProviderFormValues = {
  postcode: string;
  keyword: string;
  pageIndex: string;
  pageSize: string;
  provider: "Adzuna";
  excludedKeyword: string;
  distanceKilometers: string;
  category: string;
  salaryMin: string;
  salaryMax: string;
  fullTime: "" | "true" | "false";
  partTime: "" | "true" | "false";
  permanent: "" | "true" | "false";
  contract: "" | "true" | "false";
  sortBy: string;
  maxDaysOld: string;
  company: string;
  titleOnly: boolean;
  location0: string;
  location1: string;
  location2: string;
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
            label="Postcode"
            required
            size="small"
            value={values.postcode}
            onChange={(event) => onChange({ ...values, postcode: event.target.value })}
          />
          <TextField
            label="Keyword"
            required
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
          <TextField
            select
            label="Full time"
            size="small"
            value={values.fullTime}
            onChange={(event) => onChange({ ...values, fullTime: event.target.value as JobsImportProviderFormValues["fullTime"] })}
          >
            <MenuItem value="">Any</MenuItem>
            <MenuItem value="true">Yes</MenuItem>
            <MenuItem value="false">No</MenuItem>
          </TextField>
          <TextField
            select
            label="Part time"
            size="small"
            value={values.partTime}
            onChange={(event) => onChange({ ...values, partTime: event.target.value as JobsImportProviderFormValues["partTime"] })}
          >
            <MenuItem value="">Any</MenuItem>
            <MenuItem value="true">Yes</MenuItem>
            <MenuItem value="false">No</MenuItem>
          </TextField>
          <TextField
            select
            label="Permanent"
            size="small"
            value={values.permanent}
            onChange={(event) => onChange({ ...values, permanent: event.target.value as JobsImportProviderFormValues["permanent"] })}
          >
            <MenuItem value="">Any</MenuItem>
            <MenuItem value="true">Yes</MenuItem>
            <MenuItem value="false">No</MenuItem>
          </TextField>
          <TextField
            select
            label="Contract"
            size="small"
            value={values.contract}
            onChange={(event) => onChange({ ...values, contract: event.target.value as JobsImportProviderFormValues["contract"] })}
          >
            <MenuItem value="">Any</MenuItem>
            <MenuItem value="true">Yes</MenuItem>
            <MenuItem value="false">No</MenuItem>
          </TextField>
          <TextField
            label="Sort by"
            size="small"
            value={values.sortBy}
            onChange={(event) => onChange({ ...values, sortBy: event.target.value })}
          />
          <TextField
            label="Max days old"
            size="small"
            type="number"
            value={values.maxDaysOld}
            onChange={(event) => onChange({ ...values, maxDaysOld: event.target.value })}
          />
          <TextField
            label="Company"
            size="small"
            value={values.company}
            onChange={(event) => onChange({ ...values, company: event.target.value })}
          />
          <TextField
            label="Location 0"
            size="small"
            value={values.location0}
            onChange={(event) => onChange({ ...values, location0: event.target.value })}
          />
          <TextField
            label="Location 1"
            size="small"
            value={values.location1}
            onChange={(event) => onChange({ ...values, location1: event.target.value })}
          />
          <TextField
            label="Location 2"
            size="small"
            value={values.location2}
            onChange={(event) => onChange({ ...values, location2: event.target.value })}
          />
        </div>

        <FormControlLabel
          className="mt-4"
          control={
            <Checkbox
              checked={values.titleOnly}
              onChange={(event) => onChange({ ...values, titleOnly: event.target.checked })}
            />
          }
          label="Search title only"
        />
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
