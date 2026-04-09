import { CircularProgress } from "@mui/material";

export function RouteLoadingScreen() {
  return (
    <div className="min-h-screen bg-background-base text-foreground">
      <div className="flex min-h-screen items-center justify-center px-6">
        <div className="flex max-w-sm flex-col items-center gap-4 text-center">
          <CircularProgress color="inherit" size={28} />
          <div>
            <p className="font-serif text-2xl font-semibold">Loading screen</p>
            <p className="mt-2 text-sm text-foreground-secondary">
              Fetching the next Firefly Signal view.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
