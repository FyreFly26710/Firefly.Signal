import { render, type RenderOptions } from "@testing-library/react";
import type { ReactElement } from "react";
import { MemoryRouter } from "react-router-dom";
import { AppProviders } from "@/app/AppProviders";

type RenderWithProvidersOptions = Omit<RenderOptions, "wrapper"> & {
  hydrateSessionOnMount?: boolean;
  route?: string;
  withRouter?: boolean;
};

export function renderWithProviders(
  ui: ReactElement,
  {
    hydrateSessionOnMount = false,
    route = "/",
    withRouter = true,
    ...renderOptions
  }: RenderWithProvidersOptions = {}
) {
  const content = withRouter ? <MemoryRouter initialEntries={[route]}>{ui}</MemoryRouter> : ui;

  return render(
    <AppProviders hydrateSessionOnMount={hydrateSessionOnMount}>{content}</AppProviders>,
    renderOptions
  );
}
