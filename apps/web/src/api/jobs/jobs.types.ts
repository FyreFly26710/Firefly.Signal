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
  items: JobDetailsResponseDto[];
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
