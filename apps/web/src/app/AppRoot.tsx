import { AppProviders } from "@/app/AppProviders";
import { AppRouter } from "@/app/AppRouter";

export function AppRoot() {
  return (
    <AppProviders>
      <AppRouter />
    </AppProviders>
  );
}
