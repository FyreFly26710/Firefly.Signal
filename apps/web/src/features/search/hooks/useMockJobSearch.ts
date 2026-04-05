import { useState } from "react";
import type { SearchStatus, SearchViewModel } from "@/features/search/types/search.types";

type SearchState = {
  status: SearchStatus;
  data: SearchViewModel | null;
  errorMessage: string | null;
};

const seedJobs = [
  {
    id: 101,
    title: ".NET Backend Developer",
    company: "North Star Tech",
    locationLabel: "London · Remote",
    summary: "Own pragmatic APIs, data pipelines, and the first protected product flows for a lean hiring intelligence tool.",
    url: "https://example.com/jobs/backend-dotnet",
    sourceLabel: "sample-feed",
    isRemote: true,
    postedLabel: "4 Apr 2026",
    postcodeHints: ["SW1A", "EC1A", "LONDON"],
    keywordHints: [".NET", "backend", "platform", "api"]
  },
  {
    id: 102,
    title: "Product Data Analyst",
    company: "Signal Works",
    locationLabel: "Manchester",
    summary: "Turn noisy job and product signals into clear dashboards, sharper search criteria, and better operator decisions.",
    url: "https://example.com/jobs/product-data-analyst",
    sourceLabel: "sample-feed",
    isRemote: false,
    postedLabel: "3 Apr 2026",
    postcodeHints: ["M1", "MANCHESTER", "M4"],
    keywordHints: ["analyst", "data", "product", "insight"]
  },
  {
    id: 103,
    title: "Senior Platform Engineer",
    company: "Cloud Rail",
    locationLabel: "Edinburgh · Remote",
    summary: "Improve deployment confidence, service reliability, and developer speed across a small multi-service platform.",
    url: "https://example.com/jobs/platform-engineer",
    sourceLabel: "sample-feed",
    isRemote: true,
    postedLabel: "2 Apr 2026",
    postcodeHints: ["EH1", "EDINBURGH", "EH2"],
    keywordHints: ["platform", "engineer", "infra", "devops"]
  }
];

const initialState: SearchState = {
  status: "idle",
  data: null,
  errorMessage: null
};

const wait = (durationMs: number) => new Promise((resolve) => window.setTimeout(resolve, durationMs));

export function useMockJobSearch() {
  const [state, setState] = useState<SearchState>(initialState);

  async function runSearch(postcode: string, keyword: string) {
    setState((current) => ({
      ...current,
      status: "loading",
      errorMessage: null
    }));

    await wait(420);

    const normalizedPostcode = postcode.trim().toUpperCase();
    const normalizedKeyword = keyword.trim().toLowerCase();

    if (normalizedKeyword.includes("error")) {
      setState({
        status: "error",
        data: null,
        errorMessage: "The mock request was forced into an error state. Try another keyword."
      });
      return;
    }

    const jobs = seedJobs.filter((job) => {
      const matchesPostcode = job.postcodeHints.some((hint) => normalizedPostcode.includes(hint));
      const matchesKeyword = job.keywordHints.some((hint) => normalizedKeyword.includes(hint));
      return matchesPostcode || matchesKeyword;
    }).map((job) => ({
      id: job.id,
      title: job.title,
      company: job.company,
      locationLabel: job.locationLabel,
      summary: job.summary,
      url: job.url,
      sourceLabel: job.sourceLabel,
      isRemote: job.isRemote,
      postedLabel: job.postedLabel
    }));

    setState({
      status: jobs.length === 0 ? "empty" : "success",
      data: {
        postcode,
        keyword,
        totalCount: jobs.length,
        jobs
      },
      errorMessage: null
    });
  }

  return {
    ...state,
    runSearch
  };
}
