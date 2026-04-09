type JobsManagementHeaderProps = {
  totalCount: number;
};

export function JobsManagementHeader({ totalCount }: JobsManagementHeaderProps) {
  return (
    <div className="mb-6 flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
      <div>
        <p className="font-mono text-xs tracking-[0.2em] text-foreground-tertiary">
          ADMIN JOB MANAGEMENT
        </p>
        <h1 className="mt-2 font-serif text-4xl font-semibold text-foreground">Manage jobs</h1>
        <p className="mt-3 max-w-3xl text-base text-foreground-secondary">
          Browse more jobs in one screen, filter from the top toolbar, and use bulk hide or
          delete actions backed by the batch admin APIs.
        </p>
      </div>
      <div className="rounded-lg border border-border bg-background-elevated px-4 py-3 text-sm text-foreground-secondary">
        <span className="font-medium text-foreground">{totalCount}</span> total jobs
      </div>
    </div>
  );
}
