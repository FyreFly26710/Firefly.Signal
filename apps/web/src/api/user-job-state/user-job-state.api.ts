import { deleteJson, postJson } from "@/lib/http/client";
import type { UserJobStateDto } from "@/api/user-job-state/user-job-state.types";

export async function saveJob(jobId: string): Promise<UserJobStateDto> {
  return postJson<UserJobStateDto, Record<string, never>>(`/api/job-search/jobs/${jobId}/save`, {});
}

export async function unsaveJob(jobId: string): Promise<UserJobStateDto> {
  return deleteJson<UserJobStateDto, Record<string, never>>(`/api/job-search/jobs/${jobId}/save`, {});
}

export async function hideJob(jobId: string): Promise<UserJobStateDto> {
  return postJson<UserJobStateDto, Record<string, never>>(`/api/job-search/jobs/${jobId}/hide`, {});
}

export async function unhideJob(jobId: string): Promise<UserJobStateDto> {
  return deleteJson<UserJobStateDto, Record<string, never>>(`/api/job-search/jobs/${jobId}/hide`, {});
}
