import { describe, expect, it } from "vitest";
import {
  createSearchParams,
  createSearchPath,
  hasSearchCriteria,
  normalizeSearchCriteria,
  readSearchCriteria
} from "@/features/search/lib/search-query";

describe("search-query", () => {
  it("normalizes whitespace around search criteria", () => {
    expect(normalizeSearchCriteria({ keyword: "  designer  ", postcode: "  EC2A " })).toEqual({
      keyword: "designer",
      postcode: "EC2A"
    });
  });

  it("reads search criteria from URL params", () => {
    const result = readSearchCriteria(new URLSearchParams("keyword=Product%20Designer&postcode=SW1A"));

    expect(result).toEqual({
      keyword: "Product Designer",
      postcode: "SW1A"
    });
  });

  it("creates search params without empty values", () => {
    expect(createSearchParams({ keyword: "designer", postcode: "" }).toString()).toBe("keyword=designer");
  });

  it("creates a search path for navigation", () => {
    expect(createSearchPath({ keyword: "designer", postcode: "EC2A" })).toBe(
      "/search?keyword=designer&postcode=EC2A"
    );
  });

  it("detects whether a search is present", () => {
    expect(hasSearchCriteria({ keyword: "", postcode: "" })).toBe(false);
    expect(hasSearchCriteria({ keyword: "", postcode: "EC2A" })).toBe(true);
  });
});
