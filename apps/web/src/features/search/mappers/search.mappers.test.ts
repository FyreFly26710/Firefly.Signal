import { describe, expect, it } from "vitest";
import { mapSearchResponse } from "@/features/search/mappers/search.mappers";

describe("mapSearchResponse", () => {
  it("maps API jobs into the UI model", () => {
    const result = mapSearchResponse({
      postcode: "EC2A",
      keyword: "designer",
      pageIndex: 0,
      pageSize: 20,
      totalCount: 1,
      jobs: [
        {
          id: 42,
          title: "Senior Product Designer",
          company: "Firefly Labs",
          locationName: "London",
          summary: "Lead product design across the platform.",
          url: "https://example.com/jobs/42",
          sourceName: "Reed",
          isRemote: true,
          postedAtUtc: "2025-01-02T09:00:00.000Z"
        }
      ]
    });

    expect(result).toEqual({
      postcode: "EC2A",
      keyword: "designer",
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
          postedDate: "2 Jan 2025"
        }
      ]
    });
  });

  it("falls back when the posted date is invalid", () => {
    const result = mapSearchResponse({
      postcode: "M1",
      keyword: "engineer",
      pageIndex: 0,
      pageSize: 20,
      totalCount: 1,
      jobs: [
        {
          id: "abc",
          title: "Platform Engineer",
          company: "Signal",
          locationName: "Manchester",
          summary: "Build internal tooling.",
          url: "https://example.com/jobs/abc",
          sourceName: "LinkedIn",
          isRemote: false,
          postedAtUtc: "not-a-date"
        }
      ]
    });

    expect(result.jobs[0]?.postedDate).toBe("Date unavailable");
    expect(result.jobs[0]?.location).toBe("Manchester");
  });
});
