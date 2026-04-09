import FilterListRoundedIcon from "@mui/icons-material/FilterListRounded";
import VisibilityOffRoundedIcon from "@mui/icons-material/VisibilityOffRounded";
import DeleteOutlineRoundedIcon from "@mui/icons-material/DeleteOutlineRounded";
import { Button, MenuItem, TextField } from "@mui/material";

export type VisibilityFilter = "visible" | "hidden" | "all";

export type JobsListFilters = {
  keyword: string;
  company: string;
  postcode: string;
  location: string;
  sourceName: string;
  categoryTag: string;
  visibility: VisibilityFilter;
};

type JobsManagementToolbarProps = {
  draftFilters: JobsListFilters;
  isAdmin: boolean;
  isProcessing: boolean;
  selectionSummary: string;
  selectedCount: number;
  onApplyFilters: () => void;
  onDeleteSelected: () => void;
  onFiltersChange: (filters: JobsListFilters) => void;
  onHideSelected: () => void;
  onResetFilters: () => void;
};

export function JobsManagementToolbar({
  draftFilters,
  isAdmin,
  isProcessing,
  selectionSummary,
  selectedCount,
  onApplyFilters,
  onDeleteSelected,
  onFiltersChange,
  onHideSelected,
  onResetFilters
}: JobsManagementToolbarProps) {
  return (
    <div className="border-b border-divider p-5">
      <div className="flex flex-col gap-4">
        <div className="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
          <div className="flex items-center gap-2">
            <FilterListRoundedIcon className="text-foreground-tertiary" />
            <h2 className="font-serif text-2xl font-semibold text-foreground">Job table</h2>
          </div>
          <div className="flex flex-wrap items-center gap-3">
            <span className="text-sm text-foreground-secondary">{selectionSummary}</span>
            <Button
              variant="outlined"
              color="inherit"
              startIcon={<VisibilityOffRoundedIcon />}
              disabled={!isAdmin || selectedCount === 0 || isProcessing}
              onClick={onHideSelected}
            >
              {isProcessing ? "Working..." : "Hide selected"}
            </Button>
            <Button
              variant="outlined"
              color="error"
              startIcon={<DeleteOutlineRoundedIcon />}
              disabled={!isAdmin || selectedCount === 0 || isProcessing}
              onClick={onDeleteSelected}
            >
              {isProcessing ? "Working..." : "Delete selected"}
            </Button>
          </div>
        </div>

        <div className="grid gap-3 md:grid-cols-3 xl:grid-cols-7">
          <TextField
            label="Keyword"
            size="small"
            value={draftFilters.keyword}
            onChange={(event) =>
              onFiltersChange({ ...draftFilters, keyword: event.target.value })
            }
          />
          <TextField
            label="Company"
            size="small"
            value={draftFilters.company}
            onChange={(event) =>
              onFiltersChange({ ...draftFilters, company: event.target.value })
            }
          />
          <TextField
            label="Postcode"
            size="small"
            value={draftFilters.postcode}
            onChange={(event) =>
              onFiltersChange({ ...draftFilters, postcode: event.target.value })
            }
          />
          <TextField
            label="Location"
            size="small"
            value={draftFilters.location}
            onChange={(event) =>
              onFiltersChange({ ...draftFilters, location: event.target.value })
            }
          />
          <TextField
            label="Source"
            size="small"
            value={draftFilters.sourceName}
            onChange={(event) =>
              onFiltersChange({ ...draftFilters, sourceName: event.target.value })
            }
          />
          <TextField
            label="Category"
            size="small"
            value={draftFilters.categoryTag}
            onChange={(event) =>
              onFiltersChange({ ...draftFilters, categoryTag: event.target.value })
            }
          />
          <TextField
            select
            label="Visibility"
            size="small"
            value={draftFilters.visibility}
            onChange={(event) =>
              onFiltersChange({
                ...draftFilters,
                visibility: event.target.value as VisibilityFilter
              })
            }
          >
            <MenuItem value="visible">Visible</MenuItem>
            <MenuItem value="hidden">Hidden</MenuItem>
            <MenuItem value="all">All</MenuItem>
          </TextField>
        </div>

        <div className="flex flex-wrap gap-3">
          <Button
            variant="contained"
            onClick={onApplyFilters}
            sx={{
              bgcolor: "accent.main",
              "&:hover": { bgcolor: "accent.dark" }
            }}
          >
            Apply filters
          </Button>
          <Button variant="outlined" color="inherit" onClick={onResetFilters}>
            Reset
          </Button>
        </div>
      </div>
    </div>
  );
}
