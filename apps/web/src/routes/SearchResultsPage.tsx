import { useLocation } from "react-router-dom";
import { SearchResultsExperience } from "@/features/search/components/SearchResultsExperience";

export function SearchResultsPage() {
  const location = useLocation();
  return <SearchResultsExperience key={location.search} />;
}
