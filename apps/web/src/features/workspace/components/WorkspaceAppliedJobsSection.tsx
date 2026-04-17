import ExpandMoreRoundedIcon from "@mui/icons-material/ExpandMoreRounded";
import OpenInNewRoundedIcon from "@mui/icons-material/OpenInNewRounded";
import { Accordion, AccordionDetails, AccordionSummary, Alert } from "@mui/material";
import { useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { Link } from "react-router-dom";
import { advanceApplicationStatus, updateApplicationNote } from "@/api/applications/applications.api";
import { SectionCard } from "@/components/SectionCard";
import { ApplicationManagementPanel } from "@/features/applications/components/ApplicationManagementPanel";
import { ApplicationStatusBadge } from "@/features/applications/components/ApplicationStatusBadge";
import { useApplicationDetail } from "@/features/applications/hooks/useApplicationDetail";
import { useAppliedJobs } from "@/features/applications/hooks/useAppliedJobs";
import {
  applicationQueryKeys,
  formatApplicationTimestamp
} from "@/features/applications/lib/application-status";
import type {
  AppliedJobSummaryModel,
  ApplicationStatus
} from "@/features/applications/types/application.types";

type AppliedJobAccordionItemProps = {
  summary: AppliedJobSummaryModel;
};

export function WorkspaceAppliedJobsSection() {
  const { data: jobs, isPending, isError, error } = useAppliedJobs();

  return (
    <SectionCard className="overflow-hidden border-border-strong">
      <div className="border-b border-divider bg-background px-6 py-6">
        <p className="font-mono text-xs uppercase tracking-[0.18em] text-metadata">Workspace section</p>
        <div className="mt-3 flex flex-wrap items-end justify-between gap-4">
          <div>
            <h2 className="font-serif text-3xl font-semibold text-foreground">Applied jobs</h2>
            <p className="mt-2 max-w-3xl text-sm leading-7 text-foreground-secondary">
              Review every active application in one place, expand for notes and timeline updates,
              and keep future workspace sections free to focus on other parts of the search.
            </p>
          </div>
          <div className="rounded-2xl border border-border bg-muted px-4 py-3 text-right">
            <p className="text-xs uppercase tracking-[0.16em] text-metadata">Tracked applications</p>
            <p className="mt-2 font-mono text-2xl font-semibold text-foreground">{jobs?.length ?? 0}</p>
          </div>
        </div>
      </div>

      {isPending ? (
        <div className="px-6 py-10 text-sm text-foreground-secondary">Loading applied jobs...</div>
      ) : null}

      {isError ? (
        <div className="px-6 py-6">
          <Alert severity="error">{error instanceof Error ? error.message : "Unable to load applications."}</Alert>
        </div>
      ) : null}

      {!isPending && !isError && jobs?.length === 0 ? (
        <div className="px-6 py-10 text-sm text-foreground-secondary">
          No applied jobs yet. Mark roles as applied from search results or the job detail page to start building your workspace.
        </div>
      ) : null}

      {!isPending && !isError ? (
        <div className="divide-y divide-divider">
          {jobs?.map((job) => (
            <AppliedJobAccordionItem key={job.applicationId} summary={job} />
          ))}
        </div>
      ) : null}
    </SectionCard>
  );
}

function AppliedJobAccordionItem({ summary }: AppliedJobAccordionItemProps) {
  const queryClient = useQueryClient();
  const [expanded, setExpanded] = useState(false);
  const { data: application, isPending, isError, error } = useApplicationDetail(summary.applicationId, expanded);

  return (
    <Accordion
      expanded={expanded}
      onChange={(_, nextExpanded) => setExpanded(nextExpanded)}
      disableGutters
      elevation={0}
      square
      sx={{ "&::before": { display: "none" }, bgcolor: "transparent" }}
    >
      <AccordionSummary expandIcon={<ExpandMoreRoundedIcon />} sx={{ px: 3, py: 1.5 }}>
        {application ? (
          <div className="grid w-full gap-4 lg:grid-cols-[minmax(0,1.2fr)_220px_180px_180px] lg:items-center">
            <div>
              <p className="font-medium text-foreground">{application.title || summary.title}</p>
              <p className="mt-1 text-sm text-foreground-secondary">{application.company || summary.company}</p>
            </div>
            <div className="flex flex-wrap gap-2">
              <ApplicationStatusBadge
                status={application.currentStatus}
                roundNumber={application.statusHistory.at(-1)?.roundNumber ?? null}
              />
            </div>
            <div>
              <p className="text-xs uppercase tracking-[0.16em] text-metadata">Applied on</p>
              <p className="mt-1 text-sm text-foreground">{formatApplicationTimestamp(application.appliedAt)}</p>
            </div>
            <div>
              <p className="text-xs uppercase tracking-[0.16em] text-metadata">Last updated</p>
              <p className="mt-1 text-sm text-foreground">{formatApplicationTimestamp(application.latestStatusAt)}</p>
            </div>
          </div>
        ) : (
          <div className="grid w-full gap-4 lg:grid-cols-[minmax(0,1.2fr)_220px_180px_180px] lg:items-center">
            <div>
              <p className="font-medium text-foreground">{summary.title}</p>
              <p className="mt-1 text-sm text-foreground-secondary">{summary.company}</p>
            </div>
            <div className="flex flex-wrap gap-2">
              <ApplicationStatusBadge status={summary.currentStatus} />
            </div>
            <div>
              <p className="text-xs uppercase tracking-[0.16em] text-metadata">Applied on</p>
              <p className="mt-1 text-sm text-foreground">{formatApplicationTimestamp(summary.appliedAt)}</p>
            </div>
            <div>
              <p className="text-xs uppercase tracking-[0.16em] text-metadata">Last updated</p>
              <p className="mt-1 text-sm text-foreground">{formatApplicationTimestamp(summary.latestStatusAt)}</p>
            </div>
          </div>
        )}
      </AccordionSummary>
      <AccordionDetails sx={{ px: 3, pb: 3 }}>
        {isPending ? <p className="text-sm text-foreground-secondary">Loading application detail...</p> : null}
        {isError ? (
          <Alert severity="error">{error instanceof Error ? error.message : "Unable to load application detail."}</Alert>
        ) : null}
        {application ? (
          <ApplicationManagementPanel
            key={`${application.applicationId}-${application.latestStatusAt}-${application.note}`}
            application={application}
            title={application.title}
            subtitle={`${application.company} · Expand, edit, and keep every status change in one shared workspace section.`}
            onAdvanceStatus={async (status: ApplicationStatus) => {
              await advanceApplicationStatus(application.jobPostingId, { status });
              await Promise.all([
                queryClient.invalidateQueries({ queryKey: applicationQueryKeys.detail(application.applicationId) }),
                queryClient.invalidateQueries({ queryKey: applicationQueryKeys.all })
              ]);
            }}
            onSaveNote={async (note: string) => {
              await updateApplicationNote(application.jobPostingId, { note });
              await Promise.all([
                queryClient.invalidateQueries({ queryKey: applicationQueryKeys.detail(application.applicationId) }),
                queryClient.invalidateQueries({ queryKey: applicationQueryKeys.all })
              ]);
            }}
            footer={
              <Link
                to={`/jobs/${application.jobPostingId}`}
                className="inline-flex items-center gap-2 text-sm font-medium text-foreground-secondary transition-colors hover:text-accent-primary"
              >
                Open job detail
                <OpenInNewRoundedIcon sx={{ fontSize: 16 }} />
              </Link>
            }
          />
        ) : null}
      </AccordionDetails>
    </Accordion>
  );
}
