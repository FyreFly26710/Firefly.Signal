import type { SearchCriteria, SearchSortBy } from "@/features/search/types/search.types";

const defaultPageIndex = 0;
const defaultPageSize = 20;
const defaultSortBy: SearchSortBy = "date";
const defaultIsAsc = false;
const allowedPageSizes = new Set([20, 50, 100]);
const allowedSortBy = new Set<SearchSortBy>(["date", "salary"]);

export function normalizeSearchCriteria(criteria: SearchCriteria): SearchCriteria {
  return {
    keyword: criteria.keyword.trim(),
    where: criteria.where.trim(),
    salaryMin: normalizeSalary(criteria.salaryMin),
    salaryMax: normalizeSalary(criteria.salaryMax),
    datePosted: normalizeDatePosted(criteria.datePosted),
    sortBy: normalizeSortBy(criteria.sortBy),
    isAsc: Boolean(criteria.isAsc),
    pageIndex: normalizePageIndex(criteria.pageIndex),
    pageSize: normalizePageSize(criteria.pageSize)
  };
}

export function readSearchCriteria(searchParams: URLSearchParams): SearchCriteria {
  const rawMin = searchParams.get("salaryMin");
  const rawMax = searchParams.get("salaryMax");
  const rawDatePosted = searchParams.get("datePosted");

  return normalizeSearchCriteria({
    keyword: searchParams.get("keyword") ?? "",
    where: searchParams.get("where") ?? "",
    salaryMin: rawMin !== null ? Number(rawMin) : null,
    salaryMax: rawMax !== null ? Number(rawMax) : null,
    datePosted: rawDatePosted !== null ? Number(rawDatePosted) : null,
    sortBy: (searchParams.get("sortBy") ?? defaultSortBy) as SearchSortBy,
    isAsc: searchParams.get("isAsc") === "true",
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

  if (normalized.where) {
    params.set("where", normalized.where);
  }

  if (normalized.salaryMin !== null) {
    params.set("salaryMin", String(normalized.salaryMin));
  }

  if (normalized.salaryMax !== null) {
    params.set("salaryMax", String(normalized.salaryMax));
  }

  if (normalized.datePosted !== null) {
    params.set("datePosted", String(normalized.datePosted));
  }

  if (normalized.sortBy !== defaultSortBy) {
    params.set("sortBy", normalized.sortBy);
  }

  if (normalized.isAsc !== defaultIsAsc) {
    params.set("isAsc", "true");
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
  return criteria.keyword.length > 0 || criteria.where.length > 0;
}

function normalizeSortBy(value: SearchSortBy): SearchSortBy {
  return allowedSortBy.has(value) ? value : defaultSortBy;
}

function normalizeDatePosted(value: number | null): number | null {
  if (value === null) return null;
  const n = Math.floor(value);
  return Number.isFinite(n) && n > 0 ? n : null;
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
