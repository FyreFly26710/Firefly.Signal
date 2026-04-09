import type { JobCardModel } from "@/features/jobs/types/job.types";

export type SearchCriteria = {
  postcode: string;
  keyword: string;
  pageIndex: number;
  pageSize: number;
};

export type SearchViewModel = {
  postcode: string;
  keyword: string;
  pageIndex: number;
  pageSize: number;
  totalCount: number;
  jobs: JobCardModel[];
};

export type SearchStatus = "idle" | "loading" | "success" | "empty" | "error";
