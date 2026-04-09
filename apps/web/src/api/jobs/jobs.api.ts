import { deleteJson, deleteRequest, getJson, postJson, putJson } from "@/lib/http/client";
import type {
  CreateJobRequestDto,
  DeleteJobsResponseDto,
  GetJobsPageQueryDto,
  HideJobsResponseDto,
  IdBatchRequestDto,
  JobDetailsResponseDto,
  JobsPageResponseDto,
  UpdateJobRequestDto
} from "@/api/jobs/jobs.types";

export async function getJobsPage(query: GetJobsPageQueryDto): Promise<JobsPageResponseDto> {
  const searchParams = new URLSearchParams({
    pageIndex: String(query.pageIndex),
    pageSize: String(query.pageSize)
  });

  appendOptional(searchParams, "keyword", query.keyword);
  appendOptional(searchParams, "company", query.company);
  appendOptional(searchParams, "postcode", query.postcode);
  appendOptional(searchParams, "location", query.location);
  appendOptional(searchParams, "sourceName", query.sourceName);
  appendOptional(searchParams, "categoryTag", query.categoryTag);

  if (query.isHidden !== undefined) {
    searchParams.set("isHidden", String(query.isHidden));
  }

  return getJson<JobsPageResponseDto>(`/api/job-search/jobs?${searchParams.toString()}`);
}

export async function getJobById(jobId: number): Promise<JobDetailsResponseDto> {
  return getJson<JobDetailsResponseDto>(`/api/job-search/jobs/${jobId}`);
}

export async function createJob(request: CreateJobRequestDto): Promise<JobDetailsResponseDto> {
  return postJson<JobDetailsResponseDto, CreateJobRequestDto>("/api/job-search/jobs", request);
}

export async function updateJob(
  jobId: number,
  request: UpdateJobRequestDto
): Promise<JobDetailsResponseDto> {
  return putJson<JobDetailsResponseDto, UpdateJobRequestDto>(
    `/api/job-search/jobs/${jobId}`,
    request
  );
}

export async function hideJob(jobId: number): Promise<HideJobsResponseDto> {
  return postJson<HideJobsResponseDto, Record<string, never>>(
    `/api/job-search/jobs/${jobId}/hide`,
    {}
  );
}

export async function hideJobs(ids: number[]): Promise<HideJobsResponseDto> {
  return postJson<HideJobsResponseDto, IdBatchRequestDto>("/api/job-search/jobs/hide", {
    ids
  });
}

export async function deleteJob(jobId: number): Promise<void> {
  await deleteRequest(`/api/job-search/jobs/${jobId}`);
}

export async function deleteJobs(ids: number[]): Promise<DeleteJobsResponseDto> {
  return deleteJson<DeleteJobsResponseDto, IdBatchRequestDto>("/api/job-search/jobs", { ids });
}

function appendOptional(params: URLSearchParams, key: string, value: string | undefined) {
  if (!value) {
    return;
  }

  params.set(key, value);
}
