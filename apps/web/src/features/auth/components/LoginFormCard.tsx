import { Alert, Button, TextField } from "@mui/material";
import { SectionCard } from "@/components/SectionCard";

type LoginFormCardProps = {
  userAccount: string;
  password: string;
  submitError: string | null;
  isSubmitting: boolean;
  onUserAccountChange: (value: string) => void;
  onPasswordChange: (value: string) => void;
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
};

export function LoginFormCard({
  userAccount,
  password,
  submitError,
  isSubmitting,
  onUserAccountChange,
  onPasswordChange,
  onSubmit
}: LoginFormCardProps) {
  return (
    <SectionCard className="border-border-strong p-8 shadow-sm">
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

      <form className="mt-6 flex flex-col gap-4" onSubmit={(event) => void onSubmit(event)}>
        <TextField
          label="User account"
          value={userAccount}
          onChange={(event) => onUserAccountChange(event.target.value)}
          autoComplete="username"
          fullWidth
        />

        <TextField
          label="Password"
          type="password"
          value={password}
          onChange={(event) => onPasswordChange(event.target.value)}
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
    </SectionCard>
  );
}
