import { useQuery } from "@tanstack/react-query";
import { getAppliedJobs } from "@/api/applications/applications.api";
import { mapAppliedJobSummary } from "@/features/applications/mappers/application.mappers";
import { applicationQueryKeys } from "@/features/applications/lib/application-status";

export function useAppliedJobs() {
  return useQuery({
    queryKey: applicationQueryKeys.all,
    queryFn: async () => (await getAppliedJobs()).map(mapAppliedJobSummary),
    staleTime: 30_000
  });
}
