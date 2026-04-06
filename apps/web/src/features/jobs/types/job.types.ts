export type JobFreshness = "fresh" | "recent" | "older";

export type MockJob = {
  id: string;
  title: string;
  employer: string;
  location: string;
  postcode: string;
  source: string;
  summary: string;
  postedDate: string;
  freshness: JobFreshness;
  featured?: boolean;
  url: string;
  salary?: string;
  type?: string;
};
