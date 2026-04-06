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
        postcode: "  SW1A 1AA  "
      })
    ).toEqual({
      keyword: "frontend engineer",
      postcode: "SW1A 1AA"
    });
  });

  it("reads and trims criteria from URL search params", () => {
    const params = new URLSearchParams({
      keyword: "  designer ",
      postcode: " EC2A "
    });

    expect(readSearchCriteria(params)).toEqual({
      keyword: "designer",
      postcode: "EC2A"
    });
  });

  it("creates search params without blank values", () => {
    expect(
      createSearchParams({
        keyword: "  product manager  ",
        postcode: "   "
      }).toString()
    ).toBe("keyword=product+manager");
  });

  it("builds the search path from normalized criteria", () => {
    expect(
      createSearchPath({
        keyword: "  data scientist ",
        postcode: " E1 "
      })
    ).toBe("/search?keyword=data+scientist&postcode=E1");
  });

  it("detects whether any search criteria are present", () => {
    expect(hasSearchCriteria({ keyword: "", postcode: "" })).toBe(false);
    expect(hasSearchCriteria({ keyword: "", postcode: "SE1" })).toBe(true);
  });
});
