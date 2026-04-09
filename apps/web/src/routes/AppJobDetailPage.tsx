import { useParams } from "react-router-dom";
import { ManageJobView } from "@/features/jobs/views/ManageJobView";

export function AppJobDetailPage() {
  const { jobId } = useParams<{ jobId: string }>();
  return <ManageJobView jobId={jobId} />;
}
