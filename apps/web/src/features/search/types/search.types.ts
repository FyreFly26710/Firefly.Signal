import type { JobCardModel } from "@/features/jobs/types/job.types";

export type SearchCriteria = {
  postcode: string;
  keyword: string;
};

export type SearchViewModel = {
  postcode: string;
  keyword: string;
  totalCount: number;
  jobs: JobCardModel[];
};

export type SearchStatus = "idle" | "loading" | "success" | "empty" | "error";
