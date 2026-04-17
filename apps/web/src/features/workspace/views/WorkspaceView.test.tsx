import { screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { getAppliedJobs } from "@/api/applications/applications.api";
import { WorkspaceView } from "@/features/workspace/views/WorkspaceView";
import { renderWithProviders } from "@/test/render";

vi.mock("@/api/applications/applications.api", () => ({
  getAppliedJobs: vi.fn().mockResolvedValue([]),
  getApplicationDetail: vi.fn()
}));

describe("WorkspaceView", () => {
  it("renders the applied jobs section as the first workspace section", async () => {
    renderWithProviders(<WorkspaceView />);

    expect(screen.getByRole("heading", { name: "Your workspace" })).toBeInTheDocument();
    await screen.findByRole("heading", { name: "Applied jobs" });
    await waitFor(() => {
      expect(screen.getByText(/No applied jobs yet/)).toBeInTheDocument();
    }, { timeout: 3000 });
    expect(getAppliedJobs).toHaveBeenCalled();
  });
});
