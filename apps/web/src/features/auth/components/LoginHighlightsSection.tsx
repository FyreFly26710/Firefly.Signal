import { SectionCard } from "@/components/SectionCard";
import { LoginHighlightsPanel } from "@/features/auth/components/LoginHighlightsPanel";

export function LoginHighlightsSection() {
  return (
    <SectionCard className="px-8 py-10">
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
        <LoginHighlightsPanel
          title="Real auth"
          description="Keep the existing JWT-backed login path so protected flows still behave like the real app."
        />
        <LoginHighlightsPanel
          title="Mock discovery"
          description="The signed-in workspace and job search experience use mock data only for this UI pass."
        />
        <LoginHighlightsPanel
          title="Future-ready"
          description="The layout leaves room for saved searches, job tracking, resume workflows, and AI support."
        />
      </div>
    </SectionCard>
  );
}
