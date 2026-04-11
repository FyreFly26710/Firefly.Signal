import { render, screen } from "@testing-library/react";
import { ThemeProvider } from "@mui/material";
import { MemoryRouter } from "react-router-dom";
import { afterEach, describe, expect, it } from "vitest";
import { theme } from "@/app/theme";
import { AppHeader } from "@/components/AppHeader";
import { useSessionStore } from "@/store/session.store";

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

    render(
      <ThemeProvider theme={theme}>
        <MemoryRouter>
          <AppHeader />
        </MemoryRouter>
      </ThemeProvider>
    );

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

    render(
      <ThemeProvider theme={theme}>
        <MemoryRouter initialEntries={["/app/profile"]}>
          <AppHeader variant="authenticated" />
        </MemoryRouter>
      </ThemeProvider>
    );

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
