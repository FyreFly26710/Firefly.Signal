import { AppShell } from "@/components/AppShell";
import { LoginExperience } from "@/features/auth/components/LoginExperience";

export function LoginPage() {
  return (
    <AppShell
      eyebrow="Protected MVP"
      title="Use the live sign-in flow to open the protected MVP workspace."
      subtitle="The screen stays deliberately simple, but it now authenticates against the real identity API and prepares the web app for protected gateway-backed search."
    >
      <LoginExperience />
    </AppShell>
  );
}
