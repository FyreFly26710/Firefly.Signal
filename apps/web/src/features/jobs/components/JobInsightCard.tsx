import { SectionCard } from "@/components/SectionCard";

export function JobInsightCard() {
  return (
    <SectionCard className="sticky top-24 border-accent-secondary bg-accent-secondary p-6">
      <p className="font-mono text-xs tracking-[0.18em] text-accent-secondary-foreground/70">
        AI INSIGHT PREVIEW
      </p>
      <h2 className="mt-3 font-serif text-2xl font-semibold text-accent-secondary-foreground">
        Strong fit signal
      </h2>
      <p className="mt-3 text-sm leading-7 text-accent-secondary-foreground">
        The mock design reserves space for future guidance like fit notes, company signals,
        interview prep, and resume alignment without making the MVP dependent on AI.
      </p>
    </SectionCard>
  );
}
