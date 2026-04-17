import type { JobCardModel } from "@/features/jobs/types/job.types";

export type SearchSortBy = "date" | "salary";

export type SearchViewMode = "card" | "table";

export type SearchCriteria = {
  keyword: string;
  /** Stored in URL for round-trip but not applied — backend needs distance search. */
  where: string;
  salaryMin: number | null;
  salaryMax: number | null;
  /** Days window: null = anytime, N = last N days (cutoff = today − N). */
  datePosted: number | null;
  sortBy: SearchSortBy;
  isAsc: boolean;
  pageIndex: number;
  pageSize: number;
};

export type SearchViewModel = {
  keyword: string;
  pageIndex: number;
  pageSize: number;
  totalCount: number;
  jobs: JobCardModel[];
};

export type SearchStatus = "idle" | "loading" | "success" | "empty" | "error";
