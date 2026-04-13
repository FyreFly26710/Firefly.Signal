import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { ThemeProvider } from "@mui/material";
import { MemoryRouter } from "react-router-dom";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { theme } from "@/app/theme";
import {
  catalogHideJobs,
  exportJobs,
  getJobsPage,
  importJobsFromJson,
  importJobsFromProvider
} from "@/api/jobs/jobs.api";
import { JobsListView } from "@/features/jobs/views/JobsListView";
import { useSessionStore } from "@/store/session.store";

vi.mock("@/api/jobs/jobs.api", () => ({
  catalogHideJobs: vi.fn(),
  deleteJobs: vi.fn(),
  exportJobs: vi.fn(),
  getJobsPage: vi.fn(),
  importJobsFromJson: vi.fn(),
  importJobsFromProvider: vi.fn()
}));

describe("JobsListView", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.stubGlobal("URL", {
      createObjectURL: vi.fn(() => "blob:jobs-export"),
      revokeObjectURL: vi.fn()
    });
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

    vi.mocked(catalogHideJobs).mockResolvedValueOnce({
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
    expect(catalogHideJobs).toHaveBeenCalledWith([42]);
    expect(getJobsPage).toHaveBeenCalledWith(
      expect.objectContaining({
        pageIndex: 0,
        pageSize: 20,
        isHidden: false
      })
    );
  }, 10000);

  it("supports provider import, json import, and json export for admin users", async () => {
    const user = userEvent.setup();
    const clickSpy = vi.spyOn(HTMLAnchorElement.prototype, "click").mockImplementation(() => undefined);

    vi.mocked(getJobsPage).mockResolvedValue({
      pageIndex: 0,
      pageSize: 20,
      totalCount: 1,
      items: [
        {
          id: 99,
          jobRefreshRunId: null,
          sourceName: "Adzuna",
          sourceJobId: "adz-99",
          sourceAdReference: null,
          title: "Export Test Job",
          description: "desc",
          summary: "summary",
          url: "https://example.com/jobs/99",
          company: "Acme",
          companyDisplayName: null,
          companyCanonicalName: null,
          postcode: "EC1A 1BB",
          locationName: "London",
          locationDisplayName: null,
          locationAreaJson: null,
          latitude: null,
          longitude: null,
          categoryTag: null,
          categoryLabel: null,
          salaryMin: null,
          salaryMax: null,
          salaryCurrency: null,
          salaryIsPredicted: null,
          contractTime: null,
          contractType: null,
          isFullTime: true,
          isPartTime: false,
          isPermanent: true,
          isContract: false,
          isRemote: false,
          postedAtUtc: "2025-04-03T10:00:00Z",
          importedAtUtc: "2025-04-03T12:00:00Z",
          lastSeenAtUtc: "2025-04-03T12:00:00Z",
          isHidden: false,
          rawPayloadJson: "{}"
        }
      ]
    });

    vi.mocked(importJobsFromProvider).mockResolvedValue({
      jobRefreshRunId: 1,
      source: "Adzuna",
      importedCount: 2,
      failedCount: 0
    });

    vi.mocked(importJobsFromJson).mockResolvedValue({
      jobRefreshRunId: 2,
      source: "json-upload",
      importedCount: 1,
      failedCount: 0
    });

    vi.mocked(exportJobs).mockResolvedValue({
      exportedAtUtc: "2026-04-11T10:00:00Z",
      count: 1,
      jobs: []
    });

    render(
      <ThemeProvider theme={theme}>
        <MemoryRouter initialEntries={["/admin/manage-jobs"]}>
          <JobsListView />
        </MemoryRouter>
      </ThemeProvider>
    );

    await waitFor(() => {
      expect(screen.getByRole("heading", { name: "Manage jobs" })).toBeInTheDocument();
    });

    await user.click(screen.getByRole("button", { name: "Import from provider" }));
    await user.type(screen.getByRole("textbox", { name: "Keyword" }), "frontend engineer");
    await user.type(screen.getByRole("textbox", { name: "Postcode" }), "SW1A 1AA");
    await user.click(screen.getByRole("button", { name: "Run import" }));

    await waitFor(() => {
      expect(importJobsFromProvider).toHaveBeenCalledWith(
        expect.objectContaining({
          keyword: "frontend engineer",
          postcode: "SW1A 1AA",
          pageIndex: 0,
          pageSize: 20,
          provider: "Adzuna"
        })
      );
    });

    const file = new File(
      [JSON.stringify({ exportedAtUtc: "2026-04-11T10:00:00Z", count: 1, jobs: [] })],
      "jobs.json",
      { type: "application/json" }
    );

    const fileInput = document.querySelector<HTMLInputElement>('input[type="file"]')!;
    await user.upload(fileInput, file);

    await waitFor(() => {
      expect(importJobsFromJson).toHaveBeenCalledWith(file);
    });

    await user.click(screen.getByRole("checkbox", { name: "Select job 99" }));
    await user.click(screen.getByRole("button", { name: "Export JSON" }));

    await waitFor(() => {
      expect(exportJobs).toHaveBeenCalledWith({ jobIds: [99] });
      expect(clickSpy).toHaveBeenCalled();
    });

    clickSpy.mockRestore();
  }, 30000);
});
