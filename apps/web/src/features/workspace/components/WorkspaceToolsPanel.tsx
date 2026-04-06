import DescriptionRoundedIcon from "@mui/icons-material/DescriptionRounded";
import ScheduleRoundedIcon from "@mui/icons-material/ScheduleRounded";
import { SectionCard } from "@/components/SectionCard";
import { WorkspaceToolCard } from "@/features/workspace/components/WorkspaceToolCard";

export function WorkspaceToolsPanel() {
  return (
    <SectionCard className="p-6">
      <h2 className="font-serif text-xl font-semibold text-foreground">Career tools</h2>
      <div className="mt-4 space-y-3">
        <WorkspaceToolCard icon={<DescriptionRoundedIcon />} title="Resume analysis" />
        <WorkspaceToolCard icon={<ScheduleRoundedIcon />} title="Interview prep" />
      </div>
    </SectionCard>
  );
}
