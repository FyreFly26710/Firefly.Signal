export type JobCardDto = {
  id: number;
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

export type JobCardViewModel = {
  id: number;
  title: string;
  company: string;
  locationLabel: string;
  summary: string;
  url: string;
  sourceLabel: string;
  isRemote: boolean;
  postedLabel: string;
};

export type SearchViewModel = {
  postcode: string;
  keyword: string;
  totalCount: number;
  jobs: JobCardViewModel[];
};

export type SearchStatus = "idle" | "loading" | "success" | "empty" | "error";
