import type {
  AppliedJobDetailResponseDto,
  AppliedJobSummaryResponseDto,
  JobApplicationResponseDto,
  JobApplicationStatusHistoryDto
} from "@/api/applications/applications.types";
import type {
  AppliedJobDetailModel,
  AppliedJobSummaryModel,
  ApplicationHistoryItem
} from "@/features/applications/types/application.types";

export function mapAppliedJobSummary(summary: AppliedJobSummaryResponseDto): AppliedJobSummaryModel {
  return {
    applicationId: summary.applicationId,
    jobPostingId: summary.jobPostingId,
    title: summary.title,
    company: summary.company,
    currentStatus: summary.currentStatus,
    appliedAt: summary.appliedAtUtc,
    latestStatusAt: summary.latestStatusAtUtc
  };
}

export function mapAppliedJobDetail(detail: AppliedJobDetailResponseDto): AppliedJobDetailModel {
  return {
    applicationId: detail.applicationId,
    jobPostingId: detail.jobPostingId,
    title: detail.title,
    company: detail.company,
    note: detail.note ?? "",
    currentStatus: detail.currentStatus,
    appliedAt: detail.appliedAtUtc,
    latestStatusAt: detail.latestStatusAtUtc,
    statusHistory: detail.statusHistory.map(mapApplicationHistoryItem)
  };
}

export function mapJobApplicationResponse(response: JobApplicationResponseDto): AppliedJobDetailModel {
  return {
    applicationId: response.id,
    jobPostingId: response.jobPostingId,
    title: "",
    company: "",
    note: response.note ?? "",
    currentStatus: response.currentStatus,
    appliedAt: response.appliedAtUtc,
    latestStatusAt: response.latestStatusAtUtc,
    statusHistory: response.statusHistory.map(mapApplicationHistoryItem)
  };
}

function mapApplicationHistoryItem(item: JobApplicationStatusHistoryDto): ApplicationHistoryItem {
  return {
    status: item.status,
    roundNumber: item.roundNumber,
    statusAt: item.statusAtUtc
  };
}
