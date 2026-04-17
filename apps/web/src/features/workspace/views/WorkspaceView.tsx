import { AppHeader } from "@/components/AppHeader";
import { WorkspaceAppliedJobsSection } from "@/features/workspace/components/WorkspaceAppliedJobsSection";
import { WorkspaceHeaderSection } from "@/features/workspace/components/WorkspaceHeaderSection";

export function WorkspaceView() {
  return (
    <div className="min-h-screen bg-background">
      <AppHeader variant="authenticated" />

      <div className="mx-auto max-w-7xl px-5 py-8 sm:px-8">
        <WorkspaceHeaderSection
          title="Your workspace"
          description="Manage active applications first, then return to live discovery whenever you want to add something new."
        />
        <div className="space-y-8">
          <WorkspaceAppliedJobsSection />
        </div>
      </div>
    </div>
  );
}
