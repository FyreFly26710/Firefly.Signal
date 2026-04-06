import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { LoginFormCard } from "@/features/auth/components/LoginFormCard";
import { LoginHighlightsSection } from "@/features/auth/components/LoginHighlightsSection";
import { useSessionStore } from "@/store/session.store";

type LocationState = {
  from?: string;
};

export function LoginView() {
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
        <LoginHighlightsSection />
        <LoginFormCard
          userAccount={userAccount}
          password={password}
          submitError={submitError}
          isSubmitting={isSubmitting}
          onUserAccountChange={setUserAccount}
          onPasswordChange={setPassword}
          onSubmit={(event) => void handleSubmit(event)}
        />
      </div>
    </div>
  );
}
