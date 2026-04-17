import { useQuery } from "@tanstack/react-query";
import { getRecentImportRuns } from "@/api/jobs/jobs.api";

export function useJobImportRuns(pageIndex: number, enabled: boolean, pageSize = defaultHistoryPageSize) {
  return useQuery({
    queryKey: ["job-import-runs", pageIndex, pageSize],
    queryFn: () => getRecentImportRuns({ pageIndex, pageSize }),
    enabled
  });
}

const defaultHistoryPageSize = 5;
