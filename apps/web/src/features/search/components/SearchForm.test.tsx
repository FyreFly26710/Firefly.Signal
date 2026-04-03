import { screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import { SearchForm } from "@/features/search/components/SearchForm";
import { renderWithProviders } from "@/test/render";

describe("SearchForm", () => {
  it("requires postcode and keyword before submitting", async () => {
    const user = userEvent.setup();
    const onSubmit = vi.fn<() => Promise<void>>().mockResolvedValue();

    renderWithProviders(
      <SearchForm
        initialKeyword=""
        initialPostcode=""
        isSubmitting={false}
        onSubmit={onSubmit}
      />
    );

    await user.click(screen.getByRole("button", { name: /search jobs/i }));

    expect(onSubmit).not.toHaveBeenCalled();
    expect(screen.getByText("Enter a postcode.")).toBeInTheDocument();
    expect(screen.getByText("Enter a keyword.")).toBeInTheDocument();
  });
});
