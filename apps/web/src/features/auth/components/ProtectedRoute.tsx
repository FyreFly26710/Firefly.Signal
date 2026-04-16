import type { PropsWithChildren } from "react";
import { Navigate, useLocation } from "react-router-dom";
import { useSessionStore } from "@/features/auth/store/session.store";

export function ProtectedRoute({ children }: PropsWithChildren) {
  const isAuthenticated = useSessionStore((state) => state.isAuthenticated);
  const location = useLocation();
  const from = `${location.pathname}${location.search}`;

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from }} />;
  }

  return children;
}
