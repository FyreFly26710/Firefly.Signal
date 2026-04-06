import type { ReactNode } from "react";

type WorkspaceToolCardProps = {
  icon: ReactNode;
  title: string;
};

export function WorkspaceToolCard({ icon, title }: WorkspaceToolCardProps) {
  return (
    <div className="flex items-center gap-3 rounded-md bg-muted p-3">
      <div className="text-foreground-tertiary">{icon}</div>
      <div>
        <p className="text-sm font-medium text-foreground">{title}</p>
        <p className="text-xs text-foreground-tertiary">Coming soon</p>
      </div>
    </div>
  );
}
