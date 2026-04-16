import { screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { catalogHideJob, deleteJob, getJobById } from "@/api/jobs/jobs.api";
import { ManageJobView } from "@/features/jobs/views/ManageJobView";
import { useSessionStore } from "@/features/auth/store/session.store";
import { renderWithProviders } from "@/test/render";

vi.mock("@/api/jobs/jobs.api", () => ({
  catalogHideJob: vi.fn(),
  createJob: vi.fn(),
  deleteJob: vi.fn(),
  getJobById: vi.fn(),
  updateJob: vi.fn()
}));

describe("ManageJobView", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    useSessionStore.setState({
      user: {
        userAccount: "admin",
        displayName: "Admin",
        role: "admin",
        email: "admin@example.com"
      },
      isAuthenticated: true
    });
  });

  it("surfaces the delete conflict and lets the user hide the job", async () => {
    const user = userEvent.setup();
    const confirmSpy = vi.spyOn(window, "confirm").mockReturnValue(true);

    vi.mocked(getJobById).mockResolvedValueOnce({
      id: 42,
      jobRefreshRunId: null,
      sourceName: "Adzuna",
      sourceJobId: "adz-42",
      sourceAdReference: null,
      title: "Senior Frontend Engineer",
      description: "Build the web application.",
      summary: "Build the web application.",
      url: "https://example.com/jobs/42",
      company: "Firefly",
      companyDisplayName: "Firefly",
      companyCanonicalName: "firefly",
      postcode: "EC2A 4NE",
      locationName: "London",
      locationDisplayName: "London",
      locationAreaJson: "[\"London\"]",
      latitude: null,
      longitude: null,
      categoryTag: "it-jobs",
      categoryLabel: "IT jobs",
      salaryMin: 75000,
      salaryMax: 95000,
      salaryCurrency: "GBP",
      salaryIsPredicted: false,
      contractTime: "full_time",
      contractType: "permanent",
      isFullTime: true,
      isPartTime: false,
      isPermanent: true,
      isContract: false,
      isRemote: true,
      postedAtUtc: "2025-04-03T10:00:00Z",
      importedAtUtc: "2025-04-03T12:00:00Z",
      lastSeenAtUtc: "2025-04-03T12:00:00Z",
      isHidden: false,
      rawPayloadJson: "{}"
    });
    vi.mocked(deleteJob).mockRejectedValueOnce(
      new Error("Job 42 must be hidden before it can be deleted.")
    );
    vi.mocked(catalogHideJob).mockResolvedValueOnce({
      hiddenCount: 1,
      hiddenIds: [42],
      missingIds: []
    });

    renderWithProviders(<ManageJobView jobId="42" />);

    expect(await screen.findByDisplayValue("Senior Frontend Engineer")).toBeInTheDocument();

    await user.click(screen.getByRole("button", { name: "Delete job" }));

    await waitFor(() => {
      expect(
        screen.getByText("Job 42 must be hidden before it can be deleted.")
      ).toBeInTheDocument();
    });

    await user.click(screen.getByRole("button", { name: "Hide job" }));

    await waitFor(() => {
      expect(screen.getByText("Job hidden successfully.")).toBeInTheDocument();
    });

    expect(confirmSpy).toHaveBeenCalled();
    expect(deleteJob).toHaveBeenCalledWith(42);
    expect(catalogHideJob).toHaveBeenCalledWith(42);

    confirmSpy.mockRestore();
  }, 20000);
});
