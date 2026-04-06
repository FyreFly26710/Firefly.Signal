import { RouterProvider, createBrowserRouter } from "react-router-dom";
import { ProtectedRoute } from "@/features/auth/components/ProtectedRoute";
import { AppHomePage } from "@/routes/AppHomePage";
import { JobDetailPage } from "@/routes/JobDetailPage";
import { LoginPage } from "@/routes/LoginPage";
import { SearchPage } from "@/routes/SearchPage";
import { SearchResultsPage } from "@/routes/SearchResultsPage";

const router = createBrowserRouter([
  {
    path: "/",
    element: <SearchPage />
  },
  {
    path: "/search",
    element: <SearchResultsPage />
  },
  {
    path: "/login",
    element: <LoginPage />
  },
  {
    path: "/jobs/:jobId",
    element: <JobDetailPage />
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
