import { Link } from "react-router-dom";
import type { ReactNode } from "react";

type WorkspaceToolCardProps = {
  description?: string;
  icon: ReactNode;
  title: string;
  to?: string;
};

export function WorkspaceToolCard({
  description = "Coming soon",
  icon,
  title,
  to
}: WorkspaceToolCardProps) {
  const className = "flex items-center gap-3 rounded-md bg-muted p-3 transition-colors hover:bg-muted/80";

  if (to) {
    return (
      <Link to={to} className={className}>
        <div className="text-foreground-tertiary">{icon}</div>
        <div>
          <p className="text-sm font-medium text-foreground">{title}</p>
          <p className="text-xs text-foreground-tertiary">{description}</p>
        </div>
      </Link>
    );
  }

  return (
    <div className={className}>
      <div className="text-foreground-tertiary">{icon}</div>
      <div>
        <p className="text-sm font-medium text-foreground">{title}</p>
        <p className="text-xs text-foreground-tertiary">{description}</p>
      </div>
    </div>
  );
}
