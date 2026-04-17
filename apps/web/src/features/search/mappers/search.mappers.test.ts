import { describe, expect, it } from "vitest";
import { mapSearchResponse } from "@/features/search/mappers/search.mappers";

describe("mapSearchResponse", () => {
  it("maps API jobs into the UI model", () => {
    const result = mapSearchResponse({
      pageIndex: 0,
      pageSize: 20,
      totalCount: 1,
      items: [
        {
          id: 42,
          sourceJobId: "reed-42",
          title: "Senior Product Designer",
          summary: "Lead product design across the platform.",
          url: "https://example.com/jobs/42",
          company: "Firefly Labs",
          companyDisplayName: "Firefly Labs",
          locationName: "London",
          locationDisplayName: "London",
          isRemote: true,
          isHidden: false,
          salaryMin: 80000,
          salaryMax: 95000,
          salaryCurrency: "GBP",
          contractTime: "full_time",
          contractType: "permanent",
          isFullTime: true,
          isPartTime: false,
          isPermanent: true,
          isContract: false,
          sourceName: "Reed",
          postedAtUtc: "2025-01-02T09:00:00.000Z",
          isSaved: false,
          isUserHidden: false
        }
      ]
    }, {
      keyword: "designer"
    });

    expect(result).toEqual({
      keyword: "designer",
      pageIndex: 0,
      pageSize: 20,
      totalCount: 1,
      jobs: [
        {
          id: "42",
          title: "Senior Product Designer",
          employer: "Firefly Labs",
          location: "London · Remote",
          summary: "Lead product design across the platform.",
          url: "https://example.com/jobs/42",
          source: "Reed",
          postedDate: "2 Jan 2025",
          salary: "£80,000 - £95,000",
          type: "permanent",
          isSaved: false,
          isHidden: false
        }
      ]
    });
  });

  it("falls back when the posted date is invalid", () => {
    const result = mapSearchResponse({
      pageIndex: 0,
      pageSize: 20,
      totalCount: 1,
      items: [
        {
          id: 99,
          sourceJobId: "linkedin-abc",
          title: "Platform Engineer",
          summary: "Build internal tooling.",
          url: "https://example.com/jobs/abc",
          company: "Signal",
          companyDisplayName: null,
          locationName: "Manchester",
          locationDisplayName: null,
          isRemote: false,
          isHidden: false,
          salaryMin: null,
          salaryMax: null,
          salaryCurrency: null,
          contractTime: null,
          contractType: null,
          isFullTime: false,
          isPartTime: false,
          isPermanent: false,
          isContract: false,
          sourceName: "LinkedIn",
          postedAtUtc: "not-a-date",
          isSaved: false,
          isUserHidden: false
        }
      ]
    }, {
      keyword: "engineer"
    });

    expect(result.jobs[0]?.postedDate).toBe("Date unavailable");
    expect(result.jobs[0]?.location).toBe("Manchester");
  });
});
