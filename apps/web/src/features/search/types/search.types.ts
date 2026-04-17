import type { JobCardModel } from "@/features/jobs/types/job.types";

export type SearchSortBy = "date-desc" | "date-asc" | "salary-desc" | "salary-asc";

export type SearchViewMode = "card" | "table";

export type SearchCriteria = {
  postcode: string;
  keyword: string;
  company: string;
  sortBy: SearchSortBy;
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
