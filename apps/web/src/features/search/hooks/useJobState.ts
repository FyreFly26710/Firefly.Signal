import { useCallback, useState } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { applyToJob } from "@/api/applications/applications.api";
import { hideJob, saveJob, unhideJob, unsaveJob } from "@/api/user-job-state/user-job-state.api";
import { applicationQueryKeys } from "@/features/applications/lib/application-status";

export type JobState = {
  isSaved: boolean;
  isHidden: boolean;
  isApplied: boolean;
};

type JobStateActions = {
  toggleSave: () => Promise<void>;
  toggleHide: () => Promise<void>;
  apply: () => Promise<void>;
};

export function useJobState(jobId: string, initial?: Partial<JobState>): JobState & JobStateActions {
  const queryClient = useQueryClient();
  const [isSaved, setIsSaved] = useState(initial?.isSaved ?? false);
  const [isHidden, setIsHidden] = useState(initial?.isHidden ?? false);
  const [isApplied, setIsApplied] = useState(initial?.isApplied ?? false);

  const toggleSave = useCallback(async () => {
    const next = !isSaved;
    setIsSaved(next);
    try {
      await (next ? saveJob(jobId) : unsaveJob(jobId));
    } catch {
      setIsSaved(!next);
    }
  }, [jobId, isSaved]);

  const toggleHide = useCallback(async () => {
    const next = !isHidden;
    setIsHidden(next);
    try {
      await (next ? hideJob(jobId) : unhideJob(jobId));
    } catch {
      setIsHidden(!next);
    }
  }, [jobId, isHidden]);

  const apply = useCallback(async () => {
    if (isApplied) {
      return;
    }

    setIsApplied(true);
    try {
      await applyToJob(Number(jobId));
      void queryClient.invalidateQueries({ queryKey: applicationQueryKeys.all });
    } catch {
      setIsApplied(false);
    }
  }, [isApplied, jobId, queryClient]);

  return { isSaved, isHidden, isApplied, toggleSave, toggleHide, apply };
}
