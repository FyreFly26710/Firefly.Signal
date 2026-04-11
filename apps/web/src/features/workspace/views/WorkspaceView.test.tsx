import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { describe, expect, it } from "vitest";
import { AppProviders } from "@/app/AppProviders";
import { WorkspaceView } from "@/features/workspace/views/WorkspaceView";

describe("WorkspaceView", () => {
  it("keeps the workspace focused on supported search actions", () => {
    render(
      <MemoryRouter>
        <AppProviders>
          <WorkspaceView />
        </AppProviders>
      </MemoryRouter>
    );

    expect(screen.getByRole("heading", { name: "Your workspace" })).toBeInTheDocument();
    expect(screen.getByRole("heading", { name: "Search from your workspace" })).toBeInTheDocument();
    expect(screen.queryByText("Recent activity")).not.toBeInTheDocument();
    expect(screen.queryByText("Saved searches")).not.toBeInTheDocument();
    expect(screen.queryByText("Applications")).not.toBeInTheDocument();
  });
});
