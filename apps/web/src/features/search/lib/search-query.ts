import type { DatePosted, SearchCriteria, SearchSortBy } from "@/features/search/types/search.types";

const defaultPageIndex = 0;
const defaultPageSize = 20;
const defaultSortBy: SearchSortBy = "date-desc";
const defaultDatePosted: DatePosted = "anytime";
const allowedPageSizes = new Set([20, 50, 100]);
const allowedSortBy = new Set<SearchSortBy>(["date-desc", "date-asc", "salary-desc", "salary-asc"]);
const allowedDatePosted = new Set<DatePosted>(["anytime", "today", "3days", "1week", "2weeks"]);

export function normalizeSearchCriteria(criteria: SearchCriteria): SearchCriteria {
  return {
    keyword: criteria.keyword.trim(),
    postcode: criteria.postcode.trim(),
    salaryMin: normalizeSalary(criteria.salaryMin),
    salaryMax: normalizeSalary(criteria.salaryMax),
    datePosted: normalizeDatePosted(criteria.datePosted),
    sortBy: normalizeSortBy(criteria.sortBy),
    pageIndex: normalizePageIndex(criteria.pageIndex),
    pageSize: normalizePageSize(criteria.pageSize)
  };
}

export function readSearchCriteria(searchParams: URLSearchParams): SearchCriteria {
  const rawMin = searchParams.get("salaryMin");
  const rawMax = searchParams.get("salaryMax");

  return normalizeSearchCriteria({
    keyword: searchParams.get("keyword") ?? "",
    postcode: searchParams.get("postcode") ?? "",
    salaryMin: rawMin !== null ? Number(rawMin) : null,
    salaryMax: rawMax !== null ? Number(rawMax) : null,
    datePosted: (searchParams.get("datePosted") ?? defaultDatePosted) as DatePosted,
    sortBy: (searchParams.get("sortBy") ?? defaultSortBy) as SearchSortBy,
    pageIndex: Number(searchParams.get("pageIndex") ?? defaultPageIndex),
    pageSize: Number(searchParams.get("pageSize") ?? defaultPageSize)
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

  if (normalized.salaryMin !== null) {
    params.set("salaryMin", String(normalized.salaryMin));
  }

  if (normalized.salaryMax !== null) {
    params.set("salaryMax", String(normalized.salaryMax));
  }

  if (normalized.datePosted !== defaultDatePosted) {
    params.set("datePosted", normalized.datePosted);
  }

  if (normalized.sortBy !== defaultSortBy) {
    params.set("sortBy", normalized.sortBy);
  }

  if (normalized.pageIndex > 0) {
    params.set("pageIndex", String(normalized.pageIndex));
  }

  if (normalized.pageSize !== defaultPageSize) {
    params.set("pageSize", String(normalized.pageSize));
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

function normalizeSortBy(value: SearchSortBy): SearchSortBy {
  return allowedSortBy.has(value) ? value : defaultSortBy;
}

function normalizeDatePosted(value: DatePosted): DatePosted {
  return allowedDatePosted.has(value) ? value : defaultDatePosted;
}

function normalizeSalary(value: number | null): number | null {
  if (value === null) return null;
  const n = Math.floor(value);
  return Number.isFinite(n) && n >= 0 ? n : null;
}

function normalizePageIndex(value: number): number {
  return Number.isFinite(value) && value >= 0 ? Math.floor(value) : defaultPageIndex;
}

function normalizePageSize(value: number): number {
  const normalizedValue = Number.isFinite(value) ? Math.floor(value) : defaultPageSize;
  return allowedPageSizes.has(normalizedValue) ? normalizedValue : defaultPageSize;
}
