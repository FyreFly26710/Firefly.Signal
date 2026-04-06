const filterGroups = [
  {
    title: "Posted",
    options: ["Last 24 hours", "Last 3 days", "Last week", "Anytime"]
  },
  {
    title: "Job type",
    options: ["Full-time", "Part-time", "Contract", "Freelance"]
  },
  {
    title: "Salary",
    options: ["GBP 40k+", "GBP 60k+", "GBP 80k+", "GBP 100k+"]
  },
  {
    title: "Source",
    options: ["LinkedIn", "Indeed", "Company Site", "Reed"]
  }
] as const;

export function SearchFiltersPanel() {
  return (
    <aside className="space-y-6">
      <div>
        <h3 className="mb-3 font-mono text-sm text-metadata">FILTERS</h3>
        <div className="space-y-4">
          {filterGroups.map((group) => (
            <FilterGroup key={group.title} title={group.title} options={group.options} />
          ))}
        </div>
      </div>

      <div className="rounded-lg bg-accent-secondary p-4">
        <p className="text-sm font-medium text-accent-secondary-foreground">Save this search</p>
        <p className="mt-2 text-xs text-accent-secondary-foreground/80">
          This mock layout leaves room for saved searches and alerts without requiring them yet.
        </p>
      </div>
    </aside>
  );
}

function FilterGroup({ title, options }: { title: string; options: readonly string[] }) {
  return (
    <div className="border-t border-divider pt-4 first:border-t-0 first:pt-0">
      <p className="mb-2 text-sm font-medium text-foreground">{title}</p>
      <div className="space-y-2">
        {options.map((option) => (
          <label
            key={option}
            className="flex items-center gap-2 text-sm text-foreground-secondary transition-colors hover:text-foreground"
          >
            <input type="checkbox" className="rounded border-input-border" />
            {option}
          </label>
        ))}
      </div>
    </div>
  );
}
