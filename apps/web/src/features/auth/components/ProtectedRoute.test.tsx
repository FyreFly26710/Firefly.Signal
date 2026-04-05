import { screen } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { afterEach, describe, expect, it } from "vitest";
import { ProtectedRoute } from "@/features/auth/components/ProtectedRoute";
import { renderWithProviders } from "@/test/render";
import { useSessionStore } from "@/store/session.store";

describe("ProtectedRoute", () => {
  afterEach(() => {
    useSessionStore.getState().reset();
  });

  it("redirects unauthenticated users to login", () => {
    renderWithProviders(
      <MemoryRouter initialEntries={["/app"]}>
        <Routes>
          <Route
            path="/app"
            element={
              <ProtectedRoute>
                <div>Protected content</div>
              </ProtectedRoute>
            }
          />
          <Route path="/login" element={<div>Login screen</div>} />
        </Routes>
      </MemoryRouter>
    );

    expect(screen.getByText("Login screen")).toBeInTheDocument();
  });

  it("renders children when a session exists", () => {
    useSessionStore.setState({
      user: {
        userAccount: "admin",
        displayName: "Firefly Admin",
        role: "admin",
        email: "admin@firefly.local"
      },
      isAuthenticated: true
    });

    renderWithProviders(
      <MemoryRouter initialEntries={["/app"]}>
        <Routes>
          <Route
            path="/app"
            element={
              <ProtectedRoute>
                <div>Protected content</div>
              </ProtectedRoute>
            }
          />
        </Routes>
      </MemoryRouter>
    );

    expect(screen.getByText("Protected content")).toBeInTheDocument();
  });
});
