import AutoAwesomeRoundedIcon from "@mui/icons-material/AutoAwesomeRounded";
import DashboardRoundedIcon from "@mui/icons-material/DashboardRounded";
import LoginRoundedIcon from "@mui/icons-material/LoginRounded";
import LogoutRoundedIcon from "@mui/icons-material/LogoutRounded";
import PersonRoundedIcon from "@mui/icons-material/PersonRounded";
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

type NavigationLink = {
  to: string;
  label: string;
  icon: ReactNode;
  end?: boolean;
};

type HeaderNavigationProps = {
  links: NavigationLink[];
};

type AuthenticatedHeaderActionsProps = {
  email: string;
  onSignOut: () => void;
  displayName: string;
};

const publicLinks: NavigationLink[] = [
  { to: "/", label: "Discover", icon: <SearchRoundedIcon fontSize="inherit" />, end: true },
  { to: "/search", label: "Search", icon: <VisibilityRoundedIcon fontSize="inherit" />, end: true }
];

const authenticatedLinks: NavigationLink[] = [
  ...publicLinks,
  { to: "/app", label: "Workspace", icon: <DashboardRoundedIcon fontSize="inherit" />, end: true },
  { to: "/app/profile", label: "Profile", icon: <PersonRoundedIcon fontSize="inherit" />, end: true },
  { to: "/admin/manage-jobs", label: "Manage jobs", icon: <WorkOutlineRoundedIcon fontSize="inherit" /> }
];

function getNavigationLinkClassName(isActive: boolean) {
  return `inline-flex items-center gap-2 rounded-md px-4 py-2 text-sm font-medium transition-colors ${
    isActive
      ? "bg-accent-secondary text-accent-secondary-foreground"
      : "text-foreground-secondary hover:text-accent-primary"
  }`;
}

function HeaderBrand() {
  return (
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
  );
}

function HeaderNavigation({ links }: HeaderNavigationProps) {
  return (
    <nav className="hidden items-center gap-2 md:flex">
      {links.map((link) => (
        <NavLink
          key={link.to}
          to={link.to}
          end={link.end}
          className={({ isActive }) => getNavigationLinkClassName(isActive)}
        >
          {link.icon}
          {link.label}
        </NavLink>
      ))}
    </nav>
  );
}

function HeaderUserDetails({
  displayName,
  email
}: Pick<AuthenticatedHeaderActionsProps, "displayName" | "email">) {
  return (
    <div className="hidden text-right sm:block">
      <p className="text-sm font-medium text-foreground">{displayName}</p>
      <p className="text-xs text-foreground-tertiary">{email}</p>
    </div>
  );
}

function AuthenticatedHeaderActions({
  email,
  onSignOut,
  displayName
}: AuthenticatedHeaderActionsProps) {
  return (
    <>
      <HeaderUserDetails displayName={displayName} email={email} />
      <Button
        variant="outlined"
        color="inherit"
        startIcon={<LogoutRoundedIcon />}
        onClick={onSignOut}
        sx={{ borderColor: "rgba(214, 211, 207, 1)" }}
      >
        Sign out
      </Button>
    </>
  );
}

function PublicHeaderActions() {
  return (
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
  );
}

export function AppHeader({ variant = "public", actions }: AppHeaderProps) {
  const navigate = useNavigate();
  const user = useSessionStore((state) => state.user);
  const signOut = useSessionStore((state) => state.signOut);
  const effectiveVariant = user ? "authenticated" : variant;
  const links = effectiveVariant === "authenticated" ? authenticatedLinks : publicLinks;

  function handleSignOut() {
    signOut();
    void navigate("/login", { replace: true });
  }

  return (
    <header className="sticky top-0 z-50 border-b border-divider/90 bg-background-elevated/95 backdrop-blur">
      <div className="mx-auto flex max-w-7xl items-center justify-between gap-6 px-5 py-4 sm:px-8">
        <HeaderBrand />
        <HeaderNavigation links={links} />

        <div className="flex items-center gap-3">
          {actions}
          {effectiveVariant === "authenticated" && user ? (
            <AuthenticatedHeaderActions
              displayName={user.displayName}
              email={user.email}
              onSignOut={handleSignOut}
            />
          ) : (
            <PublicHeaderActions />
          )}
        </div>
      </div>
    </header>
  );
}
