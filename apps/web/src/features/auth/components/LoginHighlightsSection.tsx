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
          Access your search workspace, continue exploring live job listings, and keep your next
          search within easy reach.
        </p>
      </div>

      <div className="mt-10 grid gap-4 md:grid-cols-3">
        <LoginHighlightsPanel
          title="Secure access"
          description="Use the existing JWT-backed sign-in flow to open protected parts of the app."
        />
        <LoginHighlightsPanel
          title="Focused search"
          description="Jump back into job discovery quickly with a workspace built around search and review."
        />
        <LoginHighlightsPanel
          title="Clear workflow"
          description="The current release keeps the signed-in experience lean and ready for day-to-day use."
        />
      </div>
    </SectionCard>
  );
}
