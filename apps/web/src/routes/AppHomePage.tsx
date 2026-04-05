import { AppShell } from "@/components/AppShell";
import { WorkspaceExperience } from "@/features/workspace/components/WorkspaceExperience";

export function AppHomePage() {
  return (
    <AppShell
      eyebrow="Workspace"
      title="Protected search, clear status, and a calmer place to continue."
      subtitle="The current app shell is intentionally API-free. It gives us the signed-in interaction model, page rhythm, and search states before we connect the real backend."
    >
      <WorkspaceExperience />
    </AppShell>
  );
}
