import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import { MarkdownField } from "@/components/MarkdownField";

vi.mock("@/components/MarkdownContent", () => ({
  MarkdownContent: ({ content }: { content: string | null }) =>
    content ? <div data-testid="markdown-content">{content}</div> : null
}));

describe("MarkdownField", () => {
  it("renders in Write mode by default", () => {
    render(<MarkdownField label="Summary" value="Hello world" onChange={vi.fn()} />);
    expect(screen.getByDisplayValue("Hello world")).toBeInTheDocument();
    expect(screen.queryByTestId("markdown-content")).not.toBeInTheDocument();
  });

  it("switches to Preview mode showing rendered markdown", async () => {
    const user = userEvent.setup();
    render(<MarkdownField label="Summary" value="Hello world" onChange={vi.fn()} />);
    await user.click(screen.getByRole("button", { name: "Preview" }));
    expect(screen.getByTestId("markdown-content")).toBeInTheDocument();
    expect(screen.queryByDisplayValue("Hello world")).not.toBeInTheDocument();
  });

  it("shows an empty preview message when content is blank", async () => {
    const user = userEvent.setup();
    render(<MarkdownField label="Summary" value="" onChange={vi.fn()} />);
    await user.click(screen.getByRole("button", { name: "Preview" }));
    expect(screen.getByText("Nothing to preview.")).toBeInTheDocument();
  });

  it("switches back to Write mode from Preview", async () => {
    const user = userEvent.setup();
    render(<MarkdownField label="Summary" value="Hello" onChange={vi.fn()} />);
    await user.click(screen.getByRole("button", { name: "Preview" }));
    await user.click(screen.getByRole("button", { name: "Write" }));
    expect(screen.getByDisplayValue("Hello")).toBeInTheDocument();
  });
});
