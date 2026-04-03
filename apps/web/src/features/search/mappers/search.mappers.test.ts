import { describe, expect, it } from "vitest";
import { mapSearchResponse } from "@/features/search/mappers/search.mappers";

describe("mapSearchResponse", () => {
  it("maps backend job results into frontend display models", () => {
    const result = mapSearchResponse({
      postcode: "SW1A",
      keyword: ".NET",
      pageIndex: 0,
      pageSize: 20,
      totalCount: 1,
      jobs: [
        {
          id: 1,
          title: ".NET Developer",
          company: "North Star Tech",
          locationName: "London",
          summary: "Build internal APIs.",
          url: "https://example.com/job",
          sourceName: "sample-feed",
          isRemote: true,
          postedAtUtc: "2026-04-03T12:00:00Z"
        }
      ]
    });

    expect(result.totalCount).toBe(1);
    expect(result.jobs[0]).toMatchObject({
      id: 1,
      title: ".NET Developer",
      company: "North Star Tech",
      locationLabel: "London · Remote",
      sourceLabel: "sample-feed"
    });
  });
});
