import { useQuery } from "@tanstack/react-query";
import { getJobById } from "@/api/jobs/jobs.api";
import { mapJobDetail } from "@/features/jobs/mappers/job-detail.mappers";

export function useJobDetail(jobId: number | null) {
  return useQuery({
    queryKey: ["jobs", jobId],
    queryFn: () => getJobById(jobId!).then(mapJobDetail),
    enabled: jobId !== null
  });
}
