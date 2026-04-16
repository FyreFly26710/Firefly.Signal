import { screen } from "@testing-library/react";
import { afterEach, describe, expect, it } from "vitest";
import { AppHeader } from "@/components/AppHeader";
import { useSessionStore } from "@/features/auth/store/session.store";
import { renderWithProviders } from "@/test/render";

describe("AppHeader", () => {
  afterEach(() => {
    useSessionStore.setState({
      user: null,
      isAuthenticated: false
    });
  });

  it("shows authenticated actions on public pages when a user session exists", () => {
    useSessionStore.setState({
      user: {
        userAccount: "admin",
        displayName: "Admin",
        role: "admin",
        email: "admin@example.com"
      },
      isAuthenticated: true
    });

    renderWithProviders(<AppHeader />);

    expect(screen.queryByRole("link", { name: "Sign in" })).not.toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Sign out" })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Workspace" })).toBeInTheDocument();
  });

  it("keeps the workspace link inactive when the profile route is active", () => {
    useSessionStore.setState({
      user: {
        userAccount: "admin",
        displayName: "Admin",
        role: "admin",
        email: "admin@example.com"
      },
      isAuthenticated: true
    });

    renderWithProviders(<AppHeader variant="authenticated" />, { route: "/app/profile" });

    expect(screen.getByRole("link", { name: "Profile" })).toHaveClass(
      "bg-accent-secondary",
      "text-accent-secondary-foreground"
    );
    expect(screen.getByRole("link", { name: "Workspace" })).not.toHaveClass(
      "bg-accent-secondary",
      "text-accent-secondary-foreground"
    );
  });
});
