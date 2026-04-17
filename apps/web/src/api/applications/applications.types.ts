export type ApplicationStatusDto =
  | "Applied"
  | "TelephoneInterview"
  | "FaceToFaceInterview"
  | "Offered"
  | "Rejected";

export type JobApplicationStatusHistoryDto = {
  status: ApplicationStatusDto;
  roundNumber: number | null;
  statusAtUtc: string;
};

export type JobApplicationResponseDto = {
  id: number;
  jobPostingId: number;
  note: string | null;
  currentStatus: ApplicationStatusDto;
  appliedAtUtc: string;
  latestStatusAtUtc: string;
  statusHistory: JobApplicationStatusHistoryDto[];
};

export type AppliedJobSummaryResponseDto = {
  applicationId: number;
  jobPostingId: number;
  title: string;
  company: string;
  currentStatus: ApplicationStatusDto;
  appliedAtUtc: string;
  latestStatusAtUtc: string | null;
};

export type AppliedJobDetailResponseDto = {
  applicationId: number;
  jobPostingId: number;
  title: string;
  company: string;
  note: string | null;
  currentStatus: ApplicationStatusDto;
  appliedAtUtc: string;
  latestStatusAtUtc: string;
  statusHistory: JobApplicationStatusHistoryDto[];
};

export type ApplyJobRequestDto = {
  note?: string | null;
};

export type AdvanceApplicationStatusRequestDto = {
  status: ApplicationStatusDto;
};

export type UpdateApplicationNoteRequestDto = {
  note?: string | null;
};
