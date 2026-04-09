import { renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { searchJobs } from "@/api/job-search/job-search.api";
import { useJobSearch } from "@/features/search/hooks/useJobSearch";

vi.mock("@/api/job-search/job-search.api", () => ({
  searchJobs: vi.fn()
}));

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

  it("stays idle when no criteria are provided", () => {
    const { result } = renderHook(() => useJobSearch({ keyword: "", postcode: "" }));

    expect(result.current).toEqual({
      status: "idle",
      data: null,
      errorMessage: null
    });
    expect(searchJobs).not.toHaveBeenCalled();
  });

  it("loads and returns mapped results for a populated search", async () => {
    const deferred = createDeferred<Awaited<ReturnType<typeof searchJobs>>>();
    vi.mocked(searchJobs).mockReturnValueOnce(deferred.promise);

    const { result } = renderHook(() =>
      useJobSearch({
        keyword: "designer",
        postcode: "EC2A"
      })
    );

    await waitFor(() => {
      expect(result.current.status).toBe("loading");
    });

    deferred.resolve({
      postcode: "EC2A",
      keyword: "designer",
      pageIndex: 0,
      pageSize: 20,
      totalCount: 1,
      jobs: [
        {
          id: 1,
          title: "Product Designer",
          company: "Firefly",
          locationName: "London",
          summary: "Shape the MVP search flow.",
          url: "https://example.com/jobs/1",
          sourceName: "Reed",
          isRemote: false,
          postedAtUtc: "2025-01-02T09:00:00.000Z"
        }
      ]
    });

    await waitFor(() => {
      expect(result.current.status).toBe("success");
    });

    expect(result.current.data?.jobs[0]).toMatchObject({
      id: "1",
      employer: "Firefly",
      location: "London"
    });
    expect(result.current.errorMessage).toBeNull();
    expect(searchJobs).toHaveBeenCalledWith("EC2A", "designer", "adzuna");
  });

  it("returns the empty state when the search succeeds without jobs", async () => {
    vi.mocked(searchJobs).mockResolvedValueOnce({
      postcode: "SE1",
      keyword: "analyst",
      pageIndex: 0,
      pageSize: 20,
      totalCount: 0,
      jobs: []
    });

    const { result } = renderHook(() =>
      useJobSearch({
        keyword: "analyst",
        postcode: "SE1"
      })
    );

    await waitFor(() => {
      expect(result.current.status).toBe("empty");
    });

    expect(result.current.data?.jobs).toEqual([]);
  });

  it("surfaces API failures as an error state", async () => {
    vi.mocked(searchJobs).mockRejectedValueOnce(new Error("Search service is unavailable."));

    const { result } = renderHook(() =>
      useJobSearch({
        keyword: "engineer",
        postcode: "M1"
      })
    );

    await waitFor(() => {
      expect(result.current.status).toBe("error");
    });

    expect(result.current.errorMessage).toBe("Search service is unavailable.");
  });
});
