import type { FormEvent } from "react";
import type { CreateJobRequestDto } from "@/api/jobs/jobs.types";
import { JobEditorActions } from "@/features/jobs/components/JobEditorActions";
import { JobEditorClassificationSection } from "@/features/jobs/components/JobEditorClassificationSection";
import { JobEditorCoreSection } from "@/features/jobs/components/JobEditorCoreSection";
import { JobEditorFlagsSection } from "@/features/jobs/components/JobEditorFlagsSection";
import {
  type JobFormSetter
} from "@/features/jobs/components/JobEditorFormState";
import { JobEditorLocationSection } from "@/features/jobs/components/JobEditorLocationSection";
import { JobEditorSourceSection } from "@/features/jobs/components/JobEditorSourceSection";

type JobEditorFormProps = {
  formValues: CreateJobRequestDto;
  isAdmin: boolean;
  isCreateMode: boolean;
  isDeleting: boolean;
  isHiding: boolean;
  isSaving: boolean;
  onDelete: () => void;
  onHide: () => void;
  onSubmit: (event: FormEvent<HTMLFormElement>) => void;
  setFormValues: JobFormSetter;
};

export function JobEditorForm({
  formValues,
  isAdmin,
  isCreateMode,
  isDeleting,
  isHiding,
  isSaving,
  onDelete,
  onHide,
  onSubmit,
  setFormValues
}: JobEditorFormProps) {
  return (
    <form onSubmit={onSubmit} className="mt-6 space-y-6">
      <JobEditorCoreSection formValues={formValues} setFormValues={setFormValues} />
      <JobEditorSourceSection formValues={formValues} setFormValues={setFormValues} />
      <JobEditorLocationSection formValues={formValues} setFormValues={setFormValues} />
      <JobEditorClassificationSection formValues={formValues} setFormValues={setFormValues} />
      <JobEditorFlagsSection formValues={formValues} setFormValues={setFormValues} />
      <JobEditorActions
        isAdmin={isAdmin}
        isCreateMode={isCreateMode}
        isDeleting={isDeleting}
        isHiding={isHiding}
        isSaving={isSaving}
        onDelete={onDelete}
        onHide={onHide}
      />
    </form>
  );
}
