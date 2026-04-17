/// Response for job list pages — a focused projection of job fields
/// plus per-user save/hide state (defaults to false when unauthenticated).
export type JobSearchResultDto = {
  id: number;
  sourceJobId: string;
  title: string;
  summary: string;
  url: string;
  company: string;
  companyDisplayName: string | null;
  locationName: string;
  locationDisplayName: string | null;
  isRemote: boolean;
  isHidden: boolean;          // catalog hide status
  salaryMin: number | null;
  salaryMax: number | null;
  salaryCurrency: string | null;
  contractType: string | null;
  contractTime: string | null;
  isFullTime: boolean;
  isPartTime: boolean;
  isPermanent: boolean;
  isContract: boolean;
  sourceName: string;
  postedAtUtc: string;
  isSaved: boolean;           // user's saved state
  isUserHidden: boolean;      // user's personal hide state
};

export type JobDetailsResponseDto = {
  id: number;
  jobRefreshRunId: number | null;
  sourceName: string;
  sourceJobId: string;
  sourceAdReference: string | null;
  title: string;
  description: string;
  summary: string;
  url: string;
  company: string;
  companyDisplayName: string | null;
  companyCanonicalName: string | null;
  postcode: string;
  locationName: string;
  locationDisplayName: string | null;
  locationAreaJson: string | null;
  latitude: number | null;
  longitude: number | null;
  categoryTag: string | null;
  categoryLabel: string | null;
  salaryMin: number | null;
  salaryMax: number | null;
  salaryCurrency: string | null;
  salaryIsPredicted: boolean | null;
  contractTime: string | null;
  contractType: string | null;
  isFullTime: boolean;
  isPartTime: boolean;
  isPermanent: boolean;
  isContract: boolean;
  isRemote: boolean;
  postedAtUtc: string;
  importedAtUtc: string;
  lastSeenAtUtc: string;
  isHidden: boolean;
  rawPayloadJson: string;
};

export type JobsPageResponseDto = {
  pageIndex: number;
  pageSize: number;
  totalCount: number;
  items: JobSearchResultDto[];
};

export type PagedResponseDto<TItem> = {
  pageIndex: number;
  pageSize: number;
  totalCount: number;
  items: TItem[];
};

export type GetJobsPageQueryDto = {
  pageIndex: number;
  pageSize: number;
  keyword?: string;
  company?: string;
  postcode?: string;
  location?: string;
  sourceName?: string;
  categoryTag?: string;
  isHidden?: boolean;
};

export type JobWriteRequestDto = {
  jobRefreshRunId: number | null;
  sourceName: string;
  sourceJobId: string;
  sourceAdReference: string | null;
  title: string;
  description: string;
  summary: string;
  url: string;
  company: string;
  companyDisplayName: string | null;
  companyCanonicalName: string | null;
  postcode: string;
  locationName: string;
  locationDisplayName: string | null;
  locationAreaJson: string | null;
  latitude: number | null;
  longitude: number | null;
  categoryTag: string | null;
  categoryLabel: string | null;
  salaryMin: number | null;
  salaryMax: number | null;
  salaryCurrency: string | null;
  salaryIsPredicted: boolean | null;
  contractTime: string | null;
  contractType: string | null;
  isFullTime: boolean;
  isPartTime: boolean;
  isPermanent: boolean;
  isContract: boolean;
  isRemote: boolean;
  postedAtUtc: string;
  importedAtUtc: string;
  lastSeenAtUtc: string;
  isHidden: boolean;
  rawPayloadJson: string;
};

export type CreateJobRequestDto = JobWriteRequestDto;
export type UpdateJobRequestDto = JobWriteRequestDto;

export type HideJobsResponseDto = {
  hiddenCount: number;
  hiddenIds: number[];
  missingIds: number[];
};

export type DeleteJobsResponseDto = {
  deletedCount: number;
  deletedIds: number[];
  missingIds: number[];
  notHiddenIds: number[];
};

export type IdBatchRequestDto = {
  ids: number[];
};

export type ImportJobsFromProviderRequestDto = {
  pageIndex?: number;
  pageSize?: number;
  where?: string;
  keyword?: string;
  distanceKilometers?: number;
  maxDaysOld?: number;
  category?: string;
  provider?: "Adzuna";
  excludedKeyword?: string;
  salaryMin?: number;
  salaryMax?: number;
};

export type ImportJobsResponseDto = {
  jobRefreshRunId: number;
  source: string;
  importedCount: number;
  failedCount: number;
};

export type JobImportRunResponseDto = {
  id: number;
  providerName: string;
  status: string;
  jsonFilter: string;
  pagesRequested: number;
  pagesCompleted: number;
  recordsReceived: number;
  recordsInserted: number;
  recordsHidden: number;
  recordsFailed: number;
  startedAtUtc: string;
  completedAtUtc: string | null;
  failureSummary: string | null;
};

export type GetJobImportRunsQueryDto = {
  pageIndex: number;
  pageSize: number;
};

export type ExportJobsRequestDto = {
  jobIds: number[];
};

export type ExportJobsResponseDto = {
  exportedAtUtc: string;
  count: number;
  jobs: JobWriteRequestDto[];
};
