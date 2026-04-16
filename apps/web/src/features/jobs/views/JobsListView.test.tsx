import { screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import {
  catalogHideJobs,
  exportJobs,
  getJobsPage,
  importJobsFromJson,
  importJobsFromProvider
} from "@/api/jobs/jobs.api";
import { JobsListView } from "@/features/jobs/views/JobsListView";
import { useSessionStore } from "@/features/auth/store/session.store";
import { renderWithProviders } from "@/test/render";

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
          sourceJobId: "adz-42",
          title: "Senior Frontend Engineer",
          summary: "Build the web application.",
          url: "https://example.com/jobs/42",
          company: "Firefly",
          companyDisplayName: "Firefly",
          locationName: "London",
          locationDisplayName: "London",
          isRemote: true,
          isHidden: false,
          salaryMin: 75000,
          salaryMax: 95000,
          salaryCurrency: "GBP",
          contractTime: "full_time",
          contractType: "permanent",
          isFullTime: true,
          isPartTime: false,
          isPermanent: true,
          isContract: false,
          sourceName: "Adzuna",
          postedAtUtc: "2025-04-03T10:00:00Z",
          isSaved: false,
          isUserHidden: false
        }
      ]
    });

    renderWithProviders(<JobsListView />, { route: "/admin/manage-jobs" });

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
  }, 20000);

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
          sourceJobId: "adz-99",
          title: "Export Test Job",
          summary: "summary",
          url: "https://example.com/jobs/99",
          company: "Acme",
          companyDisplayName: null,
          locationName: "London",
          locationDisplayName: null,
          isRemote: false,
          isHidden: false,
          salaryMin: null,
          salaryMax: null,
          salaryCurrency: null,
          contractTime: null,
          contractType: null,
          isFullTime: true,
          isPartTime: false,
          isPermanent: true,
          isContract: false,
          sourceName: "Adzuna",
          postedAtUtc: "2025-04-03T10:00:00Z",
          isSaved: false,
          isUserHidden: false
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

    renderWithProviders(<JobsListView />, { route: "/admin/manage-jobs" });

    await waitFor(() => {
      expect(screen.getByRole("heading", { name: "Manage jobs" })).toBeInTheDocument();
    });

    await user.click(screen.getByRole("button", { name: "Import from provider" }));
    await user.type(screen.getByRole("textbox", { name: "Keyword" }), "frontend engineer");
    const whereInput = screen.getByRole("textbox", { name: "Where" });
    await user.clear(whereInput);
    await user.type(whereInput, "SW1A 1AA");
    await user.click(screen.getByRole("button", { name: "Run import" }));

    await waitFor(() => {
      expect(importJobsFromProvider).toHaveBeenCalledWith(
        expect.objectContaining({
          where: "SW1A 1AA",
          keyword: "frontend engineer",
          pageIndex: 1,
          pageSize: 50,
          distanceKilometers: 5,
          maxDaysOld: 30,
          category: "it-jobs",
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

    // Wait for the list to reload after import before interacting with the table
    await screen.findByRole("checkbox", { name: "Select job 99" });

    await user.click(screen.getByRole("checkbox", { name: "Select job 99" }));
    await user.click(screen.getByRole("button", { name: "Export JSON" }));

    await waitFor(() => {
      expect(exportJobs).toHaveBeenCalledWith({ jobIds: [99] });
      expect(clickSpy).toHaveBeenCalled();
    });

    clickSpy.mockRestore();
  }, 30000);
});
