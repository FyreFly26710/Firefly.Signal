import { describe, expect, it } from "vitest";
import {
  createSearchParams,
  createSearchPath,
  hasSearchCriteria,
  normalizeSearchCriteria,
  readSearchCriteria
} from "@/features/search/lib/search-query";

const base = {
  where: "",
  salaryMin: null,
  salaryMax: null,
  datePosted: null,
  sortBy: "date" as const,
  isAsc: false,
  pageIndex: 0,
  pageSize: 20
};

describe("search-query", () => {
  it("trims whitespace when normalizing", () => {
    expect(normalizeSearchCriteria({ ...base, keyword: "  frontend  ", where: "  London  " }))
      .toMatchObject({ keyword: "frontend", where: "London" });
  });

  it("reads criteria from URL params", () => {
    const params = new URLSearchParams({
      keyword: " designer ",
      where: " London ",
      salaryMin: "30000",
      salaryMax: "70000",
      datePosted: "7",
      sortBy: "salary",
      isAsc: "true",
      pageIndex: "2",
      pageSize: "50"
    });

    expect(readSearchCriteria(params)).toEqual({
      keyword: "designer",
      where: "London",
      salaryMin: 30000,
      salaryMax: 70000,
      datePosted: 7,
      sortBy: "salary",
      isAsc: true,
      pageIndex: 2,
      pageSize: 50
    });
  });

  it("defaults to null salary, null datePosted, date-desc when params absent", () => {
    const criteria = readSearchCriteria(new URLSearchParams({ keyword: "engineer" }));
    expect(criteria.salaryMin).toBeNull();
    expect(criteria.salaryMax).toBeNull();
    expect(criteria.datePosted).toBeNull();
    expect(criteria.sortBy).toBe("date");
    expect(criteria.isAsc).toBe(false);
    expect(criteria.where).toBe("");
  });

  it("omits default and blank values from params", () => {
    expect(
      createSearchParams({ ...base, keyword: "  product manager  " }).toString()
    ).toBe("keyword=product+manager");
  });

  it("serialises non-default salary, datePosted, sortBy, isAsc", () => {
    expect(
      createSearchParams({
        ...base,
        keyword: "engineer",
        salaryMin: 50000,
        salaryMax: 90000,
        datePosted: 7,
        sortBy: "salary",
        isAsc: true
      }).toString()
    ).toBe("keyword=engineer&salaryMin=50000&salaryMax=90000&datePosted=7&sortBy=salary&isAsc=true");
  });

  it("builds the search path", () => {
    expect(
      createSearchPath({ ...base, keyword: "data scientist", pageIndex: 1, pageSize: 50 })
    ).toBe("/search?keyword=data+scientist&pageIndex=1&pageSize=50");
  });

  it("returns /search with no params when criteria are all defaults", () => {
    expect(createSearchPath({ ...base, keyword: "" })).toBe("/search");
  });

  it("detects whether any search criteria are present", () => {
    expect(hasSearchCriteria({ ...base, keyword: "" })).toBe(false);
    expect(hasSearchCriteria({ ...base, keyword: "dev" })).toBe(true);
    expect(hasSearchCriteria({ ...base, keyword: "", where: "London" })).toBe(true);
  });

  it("normalises invalid page values to defaults", () => {
    expect(normalizeSearchCriteria({ ...base, keyword: "", pageIndex: -1, pageSize: 999 }))
      .toMatchObject({ pageIndex: 0, pageSize: 20 });
  });

  it("normalises non-positive datePosted to null", () => {
    expect(normalizeSearchCriteria({ ...base, keyword: "", datePosted: 0 }))
      .toMatchObject({ datePosted: null });
    expect(normalizeSearchCriteria({ ...base, keyword: "", datePosted: -3 }))
      .toMatchObject({ datePosted: null });
  });

  it("normalises unknown sortBy to date", () => {
    expect(normalizeSearchCriteria({ ...base, keyword: "", sortBy: "unknown" as never }))
      .toMatchObject({ sortBy: "date" });
  });
});
