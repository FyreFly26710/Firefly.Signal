import { RouterProvider, createBrowserRouter } from "react-router-dom";
import { ProtectedRoute } from "@/features/auth/components/ProtectedRoute";
import { SessionRedirect } from "@/features/auth/components/SessionRedirect";
import { AppHomePage } from "@/routes/AppHomePage";
import { LoginPage } from "@/routes/LoginPage";

const router = createBrowserRouter([
  {
    path: "/",
    element: <SessionRedirect />
  },
  {
    path: "/login",
    element: <LoginPage />
  },
  {
    path: "/app",
    element: (
      <ProtectedRoute>
        <AppHomePage />
      </ProtectedRoute>
    )
  }
]);

export function AppRouter() {
  return <RouterProvider router={router} />;
}
