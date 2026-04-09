import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { mockJobs } from "@/features/jobs/data/mockJobs";
import { createSearchPath } from "@/features/search/lib/search-query";
import { WorkspaceHeaderSection } from "@/features/workspace/components/WorkspaceHeaderSection";
import { WorkspaceQuickSearch } from "@/features/workspace/components/WorkspaceQuickSearch";
import { WorkspaceRecentActivity } from "@/features/workspace/components/WorkspaceRecentActivity";
import { WorkspaceSidebar } from "@/features/workspace/components/WorkspaceSidebar";
import { WorkspaceStatsGrid } from "@/features/workspace/components/WorkspaceStatsGrid";

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

export function WorkspaceView() {
  const navigate = useNavigate();
  const [keyword, setKeyword] = useState("");

  function handleSearch() {
    if (!keyword.trim()) {
      return;
    }

    void navigate(createSearchPath({ keyword, postcode: "", pageIndex: 0, pageSize: 20 }));
  }

  function handleSelectSearch(query: string) {
    void navigate(createSearchPath({ keyword: query, postcode: "", pageIndex: 0, pageSize: 20 }));
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
        <WorkspaceStatsGrid />

        <div className="grid gap-8 lg:grid-cols-[320px_minmax(0,1fr)]">
          <WorkspaceSidebar
            searches={savedSearches}
            pipeline={pipeline}
            onSelectSearch={handleSelectSearch}
          />
          <WorkspaceRecentActivity jobs={mockJobs.slice(0, 3)} />
        </div>
      </div>
    </div>
  );
}
