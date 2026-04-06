import type { ReactNode } from "react";
import { SectionCard } from "@/components/SectionCard";

type JobDetailContentPanelProps = {
  title: string;
  children: ReactNode;
};

export function JobDetailContentPanel({ title, children }: JobDetailContentPanelProps) {
  return (
    <SectionCard className="p-8">
      <h2 className="font-serif text-2xl font-semibold text-foreground">{title}</h2>
      <div className="mt-4 text-sm leading-7">{children}</div>
    </SectionCard>
  );
}
