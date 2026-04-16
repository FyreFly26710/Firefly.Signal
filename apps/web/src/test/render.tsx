import { render, renderHook, type RenderHookOptions, type RenderOptions } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import type { ReactElement } from "react";
import { MemoryRouter } from "react-router-dom";
import { AppProviders } from "@/app/AppProviders";

type RenderWithProvidersOptions = Omit<RenderOptions, "wrapper"> & {
  hydrateSessionOnMount?: boolean;
  route?: string;
  withRouter?: boolean;
};

function createTestQueryClient() {
  return new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
        staleTime: Infinity
      },
      mutations: {
        retry: false
      }
    }
  });
}

export function renderHookWithProviders<TResult, TProps>(
  hook: (props: TProps) => TResult,
  options?: RenderHookOptions<TProps>
) {
  const queryClient = createTestQueryClient();

  return renderHook(hook, {
    ...options,
    wrapper: ({ children }) => (
      <QueryClientProvider client={queryClient}>
        <AppProviders hydrateSessionOnMount={false}>{children}</AppProviders>
      </QueryClientProvider>
    )
  });
}

export function renderWithProviders(
  ui: ReactElement,
  {
    hydrateSessionOnMount = false,
    route = "/",
    withRouter = true,
    ...renderOptions
  }: RenderWithProvidersOptions = {}
) {
  const queryClient = createTestQueryClient();
  const content = withRouter ? <MemoryRouter initialEntries={[route]}>{ui}</MemoryRouter> : ui;

  return render(
    <QueryClientProvider client={queryClient}>
      <AppProviders hydrateSessionOnMount={hydrateSessionOnMount}>{content}</AppProviders>
    </QueryClientProvider>,
    renderOptions
  );
}
