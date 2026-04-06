import { useParams } from "react-router-dom";
import { JobDetailExperience } from "@/features/jobs/components/JobDetailExperience";

export function JobDetailPage() {
  const { jobId } = useParams<{ jobId: string }>();
  return <JobDetailExperience jobId={jobId} />;
}
