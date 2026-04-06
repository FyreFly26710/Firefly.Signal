import AutoGraphRoundedIcon from "@mui/icons-material/AutoGraphRounded";
import MyLocationRoundedIcon from "@mui/icons-material/MyLocationRounded";
import WorkOutlineRoundedIcon from "@mui/icons-material/WorkOutlineRounded";
import type { ReactNode } from "react";

export function SearchUseCases() {
  return (
    <section className="mx-auto max-w-7xl px-5 py-16 sm:px-8">
      <div className="max-w-3xl">
        <h2 className="font-serif text-4xl font-semibold text-foreground">
          Built for how you actually search.
        </h2>
        <p className="mt-4 text-lg text-foreground-secondary">
          Whether you are actively looking or passively curious, the product is shaped for quick
          scanning now and deeper career workflows later.
        </p>
      </div>

      <div className="mt-10 grid gap-6">
        <UseCaseCard
          icon={<MyLocationRoundedIcon />}
          title="Location-first search"
          body="Start with a postcode or area to find everything nearby, then narrow down only when the market starts to look interesting."
          caption="Example: EC2A -> all roles"
        />
        <UseCaseCard
          icon={<WorkOutlineRoundedIcon />}
          title="Role-focused search"
          body="Search by role, skill, or company when the role matters more than the district."
          caption="Example: Product Designer -> London"
        />
        <UseCaseCard
          icon={<AutoGraphRoundedIcon />}
          title="Passive monitoring"
          body="This UI direction also supports future saved searches and notifications without changing the product's visual identity."
          caption="Coming soon: saved searches and alerts"
        />
      </div>
    </section>
  );
}

function UseCaseCard({
  icon,
  title,
  body,
  caption
}: {
  icon: ReactNode;
  title: string;
  body: string;
  caption: string;
}) {
  return (
    <article className="rounded-lg border border-border bg-background-elevated p-8 transition-colors hover:border-border-strong">
      <div className="flex gap-6">
        <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded bg-accent-secondary text-accent-secondary-foreground">
          {icon}
        </div>
        <div>
          <h3 className="font-serif text-xl font-semibold text-foreground">{title}</h3>
          <p className="mt-3 leading-7 text-foreground-secondary">{body}</p>
          <p className="mt-3 font-mono text-sm text-accent-primary">{caption}</p>
        </div>
      </div>
    </article>
  );
}
