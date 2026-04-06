import { SearchForm } from "@/features/search/components/SearchForm";

type SearchLandingHeroProps = {
  onSearch: (postcode: string, keyword: string) => void;
};

export function SearchLandingHero({ onSearch }: SearchLandingHeroProps) {
  return (
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
        <SearchForm onSubmit={onSearch} />
      </div>
    </div>
  );
}
