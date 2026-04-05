import { AppShell } from "@/components/AppShell";
import { WorkspaceExperience } from "@/features/workspace/components/WorkspaceExperience";

export function AppHomePage() {
  return (
    <AppShell
      eyebrow="Workspace"
      title="Protected search, clear status, and a calmer place to continue."
      subtitle="The signed-in shell now runs on the real auth and search APIs through the gateway while preserving the same focused page rhythm."
    >
      <WorkspaceExperience />
    </AppShell>
  );
}
