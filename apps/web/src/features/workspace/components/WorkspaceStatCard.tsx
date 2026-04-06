import type { ReactNode } from "react";
import { SectionCard } from "@/components/SectionCard";

type WorkspaceStatCardProps = {
  icon: ReactNode;
  label: string;
  value: string;
  meta: string;
};

export function WorkspaceStatCard({ icon, label, value, meta }: WorkspaceStatCardProps) {
  return (
    <SectionCard className="p-6">
      <div className="mb-3 flex items-center justify-between">
        <div className="flex h-10 w-10 items-center justify-center rounded bg-muted text-accent-primary">{icon}</div>
        <span className="font-mono text-xs uppercase tracking-[0.14em] text-metadata">{meta}</span>
      </div>
      <p className="font-serif text-3xl font-semibold text-foreground">{value}</p>
      <p className="mt-1 text-sm text-foreground-secondary">{label}</p>
    </SectionCard>
  );
}
