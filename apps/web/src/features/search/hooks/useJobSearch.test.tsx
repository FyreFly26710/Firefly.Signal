import { waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { searchJobsPage } from "@/api/jobs/jobs.api";
import { useJobSearch } from "@/features/search/hooks/useJobSearch";
import { renderHookWithProviders } from "@/test/render";

vi.mock("@/api/jobs/jobs.api", () => ({
  searchJobsPage: vi.fn()
}));

const baseCriteria = {
  keyword: "",
  where: "",
  salaryMin: null,
  salaryMax: null,
  datePosted: null,
  sortBy: "date" as const,
  isAsc: false,
  pageIndex: 0,
  pageSize: 20
};

function createDeferred<T>() {
  let resolve!: (value: T) => void;
  let reject!: (reason?: unknown) => void;

  const promise = new Promise<T>((nextResolve, nextReject) => {
    resolve = nextResolve;
    reject = nextReject;
  });

  return { promise, resolve, reject };
}

describe("useJobSearch", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("loads visible jobs even when no criteria are provided", async () => {
    vi.mocked(searchJobsPage).mockResolvedValueOnce({
      pageIndex: 0,
      pageSize: 20,
      totalCount: 0,
      items: []
    });

    const { result } = renderHookWithProviders(() =>
      useJobSearch(baseCriteria)
    );

    await waitFor(() => {
      expect(result.current.status).toBe("empty");
    });

    expect(searchJobsPage).toHaveBeenCalledWith({
      pageIndex: 0,
      pageSize: 20,
      keyword: undefined,
      where: undefined,
      salaryMin: undefined,
      salaryMax: undefined,
      datePosted: undefined,
      sortBy: "date",
      isAsc: false
    });
  });

  it("loads and returns mapped results for a populated search", async () => {
    const deferred = createDeferred<Awaited<ReturnType<typeof searchJobsPage>>>();
    vi.mocked(searchJobsPage).mockReturnValueOnce(deferred.promise);

    const { result } = renderHookWithProviders(() =>
      useJobSearch({
        ...baseCriteria,
        keyword: "designer",
        where: "London",
        salaryMin: 50000,
        salaryMax: 90000,
        datePosted: 7,
        sortBy: "salary",
        isAsc: true,
        pageIndex: 1,
        pageSize: 50
      })
    );

    await waitFor(() => {
      expect(result.current.status).toBe("loading");
    });

    deferred.resolve({
      pageIndex: 0,
      pageSize: 50,
      totalCount: 1,
      items: [
        {
          id: 1,
          sourceJobId: "adz-1",
          title: "Product Designer",
          summary: "Shape the MVP search flow.",
          url: "https://example.com/jobs/1",
          company: "Firefly",
          companyDisplayName: "Firefly",
          locationName: "London",
          locationDisplayName: "London",
          isRemote: false,
          isHidden: false,
          salaryMin: 70000,
          salaryMax: 90000,
          salaryCurrency: "GBP",
          contractTime: "full_time",
          contractType: "permanent",
          isFullTime: true,
          isPartTime: false,
          isPermanent: true,
          isContract: false,
          sourceName: "Reed",
          postedAtUtc: "2025-01-02T09:00:00.000Z",
          isSaved: false,
          isUserHidden: false,
          isApplied: false
        }
      ]
    });

    await waitFor(() => {
      expect(result.current.status).toBe("success");
    });

    expect(result.current.data?.jobs[0]).toMatchObject({
      id: "1",
      employer: "Firefly",
      location: "London",
      salary: "\u00a370,000 - \u00a390,000"
    });
    expect(result.current.data).toMatchObject({
      pageIndex: 0,
      pageSize: 50
    });
    expect(result.current.errorMessage).toBeNull();
    expect(searchJobsPage).toHaveBeenCalledWith({
      pageIndex: 1,
      pageSize: 50,
      keyword: "designer",
      where: "London",
      salaryMin: 50000,
      salaryMax: 90000,
      datePosted: 7,
      sortBy: "salary",
      isAsc: true
    });
  });

  it("returns the empty state when the search succeeds without jobs", async () => {
    vi.mocked(searchJobsPage).mockResolvedValueOnce({
      pageIndex: 0,
      pageSize: 20,
      totalCount: 0,
      items: []
    });

    const { result } = renderHookWithProviders(() =>
      useJobSearch({ ...baseCriteria, keyword: "analyst" })
    );

    await waitFor(() => {
      expect(result.current.status).toBe("empty");
    });

    expect(result.current.data?.jobs).toEqual([]);
  });

  it("surfaces API failures as an error state", async () => {
    vi.mocked(searchJobsPage).mockRejectedValueOnce(new Error("Search service is unavailable."));

    const { result } = renderHookWithProviders(() =>
      useJobSearch({ ...baseCriteria, keyword: "engineer" })
    );

    await waitFor(() => {
      expect(result.current.status).toBe("error");
    });

    expect(result.current.errorMessage).toBe("Search service is unavailable.");
  });
});
