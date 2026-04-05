import { screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { afterEach, describe, expect, it } from "vitest";
import { LoginExperience } from "@/features/auth/components/LoginExperience";
import { renderWithProviders } from "@/test/render";
import { useSessionStore } from "@/store/session.store";

describe("LoginExperience", () => {
  afterEach(() => {
    useSessionStore.getState().reset();
  });

  it("shows an error for invalid mock credentials", async () => {
    const user = userEvent.setup();

    renderWithProviders(
      <MemoryRouter>
        <LoginExperience />
      </MemoryRouter>
    );

    await user.clear(screen.getByLabelText(/user account/i));
    await user.type(screen.getByLabelText(/user account/i), "wrong");
    await user.clear(screen.getByLabelText(/password/i));
    await user.type(screen.getByLabelText(/password/i), "bad-password");
    await user.click(screen.getByRole("button", { name: /continue to app/i }));

    expect(await screen.findByText(/do not match the current mvp mock sign-in/i)).toBeInTheDocument();
  });

  it("navigates into the app for valid mock credentials", async () => {
    const user = userEvent.setup();

    renderWithProviders(
      <MemoryRouter initialEntries={["/login"]}>
        <Routes>
          <Route path="/login" element={<LoginExperience />} />
          <Route path="/app" element={<div>Workspace route</div>} />
        </Routes>
      </MemoryRouter>
    );

    await user.click(screen.getByRole("button", { name: /continue to app/i }));

    await waitFor(() => {
      expect(screen.getByText("Workspace route")).toBeInTheDocument();
    });
  });
});
