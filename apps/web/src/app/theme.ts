/* eslint-disable @typescript-eslint/consistent-type-definitions */
import { createTheme } from "@mui/material/styles";

declare module "@mui/material/styles" {
  interface Palette {
    brand: Palette["primary"];
  }

  interface PaletteOptions {
    brand?: PaletteOptions["primary"];
  }
}

export const theme = createTheme({
  cssVariables: true,
  palette: {
    mode: "light",
    primary: {
      main: "#0f766e"
    },
    secondary: {
      main: "#f59e0b"
    },
    brand: {
      main: "#14532d",
      light: "#4d7c0f",
      dark: "#052e16",
      contrastText: "#f8fafc"
    },
    background: {
      default: "#f4f7f5",
      paper: "#ffffff"
    }
  },
  shape: {
    borderRadius: 16
  },
  typography: {
    fontFamily: '"Segoe UI", "Helvetica Neue", sans-serif',
    h1: {
      fontSize: "clamp(2.5rem, 5vw, 4rem)",
      fontWeight: 700,
      lineHeight: 1
    },
    h2: {
      fontWeight: 700
    },
    button: {
      fontWeight: 700,
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
