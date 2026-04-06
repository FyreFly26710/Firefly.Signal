import AutoGraphRoundedIcon from "@mui/icons-material/AutoGraphRounded";
import BoltRoundedIcon from "@mui/icons-material/BoltRounded";
import WorkOutlineRoundedIcon from "@mui/icons-material/WorkOutlineRounded";
import type { ReactNode } from "react";

export function SearchValueHighlights() {
  return (
    <section className="border-y border-divider bg-muted py-16">
      <div className="mx-auto grid max-w-7xl gap-12 px-5 sm:px-8 lg:grid-cols-3">
        <ValueCard
          icon={<WorkOutlineRoundedIcon />}
          title="Focused discovery"
          body="Search across multiple job sources with a single query. Results stay grounded in a clear, readable browsing flow."
        />
        <ValueCard
          icon={<BoltRoundedIcon />}
          title="Real-time signals"
          body="Highlight fresh roles quickly and keep the interface geared toward quick judgment instead of noisy dashboards."
        />
        <ValueCard
          icon={<AutoGraphRoundedIcon />}
          title="Career intelligence"
          body="The layout leaves room for saved searches, tracking, resume fit, and interview prep without overloading the MVP."
        />
      </div>
    </section>
  );
}

function ValueCard({
  icon,
  title,
  body
}: {
  icon: ReactNode;
  title: string;
  body: string;
}) {
  return (
    <div>
      <div className="mb-4 flex h-10 w-10 items-center justify-center rounded bg-accent-primary text-accent-primary-foreground">
        {icon}
      </div>
      <h3 className="font-serif text-xl font-semibold text-foreground">{title}</h3>
      <p className="mt-3 text-foreground-secondary">{body}</p>
    </div>
  );
}
