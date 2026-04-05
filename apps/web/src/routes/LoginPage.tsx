import { AppShell } from "@/components/AppShell";
import { LoginExperience } from "@/features/auth/components/LoginExperience";

export function LoginPage() {
  return (
    <AppShell
      eyebrow="Protected MVP"
      title="Create the signed-in feel before the live integration arrives."
      subtitle="This pass focuses on the web UI only: login, route protection, and the first authenticated workspace shape. Backend wiring comes after the flow feels right."
    >
      <LoginExperience />
    </AppShell>
  );
}
