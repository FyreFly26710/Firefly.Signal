import AutoGraphRoundedIcon from "@mui/icons-material/AutoGraphRounded";
import BookmarkRoundedIcon from "@mui/icons-material/BookmarkRounded";
import DescriptionRoundedIcon from "@mui/icons-material/DescriptionRounded";
import WorkspacesRoundedIcon from "@mui/icons-material/WorkspacesRounded";
import { WorkspaceStatCard } from "@/features/workspace/components/WorkspaceStatCard";

export function WorkspaceStatsGrid() {
  return (
    <section className="mb-8 grid gap-6 md:grid-cols-2 xl:grid-cols-4">
      <WorkspaceStatCard icon={<AutoGraphRoundedIcon />} label="New opportunities" value="147" meta="Last 7 days" />
      <WorkspaceStatCard icon={<BookmarkRoundedIcon />} label="Saved jobs" value="23" meta="Total" />
      <WorkspaceStatCard icon={<WorkspacesRoundedIcon />} label="Saved searches" value="3" meta="Active" />
      <WorkspaceStatCard icon={<DescriptionRoundedIcon />} label="Applications" value="11" meta="In progress" />
    </section>
  );
}
