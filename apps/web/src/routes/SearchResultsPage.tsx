import { useLocation } from "react-router-dom";
import { SearchResultsView } from "@/features/search/views/SearchResultsView";

export function SearchResultsPage() {
  const location = useLocation();
  return <SearchResultsView key={location.search} />;
}
