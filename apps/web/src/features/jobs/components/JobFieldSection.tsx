import type { ReactNode } from "react";
import { SectionCard } from "@/components/SectionCard";

type JobFieldSectionProps = {
  children: ReactNode;
  title: string;
};

export function JobFieldSection({ children, title }: JobFieldSectionProps) {
  return (
    <SectionCard className="p-6">
      <h2 className="font-serif text-2xl font-semibold text-foreground">{title}</h2>
      <div className="mt-5 grid gap-4 md:grid-cols-2">{children}</div>
    </SectionCard>
  );
}
