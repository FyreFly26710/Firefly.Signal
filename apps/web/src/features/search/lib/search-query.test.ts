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
        company: "  Acme Corp  ",
        sortBy: "date-desc",
        pageIndex: 2,
        pageSize: 50
      })
    ).toEqual({
      keyword: "frontend engineer",
      postcode: "SW1A 1AA",
      company: "Acme Corp",
      sortBy: "date-desc",
      pageIndex: 2,
      pageSize: 50
    });
  });

  it("reads and trims criteria from URL search params", () => {
    const params = new URLSearchParams({
      keyword: "  designer ",
      postcode: " EC2A ",
      company: " Google ",
      sortBy: "date-asc",
      pageIndex: "3",
      pageSize: "100"
    });

    expect(readSearchCriteria(params)).toEqual({
      keyword: "designer",
      postcode: "EC2A",
      company: "Google",
      sortBy: "date-asc",
      pageIndex: 3,
      pageSize: 100
    });
  });

  it("creates search params without blank values", () => {
    expect(
      createSearchParams({
        keyword: "  product manager  ",
        postcode: "   ",
        company: "",
        sortBy: "date-desc",
        pageIndex: 0,
        pageSize: 50
      }).toString()
    ).toBe("keyword=product+manager&pageSize=50");
  });

  it("includes company and non-default sortBy in search params", () => {
    expect(
      createSearchParams({
        keyword: "engineer",
        postcode: "",
        company: "Acme",
        sortBy: "salary-desc",
        pageIndex: 0,
        pageSize: 20
      }).toString()
    ).toBe("keyword=engineer&company=Acme&sortBy=salary-desc");
  });

  it("builds the search path from normalized criteria", () => {
    expect(
      createSearchPath({
        keyword: "  data scientist ",
        postcode: " E1 ",
        company: "",
        sortBy: "date-desc",
        pageIndex: 2,
        pageSize: 100
      })
    ).toBe("/search?keyword=data+scientist&postcode=E1&pageIndex=2&pageSize=100");
  });

  it("detects whether any search criteria are present", () => {
    expect(hasSearchCriteria({ keyword: "", postcode: "", company: "", sortBy: "date-desc", pageIndex: 0, pageSize: 20 })).toBe(false);
    expect(hasSearchCriteria({ keyword: "", postcode: "SE1", company: "", sortBy: "date-desc", pageIndex: 0, pageSize: 20 })).toBe(true);
    expect(hasSearchCriteria({ keyword: "", postcode: "", company: "Acme", sortBy: "date-desc", pageIndex: 0, pageSize: 20 })).toBe(true);
  });

  it("normalizes invalid paging values back to defaults", () => {
    expect(
      normalizeSearchCriteria({
        keyword: "",
        postcode: "",
        company: "",
        sortBy: "date-desc",
        pageIndex: -3,
        pageSize: 999
      })
    ).toEqual({
      keyword: "",
      postcode: "",
      company: "",
      sortBy: "date-desc",
      pageIndex: 0,
      pageSize: 20
    });
  });

  it("normalizes unknown sortBy values to default", () => {
    expect(
      normalizeSearchCriteria({
        keyword: "",
        postcode: "",
        company: "",
        sortBy: "unknown" as never,
        pageIndex: 0,
        pageSize: 20
      })
    ).toEqual({
      keyword: "",
      postcode: "",
      company: "",
      sortBy: "date-desc",
      pageIndex: 0,
      pageSize: 20
    });
  });
});
