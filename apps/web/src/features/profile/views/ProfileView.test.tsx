import { screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { getCurrentProfile, upsertCurrentProfile } from "@/api/profile/profile.api";
import type { UserProfileResponseDto } from "@/api/profile/profile.types";
import { ProfileView } from "@/features/profile/views/ProfileView";
import { ApiError } from "@/lib/http/api-error";
import { useSessionStore } from "@/features/auth/store/session.store";
import { renderWithProviders } from "@/test/render";

vi.mock("@/api/profile/profile.api", () => ({
  getCurrentProfile: vi.fn(),
  upsertCurrentProfile: vi.fn()
}));

vi.mock("@/components/MarkdownContent", () => ({
  MarkdownContent: ({ content }: { content: string | null }) =>
    content ? <div data-testid="markdown-content">{content}</div> : null
}));

const testProfile: UserProfileResponseDto = {
  id: 1,
  userAccountId: 1,
  fullName: "Ada Lovelace",
  preferredTitle: "Senior Engineer",
  primaryLocationPostcode: "SW1A 1AA",
  linkedInUrl: null,
  gitHubUrl: null,
  portfolioUrl: null,
  summary: "I build things.",
  skillsText: "C#, TypeScript",
  experienceText: "5 years at ACME",
  preferencesText: "Remote preferred",
  createdAtUtc: "2026-04-11T10:00:00Z",
  updatedAtUtc: "2026-04-11T10:00:00Z"
};

describe("ProfileView", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    useSessionStore.setState({
      user: { userAccount: "admin", displayName: "Admin", role: "admin", email: "admin@example.com" },
      isAuthenticated: true
    });
  });

  it("shows a loading spinner while fetching the profile", () => {
    vi.mocked(getCurrentProfile).mockReturnValueOnce(new Promise(vi.fn()));
    renderWithProviders(<ProfileView />);
    expect(screen.getByRole("progressbar")).toBeInTheDocument();
  });

  it("shows the profile in view mode when loaded", async () => {
    vi.mocked(getCurrentProfile).mockResolvedValueOnce(testProfile);
    renderWithProviders(<ProfileView />);
    expect(await screen.findByText("Ada Lovelace")).toBeInTheDocument();
    expect(screen.getByText("Senior Engineer")).toBeInTheDocument();
    expect(screen.getByText("SW1A 1AA")).toBeInTheDocument();
  });

  it("renders markdown content sections in view mode", async () => {
    vi.mocked(getCurrentProfile).mockResolvedValueOnce(testProfile);
    renderWithProviders(<ProfileView />);
    await screen.findByText("Ada Lovelace");
    const markdownEls = screen.getAllByTestId("markdown-content");
    expect(markdownEls.length).toBeGreaterThan(0);
    expect(markdownEls.some((el) => el.textContent === "I build things.")).toBe(true);
  });

  it("switches to edit mode when Edit profile is clicked", async () => {
    const user = userEvent.setup();
    vi.mocked(getCurrentProfile).mockResolvedValueOnce(testProfile);
    renderWithProviders(<ProfileView />);
    await screen.findByText("Ada Lovelace");
    await user.click(screen.getByRole("button", { name: "Edit profile" }));
    expect(screen.getByDisplayValue("Ada Lovelace")).toBeInTheDocument();
    expect(screen.getByDisplayValue("Senior Engineer")).toBeInTheDocument();
  });

  it("cancels edits and returns to view mode without saving", async () => {
    const user = userEvent.setup();
    vi.mocked(getCurrentProfile).mockResolvedValueOnce(testProfile);
    renderWithProviders(<ProfileView />);
    await screen.findByText("Ada Lovelace");
    await user.click(screen.getByRole("button", { name: "Edit profile" }));
    await user.click(screen.getByRole("button", { name: "Cancel" }));
    expect(screen.getByText("Ada Lovelace")).toBeInTheDocument();
    expect(screen.queryByDisplayValue("Ada Lovelace")).not.toBeInTheDocument();
    expect(upsertCurrentProfile).not.toHaveBeenCalled();
  });

  it("saves changes and returns to view mode", async () => {
    const user = userEvent.setup();
    vi.mocked(getCurrentProfile).mockResolvedValueOnce(testProfile);
    vi.mocked(upsertCurrentProfile).mockResolvedValueOnce({
      ...testProfile,
      preferredTitle: "Principal Engineer"
    });
    renderWithProviders(<ProfileView />);
    await screen.findByText("Ada Lovelace");
    await user.click(screen.getByRole("button", { name: "Edit profile" }));
    await user.click(screen.getByRole("button", { name: "Save profile" }));
    await waitFor(() => {
      expect(screen.getByText("Profile saved.")).toBeInTheDocument();
    });
    expect(screen.queryByDisplayValue("Ada Lovelace")).not.toBeInTheDocument();
  });

  it("shows an empty state when no profile exists", async () => {
    vi.mocked(getCurrentProfile).mockRejectedValueOnce(new ApiError("Not found", 404));
    renderWithProviders(<ProfileView />);
    expect(await screen.findByText(/no profile/i)).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /set up/i })).toBeInTheDocument();
  });

  it("shows an error state with a retry button when loading fails", async () => {
    vi.mocked(getCurrentProfile).mockRejectedValueOnce(new ApiError("Server error", 500));
    renderWithProviders(<ProfileView />);
    await waitFor(() => {
      expect(screen.getByText("We couldn't load your profile right now.")).toBeInTheDocument();
    });
    expect(screen.getByRole("button", { name: "Retry" })).toBeInTheDocument();
  });
});
