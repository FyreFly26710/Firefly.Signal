import { TextField } from "@mui/material";
import type { CreateJobRequestDto } from "@/api/jobs/jobs.types";
import { JobFieldSection } from "@/features/jobs/components/JobFieldSection";
import {
  setNullableNumberValue,
  setNullableStringValue,
  setStringValue,
  type JobFormSetter
} from "@/features/jobs/components/JobEditorFormState";

type JobEditorLocationSectionProps = {
  formValues: CreateJobRequestDto;
  setFormValues: JobFormSetter;
};

export function JobEditorLocationSection({
  formValues,
  setFormValues
}: JobEditorLocationSectionProps) {
  return (
    <JobFieldSection title="Location">
      <TextField
        label="Postcode"
        value={formValues.postcode}
        onChange={(event) => setStringValue(setFormValues, "postcode", event.target.value)}
        fullWidth
        required
      />
      <TextField
        label="Location name"
        value={formValues.locationName}
        onChange={(event) => setStringValue(setFormValues, "locationName", event.target.value)}
        fullWidth
        required
      />
      <TextField
        label="Location display name"
        value={formValues.locationDisplayName ?? ""}
        onChange={(event) =>
          setNullableStringValue(setFormValues, "locationDisplayName", event.target.value)
        }
        fullWidth
      />
      <TextField
        label="Location area JSON"
        value={formValues.locationAreaJson ?? ""}
        onChange={(event) =>
          setNullableStringValue(setFormValues, "locationAreaJson", event.target.value)
        }
        fullWidth
      />
      <TextField
        label="Latitude"
        value={formValues.latitude ?? ""}
        onChange={(event) => setNullableNumberValue(setFormValues, "latitude", event.target.value)}
        fullWidth
      />
      <TextField
        label="Longitude"
        value={formValues.longitude ?? ""}
        onChange={(event) =>
          setNullableNumberValue(setFormValues, "longitude", event.target.value)
        }
        fullWidth
      />
    </JobFieldSection>
  );
}
