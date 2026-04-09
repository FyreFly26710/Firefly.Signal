import { Checkbox, FormControlLabel, TextField } from "@mui/material";
import type { CreateJobRequestDto } from "@/api/jobs/jobs.types";
import { JobFieldSection } from "@/features/jobs/components/JobFieldSection";
import {
  setBooleanValue,
  setNullableBooleanValue,
  setStringValue,
  type JobFormSetter
} from "@/features/jobs/components/JobEditorFormState";

type JobEditorFlagsSectionProps = {
  formValues: CreateJobRequestDto;
  setFormValues: JobFormSetter;
};

export function JobEditorFlagsSection({
  formValues,
  setFormValues
}: JobEditorFlagsSectionProps) {
  return (
    <JobFieldSection title="Flags and timestamps">
      <div className="grid gap-2 md:col-span-2 md:grid-cols-3">
        <FormControlLabel
          control={
            <Checkbox
              checked={formValues.isFullTime}
              onChange={(event) => setBooleanValue(setFormValues, "isFullTime", event.target.checked)}
            />
          }
          label="Full time"
        />
        <FormControlLabel
          control={
            <Checkbox
              checked={formValues.isPartTime}
              onChange={(event) => setBooleanValue(setFormValues, "isPartTime", event.target.checked)}
            />
          }
          label="Part time"
        />
        <FormControlLabel
          control={
            <Checkbox
              checked={formValues.isPermanent}
              onChange={(event) =>
                setBooleanValue(setFormValues, "isPermanent", event.target.checked)
              }
            />
          }
          label="Permanent"
        />
        <FormControlLabel
          control={
            <Checkbox
              checked={formValues.isContract}
              onChange={(event) =>
                setBooleanValue(setFormValues, "isContract", event.target.checked)
              }
            />
          }
          label="Contract"
        />
        <FormControlLabel
          control={
            <Checkbox
              checked={formValues.isRemote}
              onChange={(event) => setBooleanValue(setFormValues, "isRemote", event.target.checked)}
            />
          }
          label="Remote"
        />
        <FormControlLabel
          control={
            <Checkbox
              checked={formValues.isHidden}
              onChange={(event) => setBooleanValue(setFormValues, "isHidden", event.target.checked)}
            />
          }
          label="Hidden"
        />
        <FormControlLabel
          control={
            <Checkbox
              checked={formValues.salaryIsPredicted ?? false}
              onChange={(event) =>
                setNullableBooleanValue(setFormValues, "salaryIsPredicted", event.target.checked)
              }
            />
          }
          label="Salary predicted"
        />
      </div>
      <TextField
        label="Posted at UTC"
        value={formValues.postedAtUtc}
        onChange={(event) => setStringValue(setFormValues, "postedAtUtc", event.target.value)}
        fullWidth
        required
      />
      <TextField
        label="Imported at UTC"
        value={formValues.importedAtUtc}
        onChange={(event) => setStringValue(setFormValues, "importedAtUtc", event.target.value)}
        fullWidth
        required
      />
      <TextField
        label="Last seen at UTC"
        value={formValues.lastSeenAtUtc}
        onChange={(event) => setStringValue(setFormValues, "lastSeenAtUtc", event.target.value)}
        fullWidth
        required
      />
    </JobFieldSection>
  );
}
