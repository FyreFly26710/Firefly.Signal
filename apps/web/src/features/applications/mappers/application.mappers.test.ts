import { describe, expect, it } from "vitest";
import { mapAppliedJobDetail } from "@/features/applications/mappers/application.mappers";

describe("application mappers", () => {
  it("maps round numbers and normalizes missing notes", () => {
    const detail = mapAppliedJobDetail({
      applicationId: 12,
      jobPostingId: 42,
      title: "Senior Product Designer",
      company: "Acme Ltd",
      note: null,
      currentStatus: "FaceToFaceInterview",
      appliedAtUtc: "2026-04-10T08:30:00Z",
      latestStatusAtUtc: "2026-04-12T09:00:00Z",
      statusHistory: [
        { status: "Applied", roundNumber: null, statusAtUtc: "2026-04-10T08:30:00Z" },
        { status: "FaceToFaceInterview", roundNumber: 2, statusAtUtc: "2026-04-12T09:00:00Z" }
      ]
    });

    expect(detail.note).toBe("");
    expect(detail.statusHistory[1]?.roundNumber).toBe(2);
  });
});
