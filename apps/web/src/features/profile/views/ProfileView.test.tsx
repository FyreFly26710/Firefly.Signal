import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { ThemeProvider } from "@mui/material";
import { MemoryRouter } from "react-router-dom";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { theme } from "@/app/theme";
import { getCurrentProfile, upsertCurrentProfile } from "@/api/profile/profile.api";
import { ProfileView } from "@/features/profile/views/ProfileView";
import { ApiError } from "@/lib/http/api-error";
import { useSessionStore } from "@/store/session.store";

vi.mock("@/api/profile/profile.api", () => ({
  getCurrentProfile: vi.fn(),
  upsertCurrentProfile: vi.fn()
}));

describe("ProfileView", () => {
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

  it("loads the current profile and saves edits", async () => {
    const user = userEvent.setup();

    vi.mocked(getCurrentProfile).mockResolvedValueOnce({
      id: 1,
      userAccountId: 1,
      fullName: "Ada Lovelace",
      preferredTitle: "Senior Developer",
      primaryLocationPostcode: "SW1A 1AA",
      linkedInUrl: "https://linkedin.com/in/ada",
      githubUrl: "https://github.com/ada",
      portfolioUrl: "https://ada.dev",
      summary: "Original summary",
      skillsText: "C#, .NET",
      experienceText: "Original experience",
      preferencesJson: "{}",
      createdAtUtc: "2026-04-11T10:00:00Z",
      updatedAtUtc: "2026-04-11T10:00:00Z"
    });

    vi.mocked(upsertCurrentProfile).mockResolvedValueOnce({
      id: 1,
      userAccountId: 1,
      fullName: "Ada Lovelace",
      preferredTitle: "Principal Engineer",
      primaryLocationPostcode: "EC1A 1BB",
      linkedInUrl: "https://linkedin.com/in/ada",
      githubUrl: "https://github.com/ada",
      portfolioUrl: "https://ada.dev",
      summary: "Updated summary",
      skillsText: "C#, .NET, Azure",
      experienceText: "Updated experience",
      preferencesJson: "{\"preferredJobTypes\":[\"backend\"]}",
      createdAtUtc: "2026-04-11T10:00:00Z",
      updatedAtUtc: "2026-04-11T11:00:00Z"
    });

    render(
      <ThemeProvider theme={theme}>
        <MemoryRouter initialEntries={["/app/profile"]}>
          <ProfileView />
        </MemoryRouter>
      </ThemeProvider>
    );

    await waitFor(() => {
      expect(screen.getByDisplayValue("Ada Lovelace")).toBeInTheDocument();
    });

    await user.clear(screen.getByLabelText("Preferred title"));
    await user.type(screen.getByLabelText("Preferred title"), "Principal Engineer");
    await user.clear(screen.getByLabelText("Primary UK postcode"));
    await user.type(screen.getByLabelText("Primary UK postcode"), "ec1a 1bb");
    await user.click(screen.getByRole("button", { name: "Save profile" }));

    await waitFor(() => {
      expect(screen.getByText("Profile saved.")).toBeInTheDocument();
    });

    expect(upsertCurrentProfile).toHaveBeenCalledWith(
      expect.objectContaining({
        preferredTitle: "Principal Engineer",
        primaryLocationPostcode: "ec1a 1bb"
      })
    );
    expect(screen.getByDisplayValue("EC1A 1BB")).toBeInTheDocument();
  }, 10000);

  it("shows a retry state when profile loading fails", async () => {
    vi.mocked(getCurrentProfile).mockRejectedValueOnce(new ApiError("Server unavailable", 500));

    render(
      <ThemeProvider theme={theme}>
        <MemoryRouter initialEntries={["/app/profile"]}>
          <ProfileView />
        </MemoryRouter>
      </ThemeProvider>
    );

    await waitFor(() => {
      expect(screen.getByText("We couldn't load your profile right now.")).toBeInTheDocument();
    });

    expect(screen.getByRole("button", { name: "Retry" })).toBeInTheDocument();
  });
});
