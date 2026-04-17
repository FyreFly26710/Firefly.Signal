import { describe, expect, it } from "vitest";
import {
  createSearchParams,
  createSearchPath,
  hasSearchCriteria,
  normalizeSearchCriteria,
  readSearchCriteria
} from "@/features/search/lib/search-query";

describe("search-query", () => {
  it("trims whitespace when normalizing search criteria", () => {
    expect(
      normalizeSearchCriteria({
        keyword: "  frontend engineer  ",
        postcode: "  SW1A 1AA  ",
        salaryMin: 40000,
        salaryMax: 80000,
        datePosted: "anytime",
        sortBy: "date-desc",
        pageIndex: 2,
        pageSize: 50
      })
    ).toEqual({
      keyword: "frontend engineer",
      postcode: "SW1A 1AA",
      salaryMin: 40000,
      salaryMax: 80000,
      datePosted: "anytime",
      sortBy: "date-desc",
      pageIndex: 2,
      pageSize: 50
    });
  });

  it("reads and trims criteria from URL search params", () => {
    const params = new URLSearchParams({
      keyword: "  designer ",
      postcode: " EC2A ",
      salaryMin: "30000",
      salaryMax: "70000",
      datePosted: "1week",
      sortBy: "date-asc",
      pageIndex: "3",
      pageSize: "100"
    });

    expect(readSearchCriteria(params)).toEqual({
      keyword: "designer",
      postcode: "EC2A",
      salaryMin: 30000,
      salaryMax: 70000,
      datePosted: "1week",
      sortBy: "date-asc",
      pageIndex: 3,
      pageSize: 100
    });
  });

  it("reads null salary when params are absent", () => {
    const params = new URLSearchParams({ keyword: "engineer" });
    const criteria = readSearchCriteria(params);
    expect(criteria.salaryMin).toBeNull();
    expect(criteria.salaryMax).toBeNull();
    expect(criteria.datePosted).toBe("anytime");
  });

  it("creates search params without blank or default values", () => {
    expect(
      createSearchParams({
        keyword: "  product manager  ",
        postcode: "   ",
        salaryMin: null,
        salaryMax: null,
        datePosted: "anytime",
        sortBy: "date-desc",
        pageIndex: 0,
        pageSize: 50
      }).toString()
    ).toBe("keyword=product+manager&pageSize=50");
  });

  it("includes salary, datePosted and non-default sortBy in search params", () => {
    expect(
      createSearchParams({
        keyword: "engineer",
        postcode: "",
        salaryMin: 50000,
        salaryMax: 90000,
        datePosted: "1week",
        sortBy: "salary-desc",
        pageIndex: 0,
        pageSize: 20
      }).toString()
    ).toBe("keyword=engineer&salaryMin=50000&salaryMax=90000&datePosted=1week&sortBy=salary-desc");
  });

  it("builds the search path from normalized criteria", () => {
    expect(
      createSearchPath({
        keyword: "  data scientist ",
        postcode: " E1 ",
        salaryMin: null,
        salaryMax: null,
        datePosted: "anytime",
        sortBy: "date-desc",
        pageIndex: 2,
        pageSize: 100
      })
    ).toBe("/search?keyword=data+scientist&postcode=E1&pageIndex=2&pageSize=100");
  });

  it("detects whether any search criteria are present", () => {
    const base = { salaryMin: null, salaryMax: null, datePosted: "anytime" as const, sortBy: "date-desc" as const, pageIndex: 0, pageSize: 20 };
    expect(hasSearchCriteria({ keyword: "", postcode: "", ...base })).toBe(false);
    expect(hasSearchCriteria({ keyword: "", postcode: "SE1", ...base })).toBe(true);
    expect(hasSearchCriteria({ keyword: "dev", postcode: "", ...base })).toBe(true);
  });

  it("normalizes invalid paging values back to defaults", () => {
    expect(
      normalizeSearchCriteria({
        keyword: "",
        postcode: "",
        salaryMin: null,
        salaryMax: null,
        datePosted: "anytime",
        sortBy: "date-desc",
        pageIndex: -3,
        pageSize: 999
      })
    ).toEqual({
      keyword: "",
      postcode: "",
      salaryMin: null,
      salaryMax: null,
      datePosted: "anytime",
      sortBy: "date-desc",
      pageIndex: 0,
      pageSize: 20
    });
  });

  it("normalizes unknown sortBy and datePosted values to defaults", () => {
    expect(
      normalizeSearchCriteria({
        keyword: "",
        postcode: "",
        salaryMin: null,
        salaryMax: null,
        datePosted: "unknown" as never,
        sortBy: "unknown" as never,
        pageIndex: 0,
        pageSize: 20
      })
    ).toMatchObject({ sortBy: "date-desc", datePosted: "anytime" });
  });

  it("normalizes negative salary values to null", () => {
    expect(
      normalizeSearchCriteria({
        keyword: "",
        postcode: "",
        salaryMin: -1000,
        salaryMax: -500,
        datePosted: "anytime",
        sortBy: "date-desc",
        pageIndex: 0,
        pageSize: 20
      })
    ).toMatchObject({ salaryMin: null, salaryMax: null });
  });
});
