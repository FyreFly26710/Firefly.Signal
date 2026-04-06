import { WorkspacePipelinePanel } from "@/features/workspace/components/WorkspacePipelinePanel";
import { WorkspaceSavedSearchesPanel } from "@/features/workspace/components/WorkspaceSavedSearchesPanel";
import { WorkspaceToolsPanel } from "@/features/workspace/components/WorkspaceToolsPanel";

type SavedSearch = {
  id: number;
  query: string;
  count: number;
  freshCount: number;
};

type PipelineItem = {
  stage: string;
  count: number;
};

type WorkspaceSidebarProps = {
  searches: SavedSearch[];
  pipeline: PipelineItem[];
  onSelectSearch: (query: string) => void;
};

export function WorkspaceSidebar({
  searches,
  pipeline,
  onSelectSearch
}: WorkspaceSidebarProps) {
  return (
    <aside className="space-y-6">
      <WorkspaceSavedSearchesPanel searches={searches} onSelectSearch={onSelectSearch} />
      <WorkspacePipelinePanel items={pipeline} />
      <WorkspaceToolsPanel />
    </aside>
  );
}
