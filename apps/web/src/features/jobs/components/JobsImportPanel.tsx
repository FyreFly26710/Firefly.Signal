import AddRoundedIcon from "@mui/icons-material/AddRounded";
import CloudDownloadRoundedIcon from "@mui/icons-material/CloudDownloadRounded";
import CloudUploadRoundedIcon from "@mui/icons-material/CloudUploadRounded";
import DownloadRoundedIcon from "@mui/icons-material/DownloadRounded";
import { Button } from "@mui/material";

type JobsImportPanelProps = {
  isProcessing: boolean;
  canExport: boolean;
  onCreateJob: () => void;
  onImportProvider: () => void;
  onImportJson: () => void;
  onExportJson: () => void;
};

export function JobsImportPanel({
  isProcessing,
  canExport,
  onCreateJob,
  onImportProvider,
  onImportJson,
  onExportJson
}: JobsImportPanelProps) {
  return (
    <div className="mb-6 rounded-2xl border border-border bg-background-elevated p-5">
      <div className="flex flex-col gap-4">
        <div className="flex flex-col gap-2 lg:flex-row lg:items-end lg:justify-between">
          <div>
            <p className="font-mono text-xs tracking-[0.18em] text-foreground-tertiary">
              IMPORT AND EXPORT
            </p>
            <h2 className="mt-2 font-serif text-2xl font-semibold text-foreground">
              Provider and JSON tools
            </h2>
            <p className="mt-2 max-w-3xl text-sm text-foreground-secondary">
              Create a manual job, trigger a provider import into PostgreSQL, upload a JSON export
              back into the catalog, or select jobs from the list below and export them to JSON.
            </p>
          </div>

          <div className="flex flex-wrap gap-3">
            <Button
              variant="outlined"
              color="inherit"
              startIcon={<AddRoundedIcon />}
              disabled={isProcessing}
              onClick={onCreateJob}
            >
              New job
            </Button>
            <Button
              variant="outlined"
              color="inherit"
              startIcon={<DownloadRoundedIcon />}
              disabled={isProcessing || !canExport}
              onClick={onExportJson}
              title={!canExport ? "Select jobs to export" : undefined}
            >
              {isProcessing ? "Working..." : "Export JSON"}
            </Button>
            <Button
              variant="outlined"
              color="inherit"
              startIcon={<CloudUploadRoundedIcon />}
              disabled={isProcessing}
              onClick={onImportJson}
            >
              {isProcessing ? "Working..." : "Import JSON"}
            </Button>
            <Button
              variant="contained"
              startIcon={<CloudDownloadRoundedIcon />}
              disabled={isProcessing}
              onClick={onImportProvider}
              sx={{
                bgcolor: "accent.main",
                "&:hover": { bgcolor: "accent.dark" }
              }}
            >
              {isProcessing ? "Working..." : "Import from provider"}
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
