import { SectionCard } from "@/components/SectionCard";

type PipelineItem = {
  stage: string;
  count: number;
};

type WorkspacePipelinePanelProps = {
  items: PipelineItem[];
};

export function WorkspacePipelinePanel({ items }: WorkspacePipelinePanelProps) {
  return (
    <SectionCard className="p-6">
      <h2 className="font-serif text-xl font-semibold text-foreground">Pipeline</h2>
      <div className="mt-4 space-y-3">
        {items.map((item) => (
          <div key={item.stage} className="flex items-center justify-between text-sm">
            <span className="text-foreground-secondary">{item.stage}</span>
            <span className="font-mono font-medium text-foreground">{item.count}</span>
          </div>
        ))}
      </div>
    </SectionCard>
  );
}
