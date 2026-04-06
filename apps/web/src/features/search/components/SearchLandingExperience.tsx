import AutoGraphRoundedIcon from "@mui/icons-material/AutoGraphRounded";
import BoltRoundedIcon from "@mui/icons-material/BoltRounded";
import MyLocationRoundedIcon from "@mui/icons-material/MyLocationRounded";
import WorkOutlineRoundedIcon from "@mui/icons-material/WorkOutlineRounded";
import type { ReactNode } from "react";
import { useNavigate } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { SearchForm } from "@/features/search/components/SearchForm";

const popularSearches = ["Product Designer", "Frontend Engineer", "Data Scientist", "Product Manager"];
const popularLocations = ["EC2A", "W1F", "E14", "SW1A"];

export function SearchLandingExperience() {
  const navigate = useNavigate();

  function handleSearch(postcode: string, keyword: string) {
    const params = new URLSearchParams();

    if (keyword) {
      params.set("keyword", keyword);
    }

    if (postcode) {
      params.set("postcode", postcode);
    }

    void navigate(`/search?${params.toString()}`);
  }

  return (
    <div className="min-h-screen bg-background">
      <AppHeader />

      <div className="relative mx-auto max-w-7xl px-5 pb-12 pt-16 sm:px-8 sm:pt-20">
        <div className="absolute right-0 top-0 -z-10 h-80 w-80 rounded-full bg-accent-secondary/50 blur-3xl" />

        <div className="max-w-3xl">
          <div className="mb-6 flex items-center gap-2">
            <div className="h-px w-12 bg-accent-primary" />
            <span className="font-mono text-sm tracking-[0.18em] text-accent-primary">
              CAREER INTELLIGENCE
            </span>
          </div>

          <h1 className="font-serif text-5xl font-semibold leading-tight text-foreground sm:text-6xl">
            Discover your next role with precision and clarity.
          </h1>

          <p className="mt-6 max-w-2xl text-xl leading-8 text-foreground-secondary">
            Firefly Signal aggregates UK job opportunities from across the web, delivering focused
            search results tailored to your career goals. No noise. Just signal.
          </p>

          <div className="mt-12">
            <SearchForm onSubmit={handleSearch} />
          </div>

          <div className="mt-6 grid gap-6 border-t border-divider pt-6 sm:grid-cols-2">
            <QuickLinkSection
              title="POPULAR SEARCHES"
              items={popularSearches}
              onClick={(item) => handleSearch("", item)}
            />
            <QuickLinkSection
              title="POPULAR LOCATIONS"
              items={popularLocations}
              onClick={(item) => handleSearch(item, "")}
            />
          </div>
        </div>
      </div>

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
    </div>
  );
}

function QuickLinkSection({
  title,
  items,
  onClick
}: {
  title: string;
  items: string[];
  onClick: (item: string) => void;
}) {
  return (
    <div>
      <p className="mb-3 font-mono text-xs text-metadata">{title}</p>
      <div className="flex flex-wrap gap-3">
        {items.map((item) => (
          <button
            key={item}
            className="text-sm text-foreground-secondary transition-colors hover:text-accent-primary"
            onClick={() => onClick(item)}
            type="button"
          >
            {item}
          </button>
        ))}
      </div>
    </div>
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
