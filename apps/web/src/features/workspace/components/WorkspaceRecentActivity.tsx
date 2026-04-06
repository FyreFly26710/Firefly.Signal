import ScheduleRoundedIcon from "@mui/icons-material/ScheduleRounded";
import { SectionCard } from "@/components/SectionCard";
import { JobCard } from "@/features/jobs/components/JobCard";
import type { MockJob } from "@/features/jobs/types/job.types";

type WorkspaceRecentActivityProps = {
  jobs: MockJob[];
};

export function WorkspaceRecentActivity({ jobs }: WorkspaceRecentActivityProps) {
  return (
    <main>
      <div className="mb-4 flex items-center justify-between gap-3">
        <h2 className="font-serif text-2xl font-semibold text-foreground">Recent activity</h2>
        <div className="inline-flex items-center gap-2 text-sm text-foreground-secondary">
          <ScheduleRoundedIcon sx={{ fontSize: 18 }} />
          Last updated 5m ago
        </div>
      </div>

      <SectionCard className="overflow-hidden">
        {jobs.map((job) => (
          <JobCard key={job.id} job={job} />
        ))}
      </SectionCard>
    </main>
  );
}
