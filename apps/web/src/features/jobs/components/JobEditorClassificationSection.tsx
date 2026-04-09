import { TextField } from "@mui/material";
import type { CreateJobRequestDto } from "@/api/jobs/jobs.types";
import { JobFieldSection } from "@/features/jobs/components/JobFieldSection";
import {
  setNullableNumberValue,
  setNullableStringValue,
  type JobFormSetter
} from "@/features/jobs/components/JobEditorFormState";

type JobEditorClassificationSectionProps = {
  formValues: CreateJobRequestDto;
  setFormValues: JobFormSetter;
};

export function JobEditorClassificationSection({
  formValues,
  setFormValues
}: JobEditorClassificationSectionProps) {
  return (
    <JobFieldSection title="Classification and salary">
      <TextField
        label="Category tag"
        value={formValues.categoryTag ?? ""}
        onChange={(event) =>
          setNullableStringValue(setFormValues, "categoryTag", event.target.value)
        }
        fullWidth
      />
      <TextField
        label="Category label"
        value={formValues.categoryLabel ?? ""}
        onChange={(event) =>
          setNullableStringValue(setFormValues, "categoryLabel", event.target.value)
        }
        fullWidth
      />
      <TextField
        label="Salary min"
        value={formValues.salaryMin ?? ""}
        onChange={(event) =>
          setNullableNumberValue(setFormValues, "salaryMin", event.target.value)
        }
        fullWidth
      />
      <TextField
        label="Salary max"
        value={formValues.salaryMax ?? ""}
        onChange={(event) =>
          setNullableNumberValue(setFormValues, "salaryMax", event.target.value)
        }
        fullWidth
      />
      <TextField
        label="Salary currency"
        value={formValues.salaryCurrency ?? ""}
        onChange={(event) =>
          setNullableStringValue(setFormValues, "salaryCurrency", event.target.value)
        }
        fullWidth
      />
      <TextField
        label="Contract time"
        value={formValues.contractTime ?? ""}
        onChange={(event) =>
          setNullableStringValue(setFormValues, "contractTime", event.target.value)
        }
        fullWidth
      />
      <TextField
        label="Contract type"
        value={formValues.contractType ?? ""}
        onChange={(event) =>
          setNullableStringValue(setFormValues, "contractType", event.target.value)
        }
        fullWidth
      />
    </JobFieldSection>
  );
}
