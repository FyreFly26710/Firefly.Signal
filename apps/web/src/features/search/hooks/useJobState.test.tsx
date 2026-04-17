import { act } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { applyToJob } from "@/api/applications/applications.api";
import { useJobState } from "@/features/search/hooks/useJobState";
import { renderHookWithProviders } from "@/test/render";

vi.mock("@/api/applications/applications.api", () => ({
  applyToJob: vi.fn()
}));

describe("useJobState", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("marks a job as applied after a successful apply request", async () => {
    vi.mocked(applyToJob).mockResolvedValueOnce({
      id: 10,
      jobPostingId: 42,
      note: null,
      currentStatus: "Applied",
      appliedAtUtc: "2026-04-10T08:30:00Z",
      latestStatusAtUtc: "2026-04-10T08:30:00Z",
      statusHistory: []
    });

    const { result } = renderHookWithProviders(() =>
      useJobState("42", { isApplied: false, isSaved: false, isHidden: false })
    );

    await act(async () => {
      await result.current.apply();
    });

    expect(result.current.isApplied).toBe(true);
    expect(applyToJob).toHaveBeenCalledWith(42);
  });
});
