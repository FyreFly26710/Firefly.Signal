import { Alert, Button, TextField } from "@mui/material";
import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { useSessionStore } from "@/store/session.store";

type LocationState = {
  from?: string;
};

export function LoginExperience() {
  const navigate = useNavigate();
  const location = useLocation();
  const signIn = useSessionStore((state) => state.signIn);
  const [userAccount, setUserAccount] = useState("admin");
  const [password, setPassword] = useState("Admin123!");
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSubmitError(null);
    setIsSubmitting(true);

    try {
      await signIn({
        userAccount: userAccount.trim(),
        password: password.trim()
      });

      const nextPath = (location.state as LocationState | null)?.from ?? "/app";
      void navigate(nextPath, { replace: true });
    } catch (error) {
      setSubmitError(
        error instanceof Error ? error.message : "The sign-in request could not be completed."
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="min-h-screen bg-background">
      <AppHeader />

      <div className="mx-auto grid max-w-7xl gap-10 px-5 py-16 sm:px-8 xl:grid-cols-[minmax(0,1fr)_420px]">
        <section className="rounded-lg border border-border bg-background-elevated px-8 py-10">
          <div className="max-w-3xl">
            <div className="mb-6 flex items-center gap-2">
              <div className="h-px w-12 bg-accent-primary" />
              <span className="font-mono text-sm tracking-[0.18em] text-accent-primary">
                PROTECTED WORKSPACE
              </span>
            </div>

            <h1 className="font-serif text-5xl font-semibold leading-tight text-foreground">
              Sign in to continue into your career intelligence workspace.
            </h1>
            <p className="mt-6 max-w-2xl text-xl leading-8 text-foreground-secondary">
              The app uses the live auth flow, but everything after sign-in is currently powered by
              mock job data so the new UI can be reviewed independently from backend search work.
            </p>
          </div>

          <div className="mt-10 grid gap-4 md:grid-cols-3">
            <HighlightCard
              title="Real auth"
              body="Keep the existing JWT-backed login path so protected flows still behave like the real app."
            />
            <HighlightCard
              title="Mock discovery"
              body="The signed-in workspace and job search experience use mock data only for this UI pass."
            />
            <HighlightCard
              title="Future-ready"
              body="The layout leaves room for saved searches, job tracking, resume workflows, and AI support."
            />
          </div>
        </section>

        <section className="rounded-lg border border-border-strong bg-background-elevated p-8 shadow-sm">
          <p className="font-mono text-xs tracking-[0.18em] text-metadata">SIGN IN</p>
          <h2 className="mt-3 font-serif text-3xl font-semibold text-foreground">Open the workspace.</h2>
          <p className="mt-3 text-sm leading-7 text-foreground-secondary">
            Use one of the seeded backend users to request a real JWT and enter the protected mock-data experience.
          </p>

          <div className="mt-5 rounded-md bg-accent-secondary p-4 text-sm text-accent-secondary-foreground">
            <p className="font-medium">Seeded credentials</p>
            <p className="mt-2"><code>admin</code> / <code>Admin123!</code></p>
            <p><code>analyst</code> / <code>Analyst123!</code></p>
          </div>

          <form className="mt-6 flex flex-col gap-4" onSubmit={(event) => void handleSubmit(event)}>
            <TextField
              label="User account"
              value={userAccount}
              onChange={(event) => setUserAccount(event.target.value)}
              autoComplete="username"
              fullWidth
            />

            <TextField
              label="Password"
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              autoComplete="current-password"
              fullWidth
            />

            {submitError ? <Alert severity="error">{submitError}</Alert> : null}

            <Button
              type="submit"
              variant="contained"
              size="large"
              disabled={isSubmitting}
              sx={{
                mt: 1,
                minHeight: 54,
                bgcolor: "accent.main",
                "&:hover": { bgcolor: "accent.dark" }
              }}
            >
              {isSubmitting ? "Signing in..." : "Continue to workspace"}
            </Button>
          </form>
        </section>
      </div>
    </div>
  );
}

function HighlightCard({ title, body }: { title: string; body: string }) {
  return (
    <div className="rounded-md border border-border bg-muted p-5">
      <h3 className="font-serif text-xl font-semibold text-foreground">{title}</h3>
      <p className="mt-3 text-sm leading-7 text-foreground-secondary">{body}</p>
    </div>
  );
}
