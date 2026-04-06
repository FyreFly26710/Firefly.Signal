import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import { SearchForm } from "@/features/search/components/SearchForm";

describe("SearchForm", () => {
  it("shows a validation message when both fields are empty", async () => {
    const user = userEvent.setup();
    const onSubmit = vi.fn();

    render(<SearchForm onSubmit={onSubmit} />);

    await user.click(screen.getByRole("button", { name: "Search UK jobs" }));

    expect(onSubmit).not.toHaveBeenCalled();
    expect(screen.getByText("Enter a keyword or postcode to start searching.")).toBeInTheDocument();
  });

  it("trims input values before submitting", async () => {
    const user = userEvent.setup();
    const onSubmit = vi.fn();

    render(<SearchForm onSubmit={onSubmit} />);

    await user.type(
      screen.getByPlaceholderText("e.g. Product Designer, Data Scientist, Engineer"),
      "  frontend engineer  "
    );
    await user.type(
      screen.getByPlaceholderText("e.g. EC2A, Shoreditch, London"),
      "  SW1A 1AA  "
    );
    await user.click(screen.getByRole("button", { name: "Search UK jobs" }));

    expect(onSubmit).toHaveBeenCalledWith("SW1A 1AA", "frontend engineer");
    expect(screen.queryByText("Enter a keyword or postcode to start searching.")).not.toBeInTheDocument();
  });

  it("submits when the user presses Enter in a search input", async () => {
    const user = userEvent.setup();
    const onSubmit = vi.fn();

    render(<SearchForm onSubmit={onSubmit} />);

    await user.type(
      screen.getByPlaceholderText("e.g. Product Designer, Data Scientist, Engineer"),
      "Product designer{enter}"
    );

    expect(onSubmit).toHaveBeenCalledWith("", "Product designer");
  });
});
