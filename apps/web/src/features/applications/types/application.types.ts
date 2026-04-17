import type { ApplicationStatusDto } from "@/api/applications/applications.types";

export type ApplicationStatus = ApplicationStatusDto;

export type ApplicationHistoryItem = {
  status: ApplicationStatus;
  roundNumber: number | null;
  statusAt: string;
};

export type AppliedJobSummaryModel = {
  applicationId: number;
  jobPostingId: number;
  title: string;
  company: string;
  currentStatus: ApplicationStatus;
  appliedAt: string;
  latestStatusAt: string | null;
};

export type AppliedJobDetailModel = {
  applicationId: number;
  jobPostingId: number;
  title: string;
  company: string;
  note: string;
  currentStatus: ApplicationStatus;
  appliedAt: string;
  latestStatusAt: string;
  statusHistory: ApplicationHistoryItem[];
};

export type ApplicationAction = {
  status: ApplicationStatus;
  label: string;
};
