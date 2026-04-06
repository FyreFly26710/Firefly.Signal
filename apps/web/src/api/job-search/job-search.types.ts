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
