import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { AppProviders } from "@/app/AppProviders";
import { AppRouter } from "@/app/AppRouter";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      retry: 1
    }
  }
});

export function AppRoot() {
  return (
    <QueryClientProvider client={queryClient}>
      <AppProviders>
        <AppRouter />
      </AppProviders>
    </QueryClientProvider>
  );
}
