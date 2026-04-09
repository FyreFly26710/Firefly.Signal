import { TextField } from "@mui/material";
import type { CreateJobRequestDto } from "@/api/jobs/jobs.types";
import { JobFieldSection } from "@/features/jobs/components/JobFieldSection";
import {
  setNullableNumberValue,
  setNullableStringValue,
  setStringValue,
  type JobFormSetter
} from "@/features/jobs/components/JobEditorFormState";

type JobEditorSourceSectionProps = {
  formValues: CreateJobRequestDto;
  setFormValues: JobFormSetter;
};

export function JobEditorSourceSection({
  formValues,
  setFormValues
}: JobEditorSourceSectionProps) {
  return (
    <JobFieldSection title="Source">
      <TextField
        label="Source name"
        value={formValues.sourceName}
        onChange={(event) => setStringValue(setFormValues, "sourceName", event.target.value)}
        fullWidth
        required
      />
      <TextField
        label="Source job ID"
        value={formValues.sourceJobId}
        onChange={(event) => setStringValue(setFormValues, "sourceJobId", event.target.value)}
        fullWidth
        required
      />
      <TextField
        label="Source ad reference"
        value={formValues.sourceAdReference ?? ""}
        onChange={(event) =>
          setNullableStringValue(setFormValues, "sourceAdReference", event.target.value)
        }
        fullWidth
      />
      <TextField
        label="Job refresh run ID"
        value={formValues.jobRefreshRunId ?? ""}
        onChange={(event) =>
          setNullableNumberValue(setFormValues, "jobRefreshRunId", event.target.value)
        }
        fullWidth
      />
      <TextField
        label="Raw payload JSON"
        value={formValues.rawPayloadJson}
        onChange={(event) => setStringValue(setFormValues, "rawPayloadJson", event.target.value)}
        fullWidth
        required
        multiline
        minRows={4}
      />
    </JobFieldSection>
  );
}
