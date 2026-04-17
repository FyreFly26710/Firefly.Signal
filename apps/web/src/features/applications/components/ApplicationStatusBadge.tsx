import { getApplicationStatusClasses, getApplicationStatusLabel } from "@/features/applications/lib/application-status";
import type { ApplicationStatus } from "@/features/applications/types/application.types";

type ApplicationStatusBadgeProps = {
  status: ApplicationStatus;
  roundNumber?: number | null;
};

export function ApplicationStatusBadge({ status, roundNumber = null }: ApplicationStatusBadgeProps) {
  return (
    <span className={`inline-flex rounded-full px-3 py-1 text-xs font-semibold ${getApplicationStatusClasses(status)}`}>
      {getApplicationStatusLabel(status, roundNumber)}
    </span>
  );
}
