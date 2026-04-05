import { useState } from "react";
import LoginRoundedIcon from "@mui/icons-material/LoginRounded";
import ShieldRoundedIcon from "@mui/icons-material/ShieldRounded";
import TipsAndUpdatesRoundedIcon from "@mui/icons-material/TipsAndUpdatesRounded";
import {
  Alert,
  Button,
  Chip,
  Paper,
  TextField
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import { useSessionStore } from "@/store/session.store";

export function LoginExperience() {
  const navigate = useNavigate();
  const signIn = useSessionStore((state) => state.signIn);
  const [userAccount, setUserAccount] = useState("admin");
  const [password, setPassword] = useState("Admin123!");
  const [userAccountError, setUserAccountError] = useState<string | null>(null);
  const [passwordError, setPasswordError] = useState<string | null>(null);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const normalizedAccount = userAccount.trim();
    const normalizedPassword = password.trim();
    const nextUserAccountError = normalizedAccount ? null : "Enter a user account.";
    const nextPasswordError = normalizedPassword ? null : "Enter a password.";

    setUserAccountError(nextUserAccountError);
    setPasswordError(nextPasswordError);
    setSubmitError(null);

    if (nextUserAccountError || nextPasswordError) {
      return;
    }

    setIsSubmitting(true);

    try {
      await signIn({
        userAccount: normalizedAccount,
        password: normalizedPassword
      });
      void navigate("/app", { replace: true });
    } catch (error) {
      setSubmitError(
        error instanceof Error
          ? error.message
          : "The mock sign-in could not be completed."
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="grid gap-6 xl:grid-cols-[minmax(0,1.15fr)_minmax(360px,440px)]">
      <section className="rounded-[32px] border border-white/60 bg-slate-950 px-6 py-7 text-slate-50 shadow-[0_24px_90px_rgba(15,23,42,0.18)] sm:px-8 sm:py-9">
        <div className="flex flex-wrap items-center gap-3">
          <Chip
            icon={<ShieldRoundedIcon />}
            label="MVP sign-in flow"
            sx={{
              bgcolor: "rgba(16,185,129,0.14)",
              color: "#d1fae5",
              "& .MuiChip-icon": { color: "#6ee7b7" }
            }}
          />
          <Chip
            icon={<TipsAndUpdatesRoundedIcon />}
            label="API wiring comes next"
            sx={{
              bgcolor: "rgba(245,158,11,0.16)",
              color: "#fde68a",
              "& .MuiChip-icon": { color: "#fcd34d" }
            }}
          />
        </div>

        <div className="mt-8 max-w-3xl">
          <p className="text-sm font-semibold uppercase tracking-[0.22em] text-emerald-300">
            Firefly Signal
          </p>
          <h1 className="mt-3 text-4xl font-bold tracking-tight text-white sm:text-5xl">
            Shape the protected app flow before the backend contract takes over.
          </h1>
          <p className="mt-4 max-w-2xl text-base leading-7 text-slate-300 sm:text-lg">
            This first screen is intentionally local-only. It lets us refine the login and
            authenticated workspace experience without blocking on live auth integration.
          </p>
        </div>

        <div className="mt-10 grid gap-4 md:grid-cols-3">
          <HighlightCard
            title="Calm entry"
            body="Make sign-in feel like the start of a focused workflow, not a generic admin panel."
          />
          <HighlightCard
            title="Protected routes"
            body="Keep the app shell and route guard explicit so later mobile clients can mirror the same contract."
          />
          <HighlightCard
            title="Mock confidence"
            body="Use seeded demo access now, then replace the auth action with the real identity API next."
          />
        </div>
      </section>

      <Paper
        className="rounded-[32px] border border-white/70 bg-white/92 p-6 shadow-[0_24px_90px_rgba(15,23,42,0.12)] sm:p-7"
        elevation={0}
      >
        <div>
          <p className="text-sm font-semibold uppercase tracking-[0.2em] text-emerald-800">
            Sign in
          </p>
          <h2 className="mt-2 text-3xl font-bold text-slate-950">Open the MVP workspace.</h2>
          <p className="mt-3 text-sm leading-6 text-slate-600">
            Use one of the current seeded users to preview the protected app flow.
          </p>
        </div>

        <div className="mt-5 rounded-3xl border border-emerald-100 bg-emerald-50/80 p-4 text-sm text-emerald-950">
          <p className="font-semibold">Mock credentials</p>
          <p className="mt-2">`admin` / `Admin123!`</p>
          <p>`analyst` / `Analyst123!`</p>
        </div>

        <form
          className="mt-6 flex flex-col gap-4"
          onSubmit={(event) => {
            void handleSubmit(event);
          }}
        >
          <TextField
            label="User account"
            name="userAccount"
            value={userAccount}
            onChange={(event) => setUserAccount(event.target.value)}
            error={Boolean(userAccountError)}
            helperText={userAccountError ?? "Use a seeded local account for this pass."}
            autoComplete="username"
            fullWidth
          />

          <TextField
            label="Password"
            name="password"
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            error={Boolean(passwordError)}
            helperText={passwordError ?? "This screen stays local-only until API wiring lands."}
            autoComplete="current-password"
            fullWidth
          />

          {submitError ? (
            <Alert severity="error" variant="filled">
              {submitError}
            </Alert>
          ) : null}

          <Button
            type="submit"
            variant="contained"
            size="large"
            disabled={isSubmitting}
            startIcon={<LoginRoundedIcon />}
            sx={{
              mt: 1,
              minHeight: 54,
              bgcolor: "brand.main",
              "&:hover": {
                bgcolor: "brand.dark"
              }
            }}
          >
            {isSubmitting ? "Opening workspace..." : "Continue to app"}
          </Button>
        </form>
      </Paper>
    </div>
  );
}

function HighlightCard({ title, body }: { title: string; body: string }) {
  return (
    <div className="rounded-[28px] border border-white/10 bg-white/[0.05] p-5">
      <h3 className="text-lg font-semibold text-white">{title}</h3>
      <p className="mt-2 text-sm leading-6 text-slate-300">{body}</p>
    </div>
  );
}
