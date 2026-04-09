import { renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { getJobsPage } from "@/api/jobs/jobs.api";
import { useJobSearch } from "@/features/search/hooks/useJobSearch";

vi.mock("@/api/jobs/jobs.api", () => ({
  getJobsPage: vi.fn()
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
    expect(getJobsPage).not.toHaveBeenCalled();
  });

  it("loads and returns mapped results for a populated search", async () => {
    const deferred = createDeferred<Awaited<ReturnType<typeof getJobsPage>>>();
    vi.mocked(getJobsPage).mockReturnValueOnce(deferred.promise);

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
      pageIndex: 0,
      pageSize: 20,
      totalCount: 1,
      items: [
        {
          id: 1,
          jobRefreshRunId: null,
          sourceJobId: "adz-1",
          sourceAdReference: null,
          title: "Product Designer",
          description: "Shape the MVP search flow.",
          company: "Firefly",
          companyDisplayName: "Firefly",
          companyCanonicalName: "firefly",
          postcode: "EC2A",
          locationName: "London",
          locationDisplayName: "London",
          locationAreaJson: null,
          latitude: null,
          longitude: null,
          categoryTag: "design",
          categoryLabel: "Design",
          summary: "Shape the MVP search flow.",
          url: "https://example.com/jobs/1",
          sourceName: "Reed",
          salaryMin: 70000,
          salaryMax: 90000,
          salaryCurrency: "GBP",
          salaryIsPredicted: false,
          contractTime: "full_time",
          contractType: "permanent",
          isFullTime: true,
          isPartTime: false,
          isPermanent: true,
          isContract: false,
          isRemote: false,
          postedAtUtc: "2025-01-02T09:00:00.000Z",
          importedAtUtc: "2025-01-02T10:00:00.000Z",
          lastSeenAtUtc: "2025-01-02T11:00:00.000Z",
          isHidden: false,
          rawPayloadJson: "{}"
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
    expect(result.current.errorMessage).toBeNull();
    expect(getJobsPage).toHaveBeenCalledWith({
      pageIndex: 0,
      pageSize: 20,
      postcode: "EC2A",
      keyword: "designer",
      isHidden: false
    });
  });

  it("returns the empty state when the search succeeds without jobs", async () => {
    vi.mocked(getJobsPage).mockResolvedValueOnce({
      pageIndex: 0,
      pageSize: 20,
      totalCount: 0,
      items: []
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
    vi.mocked(getJobsPage).mockRejectedValueOnce(new Error("Search service is unavailable."));

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
