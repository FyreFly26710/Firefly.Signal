import { Navigate } from "react-router-dom";
import { useSessionStore } from "@/store/session.store";

export function SessionRedirect() {
  const isAuthenticated = useSessionStore((state) => state.isAuthenticated);
  return <Navigate to={isAuthenticated ? "/app" : "/login"} replace />;
}
