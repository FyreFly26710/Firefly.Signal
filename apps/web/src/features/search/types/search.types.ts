import type { JobCardModel } from "@/features/jobs/types/job.types";

export type SearchCriteria = {
  postcode: string;
  keyword: string;
};

export type JobCardDto = {
  id: string | number;
  title: string;
  company: string;
  locationName: string;
  summary: string;
  url: string;
  sourceName: string;
  isRemote: boolean;
  postedAtUtc: string;
};

export type SearchJobsResponseDto = {
  postcode: string;
  keyword: string;
  pageIndex: number;
  pageSize: number;
  totalCount: number;
  jobs: JobCardDto[];
};

export type SearchViewModel = {
  postcode: string;
  keyword: string;
  totalCount: number;
  jobs: JobCardModel[];
};

export type SearchStatus = "idle" | "loading" | "success" | "empty" | "error";
