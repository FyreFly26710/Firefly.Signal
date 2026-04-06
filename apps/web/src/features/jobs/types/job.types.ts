export type JobFreshness = "fresh" | "recent" | "older";

export type JobCardModel = {
  id: string;
  title: string;
  employer: string;
  location: string;
  source: string;
  summary: string;
  postedDate: string;
  freshness?: JobFreshness;
  featured?: boolean;
  url: string;
  salary?: string;
  type?: string;
};

export type MockJob = JobCardModel & {
  postcode: string;
  freshness: JobFreshness;
};
