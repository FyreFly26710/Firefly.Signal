import { screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { JobCard } from "@/features/jobs/components/JobCard";
import { renderWithProviders } from "@/test/render";

describe("JobCard", () => {
  it("renders the apply action before the save action", () => {
    renderWithProviders(
      <JobCard
        job={{
          id: "42",
          title: "Senior Product Designer",
          employer: "Acme",
          location: "London",
          source: "Indeed",
          summary: "Design thoughtful user journeys.",
          postedDate: "10 Apr 2026",
          url: "https://example.com/jobs/42"
        }}
        onApply={vi.fn()}
        onToggleSave={vi.fn()}
        onToggleHide={vi.fn()}
      />
    );

    const buttons = screen.getAllByRole("button");

    expect(buttons[0]).toHaveAccessibleName("Mark as applied");
    expect(buttons[1]).toHaveAccessibleName("Save job");
  });
});
