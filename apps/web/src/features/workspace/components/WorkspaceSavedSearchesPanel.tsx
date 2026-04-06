import { SectionCard } from "@/components/SectionCard";

type SavedSearch = {
  id: number;
  query: string;
  count: number;
  freshCount: number;
};

type WorkspaceSavedSearchesPanelProps = {
  searches: SavedSearch[];
  onSelectSearch: (query: string) => void;
};

export function WorkspaceSavedSearchesPanel({
  searches,
  onSelectSearch
}: WorkspaceSavedSearchesPanelProps) {
  return (
    <SectionCard className="p-6">
      <h2 className="font-serif text-xl font-semibold text-foreground">Saved searches</h2>
      <div className="mt-4 space-y-3">
        {searches.map((search) => (
          <button
            key={search.id}
            type="button"
            className="w-full rounded-md bg-muted p-3 text-left transition-colors hover:bg-accent-secondary"
            onClick={() => onSelectSearch(search.query)}
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
    </SectionCard>
  );
}
