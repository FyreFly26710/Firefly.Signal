import { useCallback, useState } from "react";
import { hideJob, saveJob, unhideJob, unsaveJob } from "@/api/user-job-state/user-job-state.api";

export type JobState = {
  isSaved: boolean;
  isHidden: boolean;
};

type JobStateActions = {
  toggleSave: () => Promise<void>;
  toggleHide: () => Promise<void>;
};

export function useJobState(jobId: string, initial?: Partial<JobState>): JobState & JobStateActions {
  const [isSaved, setIsSaved] = useState(initial?.isSaved ?? false);
  const [isHidden, setIsHidden] = useState(initial?.isHidden ?? false);

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

  return { isSaved, isHidden, toggleSave, toggleHide };
}
