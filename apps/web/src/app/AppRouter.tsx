import { Suspense, lazy } from "react";
import { RouterProvider, createBrowserRouter } from "react-router-dom";
import { RouteLoadingScreen } from "@/app/RouteLoadingScreen";
import { AdminRoute } from "@/features/auth/components/AdminRoute";
import { ProtectedRoute } from "@/features/auth/components/ProtectedRoute";
import { SearchPage } from "@/routes/SearchPage";

const SearchResultsPage = lazy(() =>
  import("@/routes/SearchResultsPage").then((module) => ({
    default: module.SearchResultsPage
  }))
);
const LoginPage = lazy(() =>
  import("@/routes/LoginPage").then((module) => ({
    default: module.LoginPage
  }))
);
const JobDetailPage = lazy(() =>
  import("@/routes/JobDetailPage").then((module) => ({
    default: module.JobDetailPage
  }))
);
const AppHomePage = lazy(() =>
  import("@/routes/AppHomePage").then((module) => ({
    default: module.AppHomePage
  }))
);
const AppJobsPage = lazy(() =>
  import("@/routes/AppJobsPage").then((module) => ({
    default: module.AppJobsPage
  }))
);
const AppJobCreatePage = lazy(() =>
  import("@/routes/AppJobCreatePage").then((module) => ({
    default: module.AppJobCreatePage
  }))
);
const AppJobDetailPage = lazy(() =>
  import("@/routes/AppJobDetailPage").then((module) => ({
    default: module.AppJobDetailPage
  }))
);

function withRouteFallback(page: React.ReactNode) {
  return <Suspense fallback={<RouteLoadingScreen />}>{page}</Suspense>;
}

const router = createBrowserRouter([
  {
    path: "/",
    element: <SearchPage />
  },
  {
    path: "/search",
    element: withRouteFallback(<SearchResultsPage />)
  },
  {
    path: "/login",
    element: withRouteFallback(<LoginPage />)
  },
  {
    path: "/jobs/:jobId",
    element: withRouteFallback(<JobDetailPage />)
  },
  {
    path: "/app",
    element: (
      <ProtectedRoute>
        {withRouteFallback(<AppHomePage />)}
      </ProtectedRoute>
    )
  },
  {
    path: "/admin/manage-jobs",
    element: (
      <AdminRoute>
        {withRouteFallback(<AppJobsPage />)}
      </AdminRoute>
    )
  },
  {
    path: "/admin/manage-jobs/new",
    element: (
      <AdminRoute>
        {withRouteFallback(<AppJobCreatePage />)}
      </AdminRoute>
    )
  },
  {
    path: "/admin/manage-jobs/:jobId",
    element: (
      <AdminRoute>
        {withRouteFallback(<AppJobDetailPage />)}
      </AdminRoute>
    )
  }
]);

export function AppRouter() {
  return <RouterProvider router={router} />;
}
