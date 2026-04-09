import ArrowBackRoundedIcon from "@mui/icons-material/ArrowBackRounded";
import { Link } from "react-router-dom";

type JobEditorHeaderProps = {
  isCreateMode: boolean;
  jobId: number | null;
  pageTitle: string;
};

export function JobEditorHeader({ isCreateMode, jobId, pageTitle }: JobEditorHeaderProps) {
  return (
    <>
      <Link
        to="/admin/manage-jobs"
        className="inline-flex items-center gap-2 text-sm text-foreground-secondary transition-colors hover:text-accent-primary"
      >
        <ArrowBackRoundedIcon sx={{ fontSize: 18 }} />
        Back to jobs
      </Link>

      <div className="mt-6 flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div>
          <p className="font-mono text-xs tracking-[0.2em] text-foreground-tertiary">
            BACKEND-ALIGNED JOB CONTRACT
          </p>
          <h1 className="mt-2 font-serif text-4xl font-semibold text-foreground">{pageTitle}</h1>
          <p className="mt-3 max-w-3xl text-sm text-foreground-secondary">
            This editor mirrors the backend jobs contract directly, including hide and delete
            behavior enforced by the API.
          </p>
        </div>
        {!isCreateMode ? (
          <div className="rounded-lg border border-border bg-background-elevated px-4 py-3 text-sm text-foreground-secondary">
            Resource ID <span className="font-medium text-foreground">{jobId}</span>
          </div>
        ) : null}
      </div>
    </>
  );
}
