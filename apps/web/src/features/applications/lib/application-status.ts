import type {
  ApplicationAction,
  ApplicationHistoryItem,
  ApplicationStatus
} from "@/features/applications/types/application.types";

export const applicationQueryKeys = {
  all: ["applications"] as const,
  detail: (applicationId: number) => ["applications", applicationId] as const
};

export function getApplicationStatusLabel(status: ApplicationStatus, roundNumber?: number | null): string {
  if (status === "FaceToFaceInterview" && roundNumber) {
    return `Face-to-face interview · Round ${roundNumber}`;
  }

  return status
    .replace(/([A-Z])/g, " $1")
    .trim();
}

export function getApplicationStatusClasses(status: ApplicationStatus): string {
  switch (status) {
    case "Offered":
      return "bg-signal-fresh-bg text-signal-fresh";
    case "Rejected":
      return "bg-destructive/10 text-destructive";
    case "TelephoneInterview":
    case "FaceToFaceInterview":
      return "bg-accent-secondary text-accent-secondary-foreground";
    case "Applied":
    default:
      return "bg-[color:rgba(217,119,6,0.14)] text-accent-primary";
  }
}

export function getNextApplicationActions(currentStatus: ApplicationStatus): ApplicationAction[] {
  switch (currentStatus) {
    case "Applied":
      return [
        { status: "TelephoneInterview", label: "Mark telephone interview" },
        { status: "Rejected", label: "Mark rejected" }
      ];
    case "TelephoneInterview":
      return [
        { status: "FaceToFaceInterview", label: "Mark face-to-face interview" },
        { status: "Rejected", label: "Mark rejected" }
      ];
    case "FaceToFaceInterview":
      return [
        { status: "FaceToFaceInterview", label: "Add interview round" },
        { status: "Offered", label: "Mark offered" },
        { status: "Rejected", label: "Mark rejected" }
      ];
    default:
      return [];
  }
}

export function formatApplicationTimestamp(value: string | null): string {
  if (!value) {
    return "No update yet";
  }

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    return "Date unavailable";
  }

  return new Intl.DateTimeFormat("en-GB", {
    day: "numeric",
    month: "short",
    year: "numeric"
  }).format(date);
}

export function getCurrentFaceToFaceRound(history: ApplicationHistoryItem[]): number {
  return history
    .filter((entry) => entry.status === "FaceToFaceInterview")
    .reduce((maxRound, entry) => Math.max(maxRound, entry.roundNumber ?? 0), 0);
}
