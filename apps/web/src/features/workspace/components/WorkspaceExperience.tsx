import AutoGraphRoundedIcon from "@mui/icons-material/AutoGraphRounded";
import BookmarkRoundedIcon from "@mui/icons-material/BookmarkRounded";
import DescriptionRoundedIcon from "@mui/icons-material/DescriptionRounded";
import ScheduleRoundedIcon from "@mui/icons-material/ScheduleRounded";
import SearchRoundedIcon from "@mui/icons-material/SearchRounded";
import WorkspacesRoundedIcon from "@mui/icons-material/WorkspacesRounded";
import { Button } from "@mui/material";
import type { ReactNode } from "react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { JobCard } from "@/features/jobs/components/JobCard";
import { mockJobs } from "@/features/jobs/data/mockJobs";
import { SearchInput } from "@/features/search/components/SearchInput";

const savedSearches = [
  { id: 1, query: "Product Designer in EC2A", count: 24, freshCount: 3 },
  { id: 2, query: "Frontend Engineer London", count: 156, freshCount: 12 },
  { id: 3, query: "Data Scientist", count: 89, freshCount: 5 }
];

const pipeline = [
  { stage: "Interested", count: 12 },
  { stage: "Applied", count: 8 },
  { stage: "Interviewing", count: 3 },
  { stage: "Offer", count: 1 }
];

export function WorkspaceExperience() {
  const navigate = useNavigate();
  const [keyword, setKeyword] = useState("");

  function handleSearch() {
    if (!keyword.trim()) {
      return;
    }

    void navigate(`/search?keyword=${encodeURIComponent(keyword.trim())}`);
  }

  return (
    <div className="min-h-screen bg-background">
      <AppHeader variant="authenticated" />

      <div className="mx-auto max-w-7xl px-5 py-8 sm:px-8">
        <section className="mb-8">
          <h1 className="font-serif text-4xl font-semibold text-foreground">Your workspace</h1>
          <p className="mt-3 text-lg text-foreground-secondary">
            Track your search, manage applications, and keep the next useful opportunity within reach.
          </p>
        </section>

        <section className="mb-8 rounded-lg border border-border bg-background-elevated p-6">
          <div className="flex flex-col gap-3 lg:flex-row">
            <div className="flex-1">
              <SearchInput
                placeholder="Quick search: roles, companies, skills..."
                value={keyword}
                onChange={setKeyword}
                onSubmit={handleSearch}
                large
              />
            </div>
            <Button
              variant="contained"
              onClick={handleSearch}
              sx={{
                minWidth: 132,
                bgcolor: "accent.main",
                "&:hover": { bgcolor: "accent.dark" }
              }}
              startIcon={<SearchRoundedIcon />}
            >
              Search
            </Button>
          </div>
        </section>

        <section className="mb-8 grid gap-6 md:grid-cols-2 xl:grid-cols-4">
          <StatCard icon={<AutoGraphRoundedIcon />} label="New opportunities" value="147" meta="Last 7 days" />
          <StatCard icon={<BookmarkRoundedIcon />} label="Saved jobs" value="23" meta="Total" />
          <StatCard icon={<WorkspacesRoundedIcon />} label="Saved searches" value="3" meta="Active" />
          <StatCard icon={<DescriptionRoundedIcon />} label="Applications" value="11" meta="In progress" />
        </section>

        <div className="grid gap-8 lg:grid-cols-[320px_minmax(0,1fr)]">
          <aside className="space-y-6">
            <section className="rounded-lg border border-border bg-background-elevated p-6">
              <h2 className="font-serif text-xl font-semibold text-foreground">Saved searches</h2>
              <div className="mt-4 space-y-3">
                {savedSearches.map((search) => (
                  <button
                    key={search.id}
                    type="button"
                    className="w-full rounded-md bg-muted p-3 text-left transition-colors hover:bg-accent-secondary"
                    onClick={() => {
                      void navigate(`/search?keyword=${encodeURIComponent(search.query)}`);
                    }}
                  >
                    <div className="flex items-start justify-between gap-2">
                      <p className="text-sm font-medium text-foreground">{search.query}</p>
                      <span className="rounded bg-signal-fresh px-1.5 py-0.5 text-xs font-medium text-white">
                        {search.freshCount}
                      </span>
                    </div>
                    <p className="mt-1 text-xs text-foreground-tertiary">{search.count} total results</p>
                  </button>
                ))}
              </div>
            </section>

            <section className="rounded-lg border border-border bg-background-elevated p-6">
              <h2 className="font-serif text-xl font-semibold text-foreground">Pipeline</h2>
              <div className="mt-4 space-y-3">
                {pipeline.map((item) => (
                  <div key={item.stage} className="flex items-center justify-between text-sm">
                    <span className="text-foreground-secondary">{item.stage}</span>
                    <span className="font-mono font-medium text-foreground">{item.count}</span>
                  </div>
                ))}
              </div>
            </section>

            <section className="rounded-lg border border-border bg-background-elevated p-6">
              <h2 className="font-serif text-xl font-semibold text-foreground">Career tools</h2>
              <div className="mt-4 space-y-3">
                <ToolCard icon={<DescriptionRoundedIcon />} title="Resume analysis" />
                <ToolCard icon={<ScheduleRoundedIcon />} title="Interview prep" />
              </div>
            </section>
          </aside>

          <main>
            <div className="mb-4 flex items-center justify-between gap-3">
              <h2 className="font-serif text-2xl font-semibold text-foreground">Recent activity</h2>
              <div className="inline-flex items-center gap-2 text-sm text-foreground-secondary">
                <ScheduleRoundedIcon sx={{ fontSize: 18 }} />
                Last updated 5m ago
              </div>
            </div>

            <div className="overflow-hidden rounded-lg border border-border bg-background-elevated">
              {mockJobs.slice(0, 3).map((job) => (
                <JobCard key={job.id} job={job} />
              ))}
            </div>
          </main>
        </div>
      </div>
    </div>
  );
}

function StatCard({ icon, label, value, meta }: { icon: ReactNode; label: string; value: string; meta: string }) {
  return (
    <article className="rounded-lg border border-border bg-background-elevated p-6">
      <div className="mb-3 flex items-center justify-between">
        <div className="flex h-10 w-10 items-center justify-center rounded bg-muted text-accent-primary">{icon}</div>
        <span className="font-mono text-xs uppercase tracking-[0.14em] text-metadata">{meta}</span>
      </div>
      <p className="font-serif text-3xl font-semibold text-foreground">{value}</p>
      <p className="mt-1 text-sm text-foreground-secondary">{label}</p>
    </article>
  );
}

function ToolCard({ icon, title }: { icon: ReactNode; title: string }) {
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
