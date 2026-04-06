import type { ReactNode } from "react";
import { SectionCard } from "@/components/SectionCard";

type JobDetailListPanelProps = {
  title: string;
  items: string[];
  icon: ReactNode;
};

export function JobDetailListPanel({ title, items, icon }: JobDetailListPanelProps) {
  return (
    <SectionCard className="p-8">
      <h2 className="font-serif text-2xl font-semibold text-foreground">{title}</h2>
      <ul className="mt-5 space-y-3">
        {items.map((item) => (
          <li key={item} className="flex gap-3 text-sm leading-7 text-foreground-secondary">
            <span className="mt-0.5">{icon}</span>
            <span>{item}</span>
          </li>
        ))}
      </ul>
    </SectionCard>
  );
}
