import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { ThemeProvider } from "@mui/material";
import { MemoryRouter } from "react-router-dom";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { theme } from "@/app/theme";
import { getJobsPage, hideJobs } from "@/api/jobs/jobs.api";
import { JobsListView } from "@/features/jobs/views/JobsListView";
import { useSessionStore } from "@/store/session.store";

vi.mock("@/api/jobs/jobs.api", () => ({
  deleteJobs: vi.fn(),
  getJobsPage: vi.fn(),
  hideJobs: vi.fn()
}));

describe("JobsListView", () => {
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

  it("loads jobs into the admin table and can hide selected rows", async () => {
    const user = userEvent.setup();

    vi.mocked(getJobsPage).mockResolvedValue({
      pageIndex: 0,
      pageSize: 20,
      totalCount: 1,
      items: [
        {
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
        }
      ]
    });

    render(
      <ThemeProvider theme={theme}>
        <MemoryRouter initialEntries={["/admin/manage-jobs"]}>
          <JobsListView />
        </MemoryRouter>
      </ThemeProvider>
    );

    await waitFor(() => {
      expect(screen.getByText("Senior Frontend Engineer")).toBeInTheDocument();
    });

    vi.mocked(hideJobs).mockResolvedValueOnce({
      hiddenCount: 1,
      hiddenIds: [42],
      missingIds: []
    });

    await user.click(screen.getByRole("checkbox", { name: "Select job 42" }));
    await user.click(screen.getByRole("button", { name: "Hide selected" }));

    await waitFor(() => {
      expect(screen.getByText("1 jobs hidden successfully.")).toBeInTheDocument();
    });

    expect(screen.getByText(/total jobs/i)).toBeInTheDocument();
    expect(screen.getByRole("heading", { name: "Manage jobs" })).toBeInTheDocument();
    expect(hideJobs).toHaveBeenCalledWith([42]);
    expect(getJobsPage).toHaveBeenCalledWith(
      expect.objectContaining({
        pageIndex: 0,
        pageSize: 20,
        isHidden: false
      })
    );
  });
});
