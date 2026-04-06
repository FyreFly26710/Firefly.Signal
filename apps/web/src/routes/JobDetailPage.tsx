import { useParams } from "react-router-dom";
import { JobDetailView } from "@/features/jobs/views/JobDetailView";

export function JobDetailPage() {
  const { jobId } = useParams<{ jobId: string }>();
  return <JobDetailView jobId={jobId} />;
}
