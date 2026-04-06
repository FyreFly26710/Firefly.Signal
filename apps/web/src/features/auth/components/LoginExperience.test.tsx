import { screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { LoginExperience } from "@/features/auth/components/LoginExperience";
import { renderWithProviders } from "@/test/render";
import { useSessionStore } from "@/store/session.store";

describe("LoginExperience", () => {
  beforeEach(() => {
    vi.stubGlobal("fetch", vi.fn());
  });

  afterEach(() => {
    useSessionStore.getState().reset();
    vi.unstubAllGlobals();
  });

  it("shows an error for rejected credentials", async () => {
    const user = userEvent.setup();
    vi.mocked(fetch).mockResolvedValue(
      new Response(null, {
        status: 401
      })
    );

    renderWithProviders(
      <MemoryRouter>
        <LoginExperience />
      </MemoryRouter>
    );

    await user.clear(screen.getByLabelText(/user account/i));
    await user.type(screen.getByLabelText(/user account/i), "wrong");
    await user.clear(screen.getByLabelText(/password/i));
    await user.type(screen.getByLabelText(/password/i), "bad-password");
    await user.click(screen.getByRole("button", { name: /continue to workspace/i }));

    expect(await screen.findByText(/rejected by the identity api/i)).toBeInTheDocument();
  });

  it("navigates into the app for valid credentials", async () => {
    const user = userEvent.setup();
    vi.mocked(fetch).mockResolvedValue(
      new Response(
        JSON.stringify({
          accessToken: "test-token",
          tokenType: "Bearer",
          expiresAtUtc: "2026-04-05T20:00:00Z",
          user: {
            userId: 1,
            userAccount: "admin",
            displayName: "Firefly Admin",
            email: "admin@firefly.local",
            role: "admin"
          }
        }),
        {
          status: 200,
          headers: {
            "Content-Type": "application/json"
          }
        }
      )
    );

    renderWithProviders(
      <MemoryRouter initialEntries={["/login"]}>
        <Routes>
          <Route path="/login" element={<LoginExperience />} />
          <Route path="/app" element={<div>Workspace route</div>} />
        </Routes>
      </MemoryRouter>
    );

    await user.click(screen.getByRole("button", { name: /continue to workspace/i }));

    await waitFor(() => {
      expect(screen.getByText("Workspace route")).toBeInTheDocument();
    });
  });
});
