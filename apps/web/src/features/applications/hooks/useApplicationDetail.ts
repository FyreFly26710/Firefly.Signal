import { useQuery } from "@tanstack/react-query";
import { getApplicationDetail } from "@/api/applications/applications.api";
import { applicationQueryKeys } from "@/features/applications/lib/application-status";
import { mapAppliedJobDetail } from "@/features/applications/mappers/application.mappers";
import type { AppliedJobDetailModel } from "@/features/applications/types/application.types";

export function useApplicationDetail(applicationId: number | null, enabled = true) {
  return useQuery<AppliedJobDetailModel>({
    queryKey: applicationId !== null ? applicationQueryKeys.detail(applicationId) : applicationQueryKeys.detail(-1),
    queryFn: async () => mapAppliedJobDetail(await getApplicationDetail(applicationId!)),
    enabled: enabled && applicationId !== null
  });
}
