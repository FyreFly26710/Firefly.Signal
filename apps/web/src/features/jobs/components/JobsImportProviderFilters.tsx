import { MenuItem, TextField } from "@mui/material";
import type { JobsImportProviderFormValues } from "@/features/jobs/components/JobsImportProviderDialog";

type JobsImportProviderFiltersProps = {
  values: JobsImportProviderFormValues;
  onChange: (values: JobsImportProviderFormValues) => void;
};

export function JobsImportProviderFilters({
  values,
  onChange
}: JobsImportProviderFiltersProps) {
  return (
    <div className="rounded-2xl border border-divider bg-background-subtle p-4">
      <p className="font-mono text-[11px] tracking-[0.18em] text-foreground-tertiary">
        IMPORT FILTERS
      </p>

      <div className="mt-4 space-y-3">
        <div className="grid gap-3 xl:grid-cols-[minmax(260px,1.15fr)_minmax(220px,0.9fr)_minmax(180px,0.7fr)]">
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
        </div>

        <div className="grid gap-3 xl:grid-cols-[minmax(220px,1fr)_minmax(160px,0.65fr)_minmax(160px,0.65fr)_minmax(180px,0.8fr)]">
          <TextField
            label="Excluded keyword"
            size="small"
            value={values.excludedKeyword}
            onChange={(event) => onChange({ ...values, excludedKeyword: event.target.value })}
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
        </div>

        <div className="grid gap-3 xl:grid-cols-[minmax(140px,0.55fr)_minmax(140px,0.55fr)_minmax(160px,0.6fr)_minmax(160px,0.6fr)]">
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
      </div>
    </div>
  );
}
