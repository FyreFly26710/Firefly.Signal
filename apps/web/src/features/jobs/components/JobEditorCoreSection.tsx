import { TextField } from "@mui/material";
import type { CreateJobRequestDto } from "@/api/jobs/jobs.types";
import { JobFieldSection } from "@/features/jobs/components/JobFieldSection";
import { setStringValue, type JobFormSetter } from "@/features/jobs/components/JobEditorFormState";

type JobEditorCoreSectionProps = {
  formValues: CreateJobRequestDto;
  setFormValues: JobFormSetter;
};

export function JobEditorCoreSection({
  formValues,
  setFormValues
}: JobEditorCoreSectionProps) {
  return (
    <JobFieldSection title="Core">
      <TextField
        label="Title"
        value={formValues.title}
        onChange={(event) => setStringValue(setFormValues, "title", event.target.value)}
        fullWidth
        required
      />
      <TextField
        label="Company"
        value={formValues.company}
        onChange={(event) => setStringValue(setFormValues, "company", event.target.value)}
        fullWidth
        required
      />
      <TextField
        label="Job URL"
        value={formValues.url}
        onChange={(event) => setStringValue(setFormValues, "url", event.target.value)}
        fullWidth
        required
      />
      <TextField
        label="Summary"
        value={formValues.summary}
        onChange={(event) => setStringValue(setFormValues, "summary", event.target.value)}
        fullWidth
        required
        multiline
        minRows={3}
      />
      <TextField
        label="Description"
        value={formValues.description}
        onChange={(event) => setStringValue(setFormValues, "description", event.target.value)}
        fullWidth
        required
        multiline
        minRows={5}
      />
    </JobFieldSection>
  );
}
