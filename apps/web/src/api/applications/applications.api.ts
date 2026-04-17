import { getJson, postJson, putJson } from "@/lib/http/client";
import type {
  AdvanceApplicationStatusRequestDto,
  AppliedJobDetailResponseDto,
  AppliedJobSummaryResponseDto,
  ApplyJobRequestDto,
  JobApplicationResponseDto,
  UpdateApplicationNoteRequestDto
} from "@/api/applications/applications.types";

export async function applyToJob(jobId: number, request: ApplyJobRequestDto = {}): Promise<JobApplicationResponseDto> {
  return postJson<JobApplicationResponseDto, ApplyJobRequestDto>(`/api/job-search/jobs/${jobId}/apply`, request);
}

export async function advanceApplicationStatus(
  jobId: number,
  request: AdvanceApplicationStatusRequestDto
): Promise<JobApplicationResponseDto> {
  return putJson<JobApplicationResponseDto, AdvanceApplicationStatusRequestDto>(
    `/api/job-search/jobs/${jobId}/apply/status`,
    request
  );
}

export async function updateApplicationNote(
  jobId: number,
  request: UpdateApplicationNoteRequestDto
): Promise<JobApplicationResponseDto> {
  return putJson<JobApplicationResponseDto, UpdateApplicationNoteRequestDto>(
    `/api/job-search/jobs/${jobId}/apply/note`,
    request
  );
}

export async function getAppliedJobs(): Promise<AppliedJobSummaryResponseDto[]> {
  return getJson<AppliedJobSummaryResponseDto[]>("/api/job-search/applications");
}

export async function getApplicationDetail(applicationId: number): Promise<AppliedJobDetailResponseDto> {
  return getJson<AppliedJobDetailResponseDto>(`/api/job-search/applications/${applicationId}`);
}
