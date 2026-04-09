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
          jobRefreshRunId: null,
          sourceJobId: "reed-42",
          sourceAdReference: null,
          title: "Senior Product Designer",
          description: "Lead product design across the platform.",
          company: "Firefly Labs",
          companyDisplayName: "Firefly Labs",
          companyCanonicalName: "firefly-labs",
          postcode: "EC2A",
          locationName: "London",
          locationDisplayName: "London",
          locationAreaJson: null,
          latitude: null,
          longitude: null,
          categoryTag: null,
          categoryLabel: null,
          summary: "Lead product design across the platform.",
          url: "https://example.com/jobs/42",
          sourceName: "Reed",
          salaryMin: 80000,
          salaryMax: 95000,
          salaryCurrency: "GBP",
          salaryIsPredicted: false,
          contractTime: "full_time",
          contractType: "permanent",
          isFullTime: true,
          isPartTime: false,
          isPermanent: true,
          isContract: false,
          isRemote: true,
          postedAtUtc: "2025-01-02T09:00:00.000Z",
          importedAtUtc: "2025-01-02T10:00:00.000Z",
          lastSeenAtUtc: "2025-01-02T11:00:00.000Z",
          isHidden: false,
          rawPayloadJson: "{}"
        }
      ]
    }, {
      postcode: "EC2A",
      keyword: "designer"
    });

    expect(result).toEqual({
      postcode: "EC2A",
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
          type: "permanent"
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
          jobRefreshRunId: null,
          sourceJobId: "linkedin-abc",
          sourceAdReference: null,
          title: "Platform Engineer",
          description: "Build internal tooling.",
          company: "Signal",
          companyDisplayName: null,
          companyCanonicalName: null,
          postcode: "M1",
          locationName: "Manchester",
          locationDisplayName: null,
          locationAreaJson: null,
          latitude: null,
          longitude: null,
          categoryTag: null,
          categoryLabel: null,
          summary: "Build internal tooling.",
          url: "https://example.com/jobs/abc",
          sourceName: "LinkedIn",
          salaryMin: null,
          salaryMax: null,
          salaryCurrency: null,
          salaryIsPredicted: null,
          contractTime: null,
          contractType: null,
          isFullTime: false,
          isPartTime: false,
          isPermanent: false,
          isContract: false,
          isRemote: false,
          postedAtUtc: "not-a-date",
          importedAtUtc: "2025-01-02T10:00:00.000Z",
          lastSeenAtUtc: "2025-01-02T11:00:00.000Z",
          isHidden: false,
          rawPayloadJson: "{}"
        }
      ]
    }, {
      postcode: "M1",
      keyword: "engineer"
    });

    expect(result.jobs[0]?.postedDate).toBe("Date unavailable");
    expect(result.jobs[0]?.location).toBe("Manchester");
  });
});
