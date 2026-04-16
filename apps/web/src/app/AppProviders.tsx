import { CssBaseline, ThemeProvider } from "@mui/material";
import { useEffect } from "react";
import type { PropsWithChildren } from "react";
import { theme } from "@/app/theme";
import { useSessionStore } from "@/features/auth/store/session.store";

type AppProvidersProps = PropsWithChildren<{
  hydrateSessionOnMount?: boolean;
}>;

export function AppProviders({
  children,
  hydrateSessionOnMount = true
}: AppProvidersProps) {
  const hydrate = useSessionStore((state) => state.hydrate);

  useEffect(() => {
    if (!hydrateSessionOnMount) {
      return;
    }

    void hydrate();
  }, [hydrate, hydrateSessionOnMount]);

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      {children}
    </ThemeProvider>
  );
}
