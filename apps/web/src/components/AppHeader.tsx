import AutoAwesomeRoundedIcon from "@mui/icons-material/AutoAwesomeRounded";
import DashboardRoundedIcon from "@mui/icons-material/DashboardRounded";
import LoginRoundedIcon from "@mui/icons-material/LoginRounded";
import LogoutRoundedIcon from "@mui/icons-material/LogoutRounded";
import SearchRoundedIcon from "@mui/icons-material/SearchRounded";
import VisibilityRoundedIcon from "@mui/icons-material/VisibilityRounded";
import WorkOutlineRoundedIcon from "@mui/icons-material/WorkOutlineRounded";
import { Button } from "@mui/material";
import type { ReactNode } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import { useSessionStore } from "@/store/session.store";

type AppHeaderProps = {
  variant?: "public" | "authenticated";
  actions?: ReactNode;
};

const publicLinks = [
  { to: "/", label: "Discover", icon: <SearchRoundedIcon fontSize="inherit" /> },
  { to: "/search", label: "Search", icon: <VisibilityRoundedIcon fontSize="inherit" /> }
];

const authenticatedLinks = [
  ...publicLinks,
  { to: "/app", label: "Workspace", icon: <DashboardRoundedIcon fontSize="inherit" /> },
  { to: "/admin/manage-jobs", label: "Manage jobs", icon: <WorkOutlineRoundedIcon fontSize="inherit" /> }
];

export function AppHeader({ variant = "public", actions }: AppHeaderProps) {
  const navigate = useNavigate();
  const user = useSessionStore((state) => state.user);
  const signOut = useSessionStore((state) => state.signOut);
  const links = variant === "authenticated" ? authenticatedLinks : publicLinks;

  function handleSignOut() {
    signOut();
    void navigate("/login", { replace: true });
  }

  return (
    <header className="sticky top-0 z-50 border-b border-divider/90 bg-background-elevated/95 backdrop-blur">
      <div className="mx-auto flex max-w-7xl items-center justify-between gap-6 px-5 py-4 sm:px-8">
        <NavLink to="/" className="flex items-center gap-3">
          <div className="flex h-10 w-10 items-center justify-center rounded-md bg-accent-primary text-accent-primary-foreground">
            <AutoAwesomeRoundedIcon fontSize="small" />
          </div>
          <div>
            <p className="font-serif text-xl font-semibold text-foreground">Firefly Signal</p>
            <p className="font-mono text-xs tracking-[0.18em] text-foreground-tertiary">
              CAREER INTELLIGENCE
            </p>
          </div>
        </NavLink>

        <nav className="hidden items-center gap-2 md:flex">
          {links.map((link) => (
            <NavLink
              key={link.to}
              to={link.to}
              className={({ isActive }) =>
                `inline-flex items-center gap-2 rounded-md px-4 py-2 text-sm font-medium transition-colors ${
                  isActive
                    ? "bg-accent-secondary text-accent-secondary-foreground"
                    : "text-foreground-secondary hover:text-accent-primary"
                }`
              }
            >
              {link.icon}
              {link.label}
            </NavLink>
          ))}
        </nav>

        <div className="flex items-center gap-3">
          {actions}
          {variant === "authenticated" && user ? (
            <>
              <div className="hidden text-right sm:block">
                <p className="text-sm font-medium text-foreground">{user.displayName}</p>
                <p className="text-xs text-foreground-tertiary">{user.email}</p>
              </div>
              <Button
                variant="outlined"
                color="inherit"
                startIcon={<LogoutRoundedIcon />}
                onClick={handleSignOut}
                sx={{ borderColor: "rgba(214, 211, 207, 1)" }}
              >
                Sign out
              </Button>
            </>
          ) : (
            <>
              <Button component={NavLink} to="/login" color="inherit" startIcon={<LoginRoundedIcon />}>
                Sign in
              </Button>
              <Button
                component={NavLink}
                to="/app"
                variant="contained"
                sx={{
                  bgcolor: "accent.main",
                  "&:hover": { bgcolor: "accent.dark" }
                }}
              >
                Workspace
              </Button>
            </>
          )}
        </div>
      </div>
    </header>
  );
}
