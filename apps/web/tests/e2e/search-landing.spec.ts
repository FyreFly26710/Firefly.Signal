import { expect, test } from "@playwright/test";

test.describe("Search landing", () => {
  test("shows the public entry points", async ({ page }) => {
    await page.goto("/");

    await expect(page.getByRole("link", { name: "Discover" })).toBeVisible();
    await expect(page.getByRole("link", { name: "Search" })).toBeVisible();
    await expect(page.getByRole("link", { name: "Workspace" })).toBeVisible();
  });
});
