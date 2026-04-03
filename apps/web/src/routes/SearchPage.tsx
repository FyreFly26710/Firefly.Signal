import { AppShell } from "@/components/AppShell";
import { SearchExperience } from "@/features/search/components/SearchExperience";

export function SearchPage() {
  return (
    <AppShell
      eyebrow="Firefly Signal"
      title="Search UK jobs without starting from scratch every time."
      subtitle="Begin with postcode and keyword, review fast-loading results, and keep the structure ready for saved searches, AI filters, and later mobile clients."
    >
      <SearchExperience />
    </AppShell>
  );
}
