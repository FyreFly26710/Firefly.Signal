import {
  deleteJson,
  deleteRequest,
  getJson,
  postFormData,
  postJson,
  putJson
} from "@/lib/http/client";
import type {
  CreateJobRequestDto,
  DeleteJobsResponseDto,
  ExportJobsRequestDto,
  ExportJobsResponseDto,
  GetJobImportRunsQueryDto,
  GetJobsPageQueryDto,
  HideJobsResponseDto,
  IdBatchRequestDto,
  JobImportRunResponseDto,
  ImportJobsFromProviderRequestDto,
  ImportJobsResponseDto,
  JobDetailsResponseDto,
  JobsPageResponseDto,
  SearchJobsPageQueryDto,
  PagedResponseDto,
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

export async function searchJobsPage(query: SearchJobsPageQueryDto): Promise<JobsPageResponseDto> {
  const params = new URLSearchParams({
    pageIndex: String(query.pageIndex),
    pageSize: String(query.pageSize)
  });

  appendOptional(params, "keyword", query.keyword);
  appendOptional(params, "where", query.where);

  if (query.salaryMin !== undefined) params.set("salaryMin", String(query.salaryMin));
  if (query.salaryMax !== undefined) params.set("salaryMax", String(query.salaryMax));
  if (query.datePosted !== undefined) params.set("datePosted", String(query.datePosted));
  if (query.sortBy !== undefined) params.set("sortBy", query.sortBy);
  if (query.isAsc) params.set("isAsc", "true");

  return getJson<JobsPageResponseDto>(`/api/job-search/jobs?${params.toString()}`);
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

export async function catalogHideJob(jobId: number): Promise<HideJobsResponseDto> {
  return postJson<HideJobsResponseDto, Record<string, never>>(
    `/api/job-search/jobs/${jobId}/catalog-hide`,
    {}
  );
}

export async function catalogHideJobs(ids: number[]): Promise<HideJobsResponseDto> {
  return postJson<HideJobsResponseDto, IdBatchRequestDto>("/api/job-search/jobs/catalog-hide", {
    ids
  });
}

export async function deleteJob(jobId: number): Promise<void> {
  await deleteRequest(`/api/job-search/jobs/${jobId}`);
}

export async function deleteJobs(ids: number[]): Promise<DeleteJobsResponseDto> {
  return deleteJson<DeleteJobsResponseDto, IdBatchRequestDto>("/api/job-search/jobs", { ids });
}

export async function importJobsFromProvider(
  request: ImportJobsFromProviderRequestDto
): Promise<ImportJobsResponseDto> {
  return postJson<ImportJobsResponseDto, ImportJobsFromProviderRequestDto>(
    "/api/job-search/jobs/import/provider",
    request
  );
}

export async function importJobsFromJson(file: File): Promise<ImportJobsResponseDto> {
  const formData = new FormData();
  formData.append("file", file);

  return postFormData<ImportJobsResponseDto>("/api/job-search/jobs/import/json", formData);
}

export async function getRecentImportRuns(
  query: GetJobImportRunsQueryDto
): Promise<PagedResponseDto<JobImportRunResponseDto>> {
  const searchParams = new URLSearchParams({
    pageIndex: String(query.pageIndex),
    pageSize: String(query.pageSize)
  });

  return getJson<PagedResponseDto<JobImportRunResponseDto>>(
    `/api/job-search/jobs/import-runs?${searchParams.toString()}`
  );
}

export async function exportJobs(request: ExportJobsRequestDto): Promise<ExportJobsResponseDto> {
  return postJson<ExportJobsResponseDto, ExportJobsRequestDto>(
    "/api/job-search/jobs/export",
    request
  );
}

function appendOptional(params: URLSearchParams, key: string, value: string | undefined) {
  if (!value) {
    return;
  }

  params.set(key, value);
}
