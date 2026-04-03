import { RouterProvider, createBrowserRouter } from "react-router-dom";
import { SearchPage } from "@/routes/SearchPage";

const router = createBrowserRouter([
  {
    path: "/",
    element: <SearchPage />
  }
]);

export function AppRouter() {
  return <RouterProvider router={router} />;
}
