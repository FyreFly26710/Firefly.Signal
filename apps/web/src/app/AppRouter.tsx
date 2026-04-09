import { RouterProvider, createBrowserRouter } from "react-router-dom";
import { ProtectedRoute } from "@/features/auth/components/ProtectedRoute";
import { AppJobCreatePage } from "@/routes/AppJobCreatePage";
import { AppJobDetailPage } from "@/routes/AppJobDetailPage";
import { AppJobsPage } from "@/routes/AppJobsPage";
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
  },
  {
    path: "/admin/manage-jobs",
    element: (
      <ProtectedRoute>
        <AppJobsPage />
      </ProtectedRoute>
    )
  },
  {
    path: "/admin/manage-jobs/new",
    element: (
      <ProtectedRoute>
        <AppJobCreatePage />
      </ProtectedRoute>
    )
  },
  {
    path: "/admin/manage-jobs/:jobId",
    element: (
      <ProtectedRoute>
        <AppJobDetailPage />
      </ProtectedRoute>
    )
  }
]);

export function AppRouter() {
  return <RouterProvider router={router} />;
}
