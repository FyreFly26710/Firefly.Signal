import { Button } from "@mui/material";
import { Link } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";

export function JobDetailNotFound() {
  return (
    <div className="min-h-screen bg-background">
      <AppHeader />
      <div className="mx-auto max-w-7xl px-5 py-20 text-center sm:px-8">
        <h1 className="font-serif text-4xl font-semibold text-foreground">Job not found</h1>
        <p className="mt-4 text-foreground-secondary">
          This listing is unavailable or may have been removed. Head back to the search results to
          continue browsing.
        </p>
        <Button component={Link} to="/search" sx={{ mt: 4 }}>
          Back to search
        </Button>
      </div>
    </div>
  );
}
