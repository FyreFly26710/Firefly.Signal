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
  isSaved?: boolean;
  isHidden?: boolean;
};

export type JobDetailModel = JobCardModel & {
  postcode: string;
  description: string;
};
