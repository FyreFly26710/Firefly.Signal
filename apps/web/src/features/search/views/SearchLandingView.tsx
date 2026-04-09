import { useNavigate } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { SearchLandingHero } from "@/features/search/components/SearchLandingHero";
import { SearchQuickLinks } from "@/features/search/components/SearchQuickLinks";
import { SearchUseCases } from "@/features/search/components/SearchUseCases";
import { SearchValueHighlights } from "@/features/search/components/SearchValueHighlights";
import { createSearchPath } from "@/features/search/lib/search-query";

const popularSearches = ["Product Designer", "Frontend Engineer", "Data Scientist", "Product Manager"];
const popularLocations = ["EC2A", "W1F", "E14", "SW1A"];

export function SearchLandingView() {
  const navigate = useNavigate();

  function handleSearch(postcode: string, keyword: string) {
    void navigate(createSearchPath({ keyword, postcode, pageIndex: 0, pageSize: 20 }));
  }

  return (
    <div className="min-h-screen bg-background">
      <AppHeader />

      <div className="relative mx-auto max-w-7xl px-5 pb-12 pt-8 sm:px-8 sm:pt-20">
        <div className="absolute right-0 top-0 -z-10 h-80 w-80 rounded-full bg-accent-secondary/50 blur-3xl" />
        <SearchLandingHero onSearch={handleSearch} />
        <SearchQuickLinks
          popularSearches={popularSearches}
          popularLocations={popularLocations}
          onSearch={handleSearch}
        />
      </div>

      <SearchValueHighlights />
      <SearchUseCases />
    </div>
  );
}
