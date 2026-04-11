import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { SectionCard } from "@/components/SectionCard";
import { createSearchPath } from "@/features/search/lib/search-query";
import { WorkspaceHeaderSection } from "@/features/workspace/components/WorkspaceHeaderSection";
import { WorkspaceQuickSearch } from "@/features/workspace/components/WorkspaceQuickSearch";

export function WorkspaceView() {
  const navigate = useNavigate();
  const [keyword, setKeyword] = useState("");

  function handleSearch() {
    if (!keyword.trim()) {
      return;
    }

    void navigate(createSearchPath({ keyword, postcode: "", pageIndex: 0, pageSize: 20 }));
  }

  return (
    <div className="min-h-screen bg-background">
      <AppHeader variant="authenticated" />

      <div className="mx-auto max-w-7xl px-5 py-8 sm:px-8">
        <WorkspaceHeaderSection
          title="Your workspace"
          description="Track your search, manage applications, and keep the next useful opportunity within reach."
        />
        <WorkspaceQuickSearch
          keyword={keyword}
          onKeywordChange={setKeyword}
          onSearch={handleSearch}
        />
        <SectionCard className="p-6">
          <h2 className="font-serif text-2xl font-semibold text-foreground">Search from your workspace</h2>
          <p className="mt-3 max-w-3xl text-sm leading-7 text-foreground-secondary">
            Use the quick search above to return to live job discovery. Saved searches, activity
            tracking, and application management are not part of this release.
          </p>
        </SectionCard>
      </div>
    </div>
  );
}
