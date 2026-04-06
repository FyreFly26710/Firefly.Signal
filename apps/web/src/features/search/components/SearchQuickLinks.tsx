type SearchQuickLinksProps = {
  popularSearches: string[];
  popularLocations: string[];
  onSearch: (postcode: string, keyword: string) => void;
};

export function SearchQuickLinks({
  popularSearches,
  popularLocations,
  onSearch
}: SearchQuickLinksProps) {
  return (
    <div className="mt-6 grid gap-6 border-t border-divider pt-6 sm:grid-cols-2">
      <QuickLinkSection
        title="POPULAR SEARCHES"
        items={popularSearches}
        onClick={(item) => onSearch("", item)}
      />
      <QuickLinkSection
        title="POPULAR LOCATIONS"
        items={popularLocations}
        onClick={(item) => onSearch(item, "")}
      />
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
