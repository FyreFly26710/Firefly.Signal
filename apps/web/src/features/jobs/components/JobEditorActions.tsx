import DeleteOutlineRoundedIcon from "@mui/icons-material/DeleteOutlineRounded";
import SaveRoundedIcon from "@mui/icons-material/SaveRounded";
import VisibilityOffRoundedIcon from "@mui/icons-material/VisibilityOffRounded";
import { Button } from "@mui/material";
import { SectionCard } from "@/components/SectionCard";

type JobEditorActionsProps = {
  isAdmin: boolean;
  isCreateMode: boolean;
  isDeleting: boolean;
  isHiding: boolean;
  isSaving: boolean;
  onDelete: () => void;
  onHide: () => void;
};

export function JobEditorActions({
  isAdmin,
  isCreateMode,
  isDeleting,
  isHiding,
  isSaving,
  onDelete,
  onHide
}: JobEditorActionsProps) {
  return (
    <SectionCard className="flex flex-col gap-4 p-6 sm:flex-row sm:items-center sm:justify-between">
      <div className="text-sm text-foreground-secondary">
        {isCreateMode
          ? "Create a new job resource in the backend table."
          : "Save changes, hide the job, or delete it after it has been hidden."}
      </div>
      <div className="flex flex-wrap gap-3">
        {!isCreateMode ? (
          <Button
            type="button"
            variant="outlined"
            color="inherit"
            startIcon={<VisibilityOffRoundedIcon />}
            disabled={!isAdmin || isHiding || isSaving || isDeleting}
            onClick={onHide}
          >
            {isHiding ? "Hiding..." : "Hide job"}
          </Button>
        ) : null}
        {!isCreateMode ? (
          <Button
            type="button"
            color="error"
            variant="outlined"
            startIcon={<DeleteOutlineRoundedIcon />}
            disabled={!isAdmin || isDeleting || isSaving || isHiding}
            onClick={onDelete}
          >
            {isDeleting ? "Deleting..." : "Delete job"}
          </Button>
        ) : null}
        <Button
          type="submit"
          variant="contained"
          startIcon={<SaveRoundedIcon />}
          disabled={!isAdmin || isSaving || isHiding || isDeleting}
          sx={{
            bgcolor: "accent.main",
            "&:hover": { bgcolor: "accent.dark" }
          }}
        >
          {isSaving ? "Saving..." : isCreateMode ? "Create job" : "Save changes"}
        </Button>
      </div>
    </SectionCard>
  );
}
