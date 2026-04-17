import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { AppHeader } from "@/components/AppHeader";
import { SectionCard } from "@/components/SectionCard";
import { createSearchPath } from "@/features/search/lib/search-query";
import { WorkspaceAppliedJobsSection } from "@/features/workspace/components/WorkspaceAppliedJobsSection";
import { WorkspaceHeaderSection } from "@/features/workspace/components/WorkspaceHeaderSection";
import { WorkspaceQuickSearch } from "@/features/workspace/components/WorkspaceQuickSearch";

export function WorkspaceView() {
  const navigate = useNavigate();
  const [keyword, setKeyword] = useState("");

  function handleSearch() {
    if (!keyword.trim()) {
      return;
    }

    void navigate(createSearchPath({ keyword, where: "", salaryMin: null, salaryMax: null, datePosted: null, sortBy: "date", isAsc: false, pageIndex: 0, pageSize: 20 }));
  }

  return (
    <div className="min-h-screen bg-background">
      <AppHeader variant="authenticated" />

      <div className="mx-auto max-w-7xl px-5 py-8 sm:px-8">
        <WorkspaceHeaderSection
          title="Your workspace"
          description="Manage active applications first, then return to live discovery whenever you want to add something new."
        />
        <WorkspaceQuickSearch
          keyword={keyword}
          onKeywordChange={setKeyword}
          onSearch={handleSearch}
        />
        <div className="space-y-8">
          <WorkspaceAppliedJobsSection />
          <SectionCard className="border-dashed p-6">
            <p className="font-mono text-xs uppercase tracking-[0.18em] text-metadata">Future sections</p>
            <h2 className="mt-3 font-serif text-2xl font-semibold text-foreground">More workspace sections can grow here</h2>
            <p className="mt-3 max-w-3xl text-sm leading-7 text-foreground-secondary">
              This layout intentionally leaves room for saved searches, reminders, interview prep, and other follow-on sections without rewriting the workspace shell again.
            </p>
          </SectionCard>
        </div>
      </div>
    </div>
  );
}
