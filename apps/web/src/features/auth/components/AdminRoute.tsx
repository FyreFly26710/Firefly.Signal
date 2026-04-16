import type { PropsWithChildren } from "react";
import { Navigate, useLocation } from "react-router-dom";
import { useSessionStore } from "@/features/auth/store/session.store";

export function AdminRoute({ children }: PropsWithChildren) {
  const isAuthenticated = useSessionStore((state) => state.isAuthenticated);
  const user = useSessionStore((state) => state.user);
  const location = useLocation();
  const from = `${location.pathname}${location.search}`;

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from }} />;
  }

  if (user?.role !== "admin") {
    return <Navigate to="/app" replace />;
  }

  return children;
}
