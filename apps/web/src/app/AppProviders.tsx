import { CssBaseline, ThemeProvider } from "@mui/material";
import { useEffect } from "react";
import type { PropsWithChildren } from "react";
import { theme } from "@/app/theme";
import { useSessionStore } from "@/store/session.store";

export function AppProviders({ children }: PropsWithChildren) {
  const hydrate = useSessionStore((state) => state.hydrate);

  useEffect(() => {
    void hydrate();
  }, [hydrate]);

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      {children}
    </ThemeProvider>
  );
}
