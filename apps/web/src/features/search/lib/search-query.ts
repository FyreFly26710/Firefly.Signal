import type { SearchCriteria } from "@/features/search/types/search.types";

export function normalizeSearchCriteria(criteria: SearchCriteria): SearchCriteria {
  return {
    keyword: criteria.keyword.trim(),
    postcode: criteria.postcode.trim()
  };
}

export function readSearchCriteria(searchParams: URLSearchParams): SearchCriteria {
  return normalizeSearchCriteria({
    keyword: searchParams.get("keyword") ?? "",
    postcode: searchParams.get("postcode") ?? ""
  });
}

export function createSearchParams(criteria: SearchCriteria): URLSearchParams {
  const normalized = normalizeSearchCriteria(criteria);
  const params = new URLSearchParams();

  if (normalized.keyword) {
    params.set("keyword", normalized.keyword);
  }

  if (normalized.postcode) {
    params.set("postcode", normalized.postcode);
  }

  return params;
}

export function createSearchPath(criteria: SearchCriteria): string {
  const params = createSearchParams(criteria);
  const query = params.toString();

  return query ? `/search?${query}` : "/search";
}

export function hasSearchCriteria(criteria: SearchCriteria): boolean {
  return criteria.keyword.length > 0 || criteria.postcode.length > 0;
}
