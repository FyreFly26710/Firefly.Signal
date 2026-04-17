import { useQuery } from "@tanstack/react-query";
import { getRecentImportRuns } from "@/api/jobs/jobs.api";

const defaultHistoryPageSize = 5;

export function useJobImportRuns(pageIndex: number, enabled: boolean, pageSize = defaultHistoryPageSize) {
  const requestedLimit = (pageIndex + 1) * pageSize;

  return useQuery({
    queryKey: ["job-import-runs", requestedLimit],
    queryFn: () => getRecentImportRuns(requestedLimit),
    enabled
  });
}
