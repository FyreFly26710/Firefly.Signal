/* eslint-disable @typescript-eslint/consistent-type-definitions */
import { createTheme } from "@mui/material/styles";

declare module "@mui/material/styles" {
  interface Palette {
    brand: Palette["primary"];
    accent: Palette["primary"];
  }

  interface PaletteOptions {
    brand?: PaletteOptions["primary"];
    accent?: PaletteOptions["primary"];
  }
}

export const theme = createTheme({
  cssVariables: true,
  palette: {
    mode: "light",
    primary: {
      main: "#1a1614"
    },
    secondary: {
      main: "#fef3c7",
      contrastText: "#78350f"
    },
    brand: {
      main: "#d97706",
      light: "#f59e0b",
      dark: "#b45309",
      contrastText: "#ffffff"
    },
    accent: {
      main: "#d97706",
      light: "#f59e0b",
      dark: "#b45309",
      contrastText: "#ffffff"
    },
    background: {
      default: "#faf9f7",
      paper: "#ffffff"
    }
  },
  shape: {
    borderRadius: 8
  },
  typography: {
    fontFamily: '"Inter", system-ui, sans-serif',
    h1: {
      fontFamily: '"Crimson Pro", Georgia, serif',
      fontSize: "clamp(2.5rem, 5vw, 4rem)",
      fontWeight: 600,
      lineHeight: 1.1
    },
    h2: {
      fontFamily: '"Crimson Pro", Georgia, serif',
      fontWeight: 600
    },
    button: {
      fontWeight: 500,
      textTransform: "none"
    }
  },
  components: {
    MuiPaper: {
      styleOverrides: {
        root: {
          backgroundImage: "none"
        }
      }
    },
    MuiButton: {
      defaultProps: {
        disableElevation: true
      }
    }
  }
});
