import { screen, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { getJobById } from "@/api/jobs/jobs.api";
import { JobDetailView } from "@/features/jobs/views/JobDetailView";
import { ApiError } from "@/lib/http/api-error";
import { renderWithProviders } from "@/test/render";

vi.mock("@/api/jobs/jobs.api", () => ({
  getJobById: vi.fn()
}));

describe("JobDetailView", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders job details from the API", async () => {
    vi.mocked(getJobById).mockResolvedValueOnce({
      id: 42,
      jobRefreshRunId: null,
      sourceName: "Indeed",
      sourceJobId: "abc-42",
      sourceAdReference: null,
      title: "Senior Product Designer",
      description: "Lead product discovery.\n\nShape high-quality interfaces.",
      summary: "Design thoughtful user journeys.",
      url: "https://example.com/jobs/42",
      company: "Acme",
      companyDisplayName: "Acme Ltd",
      companyCanonicalName: null,
      postcode: "EC2A 4DP",
      locationName: "London",
      locationDisplayName: "London",
      locationAreaJson: null,
      latitude: null,
      longitude: null,
      categoryTag: null,
      categoryLabel: null,
      salaryMin: 70000,
      salaryMax: 90000,
      salaryCurrency: "GBP",
      salaryIsPredicted: null,
      contractTime: null,
      contractType: "full_time",
      isFullTime: true,
      isPartTime: false,
      isPermanent: true,
      isContract: false,
      isRemote: false,
      postedAtUtc: "2026-04-10T08:30:00Z",
      importedAtUtc: "2026-04-10T08:30:00Z",
      lastSeenAtUtc: "2026-04-10T08:30:00Z",
      isHidden: false,
      applicationId: null,
      isApplied: false,
      rawPayloadJson: "{}"
    });

    renderWithProviders(<JobDetailView jobId="42" />);

    expect(screen.getByText("Fetching the latest job details...")).toBeInTheDocument();

    await screen.findByRole("heading", { name: "Senior Product Designer" }, { timeout: 3000 });

    expect(screen.getByText("Lead product discovery.")).toBeInTheDocument();
    expect(screen.getByText("Shape high-quality interfaces.")).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Back to search results" })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Apply on Indeed" })).toBeInTheDocument();
  });

  it("shows the not-found state when the API returns 404", async () => {
    vi.mocked(getJobById).mockRejectedValueOnce(new ApiError("Not found", 404));

    renderWithProviders(<JobDetailView jobId="999" />);

    await screen.findByRole("heading", { name: "Job not found" });

    expect(
      screen.getByText("This listing is unavailable or may have been removed. Head back to the search results to continue browsing.")
    ).toBeInTheDocument();
  });

  it("shows an error state when the job request fails", async () => {
    vi.mocked(getJobById).mockRejectedValueOnce(new Error("Search service is unavailable."));

    renderWithProviders(<JobDetailView jobId="42" />);

    await waitFor(() => {
      expect(screen.getByText("Search service is unavailable.")).toBeInTheDocument();
    });
  });

  it("shows the not-found state for an invalid job id", () => {
    renderWithProviders(<JobDetailView jobId={undefined} />);

    expect(screen.getByRole("heading", { name: "Job not found" })).toBeInTheDocument();
  });
});
