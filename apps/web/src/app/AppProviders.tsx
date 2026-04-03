import { CssBaseline, ThemeProvider } from "@mui/material";
import type { PropsWithChildren } from "react";
import { theme } from "@/app/theme";

export function AppProviders({ children }: PropsWithChildren) {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      {children}
    </ThemeProvider>
  );
}
