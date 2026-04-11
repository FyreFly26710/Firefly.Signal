import { SectionCard } from "@/components/SectionCard";

export function JobInsightCard() {
  return (
    <SectionCard className="sticky top-24 border-accent-secondary bg-accent-secondary p-6">
      <p className="font-mono text-xs tracking-[0.18em] text-accent-secondary-foreground/70">
        APPLICATION SOURCE
      </p>
      <h2 className="mt-3 font-serif text-2xl font-semibold text-accent-secondary-foreground">
        Continue on the original listing
      </h2>
      <p className="mt-3 text-sm leading-7 text-accent-secondary-foreground">
        Review the role details here, then use the source link above to apply through the original
        job posting.
      </p>
    </SectionCard>
  );
}
