import { screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { WorkspaceView } from "@/features/workspace/views/WorkspaceView";
import { renderWithProviders } from "@/test/render";

describe("WorkspaceView", () => {
  it("keeps the workspace focused on supported search actions", () => {
    renderWithProviders(<WorkspaceView />);

    expect(screen.getByRole("heading", { name: "Your workspace" })).toBeInTheDocument();
    expect(screen.getByRole("heading", { name: "Search from your workspace" })).toBeInTheDocument();
    expect(screen.queryByText("Recent activity")).not.toBeInTheDocument();
    expect(screen.queryByText("Saved searches")).not.toBeInTheDocument();
    expect(screen.queryByText("Applications")).not.toBeInTheDocument();
  });
});
