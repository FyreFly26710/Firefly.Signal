import SaveRoundedIcon from "@mui/icons-material/SaveRounded";
import { Alert, Button, TextField } from "@mui/material";
import { useState, type ReactNode } from "react";
import { SectionCard } from "@/components/SectionCard";
import {
  formatApplicationTimestamp,
  getNextApplicationActions,
  getApplicationStatusLabel
} from "@/features/applications/lib/application-status";
import { ApplicationStatusBadge } from "@/features/applications/components/ApplicationStatusBadge";
import type {
  AppliedJobDetailModel,
  ApplicationStatus
} from "@/features/applications/types/application.types";
import { useAsyncTask } from "@/lib/async/useAsyncTask";

type ApplicationManagementPanelProps = {
  application: AppliedJobDetailModel;
  title?: string;
  onAdvanceStatus: (status: ApplicationStatus) => Promise<void>;
  onSaveNote: (note: string) => Promise<void>;
  footer?: ReactNode;
};

export function ApplicationManagementPanel({
  application,
  title = "Application status",
  onAdvanceStatus,
  onSaveNote,
  footer
}: ApplicationManagementPanelProps) {
  const [noteDraft, setNoteDraft] = useState(application.note);
  const advanceTask = useAsyncTask(async (status: ApplicationStatus) => {
    await onAdvanceStatus(status);
    return status;
  });
  const noteTask = useAsyncTask(async (note: string) => {
    await onSaveNote(note);
    return note;
  });

  const nextActions = getNextApplicationActions(application.currentStatus);
  const hasNoteChanges = noteDraft.trim() !== application.note.trim();

  return (
    <SectionCard className="p-6">
      <div className="flex flex-wrap items-start justify-between gap-4">
        <div>
          <p className="font-mono text-xs uppercase tracking-[0.18em] text-metadata">Application workflow</p>
          <h2 className="mt-3 font-serif text-2xl font-semibold text-foreground">{title}</h2>
        </div>
        <ApplicationStatusBadge
          status={application.currentStatus}
          roundNumber={application.statusHistory.at(-1)?.roundNumber ?? null}
        />
      </div>

      <div className="mt-5 grid gap-3 sm:grid-cols-2">
        <div className="rounded-2xl border border-border bg-muted px-4 py-3">
          <p className="text-xs uppercase tracking-[0.16em] text-metadata">Applied on</p>
          <p className="mt-2 text-sm font-medium text-foreground">{formatApplicationTimestamp(application.appliedAt)}</p>
        </div>
        <div className="rounded-2xl border border-border bg-muted px-4 py-3">
          <p className="text-xs uppercase tracking-[0.16em] text-metadata">Last updated</p>
          <p className="mt-2 text-sm font-medium text-foreground">{formatApplicationTimestamp(application.latestStatusAt)}</p>
        </div>
      </div>

      {nextActions.length ? (
        <div className="mt-6 flex flex-wrap gap-3">
          {nextActions.map((action) => (
            <Button
              key={`${application.applicationId}-${action.label}`}
              variant={action.status === "Rejected" ? "outlined" : "contained"}
              color={action.status === "Rejected" ? "inherit" : "primary"}
              disabled={advanceTask.status === "loading"}
              onClick={() => { void advanceTask.execute(action.status); }}
              sx={action.status === "Rejected" ? undefined : { bgcolor: "accent.main", "&:hover": { bgcolor: "accent.dark" } }}
            >
              {action.label}
            </Button>
          ))}
        </div>
      ) : null}

      {advanceTask.status === "error" ? (
        <Alert severity="error" sx={{ mt: 2 }}>{advanceTask.errorMessage}</Alert>
      ) : null}

      <div className="mt-6">
        <TextField
          fullWidth
          multiline
          minRows={4}
          label="Application note"
          value={noteDraft}
          onChange={(event) => setNoteDraft(event.target.value)}
        />
        <div className="mt-3 flex items-center justify-end gap-3">
          <Button
            variant="outlined"
            startIcon={<SaveRoundedIcon />}
            disabled={!hasNoteChanges || noteTask.status === "loading"}
            onClick={() => { void noteTask.execute(noteDraft); }}
          >
            Save note
          </Button>
        </div>
        {noteTask.status === "error" ? (
          <Alert severity="error" sx={{ mt: 2 }}>{noteTask.errorMessage}</Alert>
        ) : null}
      </div>

      <div className="mt-8">
        <h3 className="font-serif text-xl font-semibold text-foreground">Status history</h3>
        <ol className="mt-4 space-y-3">
          {application.statusHistory.map((entry) => (
            <li key={`${entry.status}-${entry.statusAt}-${entry.roundNumber ?? "none"}`} className="rounded-2xl border border-border bg-background px-4 py-3">
              <div className="flex flex-wrap items-center justify-between gap-3">
                <span className="text-sm font-medium text-foreground">
                  {getApplicationStatusLabel(entry.status, entry.roundNumber)}
                </span>
                <span className="text-xs uppercase tracking-[0.14em] text-foreground-tertiary">
                  {formatApplicationTimestamp(entry.statusAt)}
                </span>
              </div>
            </li>
          ))}
        </ol>
      </div>

      {footer ? <div className="mt-6">{footer}</div> : null}
    </SectionCard>
  );
}
