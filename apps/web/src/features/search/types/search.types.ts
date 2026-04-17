import type { JobCardModel } from "@/features/jobs/types/job.types";

export type SearchSortBy = "date-desc" | "date-asc" | "salary-desc" | "salary-asc";

export type DatePosted = "anytime" | "today" | "3days" | "1week" | "2weeks";

export type SearchViewMode = "card" | "table";

export type SearchCriteria = {
  keyword: string;
  postcode: string;
  salaryMin: number | null;
  salaryMax: number | null;
  datePosted: DatePosted;
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
