import { Alert } from "@mui/material";

type JobEditorAlertsProps = {
  errorMessage: string | null;
  isAdmin: boolean;
  saveMessage: string | null;
};

export function JobEditorAlerts({
  errorMessage,
  isAdmin,
  saveMessage
}: JobEditorAlertsProps) {
  return (
    <>
      {!isAdmin ? (
        <Alert severity="info" sx={{ mt: 4 }}>
          You can inspect the job resource, but save, hide, and delete actions require an admin
          role from the backend API.
        </Alert>
      ) : null}

      {saveMessage ? <Alert severity="success" sx={{ mt: 4 }}>{saveMessage}</Alert> : null}
      {errorMessage ? <Alert severity="error" sx={{ mt: 4 }}>{errorMessage}</Alert> : null}
    </>
  );
}
