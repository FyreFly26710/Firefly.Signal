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
    <div className="border-b border-divider px-5 py-4">
      <div className="flex flex-col gap-3">
        <div className="flex flex-col gap-2 lg:flex-row lg:items-center lg:justify-between">
          <div className="flex items-center gap-2">
            <FilterListRoundedIcon className="text-foreground-tertiary" />
            <h2 className="font-serif text-xl font-semibold text-foreground">Job table</h2>
          </div>
          <div className="flex flex-wrap items-center gap-2">
            <span className="text-xs text-foreground-secondary">{selectionSummary}</span>
            <Button
              variant="outlined"
              color="inherit"
              startIcon={<VisibilityOffRoundedIcon />}
              disabled={!isAdmin || selectedCount === 0 || isProcessing}
              onClick={onHideSelected}
              size="small"
            >
              {isProcessing ? "Working..." : "Hide selected"}
            </Button>
            <Button
              variant="outlined"
              color="error"
              startIcon={<DeleteOutlineRoundedIcon />}
              disabled={!isAdmin || selectedCount === 0 || isProcessing}
              onClick={onDeleteSelected}
              size="small"
            >
              {isProcessing ? "Working..." : "Delete selected"}
            </Button>
          </div>
        </div>

        <div className="grid gap-2 md:grid-cols-3 xl:grid-cols-[1.2fr_1fr_0.8fr_1fr_0.9fr_0.9fr_0.9fr_auto_auto]">
          <TextField
            size="small"
            placeholder="Keyword"
            inputProps={{ "aria-label": "Keyword" }}
            value={draftFilters.keyword}
            onChange={(event) =>
              onFiltersChange({ ...draftFilters, keyword: event.target.value })
            }
          />
          <TextField
            size="small"
            placeholder="Company"
            inputProps={{ "aria-label": "Company" }}
            value={draftFilters.company}
            onChange={(event) =>
              onFiltersChange({ ...draftFilters, company: event.target.value })
            }
          />
          <TextField
            size="small"
            placeholder="Postcode"
            inputProps={{ "aria-label": "Postcode" }}
            value={draftFilters.postcode}
            onChange={(event) =>
              onFiltersChange({ ...draftFilters, postcode: event.target.value })
            }
          />
          <TextField
            size="small"
            placeholder="Location"
            inputProps={{ "aria-label": "Location" }}
            value={draftFilters.location}
            onChange={(event) =>
              onFiltersChange({ ...draftFilters, location: event.target.value })
            }
          />
          <TextField
            size="small"
            placeholder="Source"
            inputProps={{ "aria-label": "Source" }}
            value={draftFilters.sourceName}
            onChange={(event) =>
              onFiltersChange({ ...draftFilters, sourceName: event.target.value })
            }
          />
          <TextField
            size="small"
            placeholder="Category"
            inputProps={{ "aria-label": "Category" }}
            value={draftFilters.categoryTag}
            onChange={(event) =>
              onFiltersChange({ ...draftFilters, categoryTag: event.target.value })
            }
          />
          <TextField
            select
            size="small"
            inputProps={{ "aria-label": "Visibility" }}
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
          <Button
            variant="contained"
            onClick={onApplyFilters}
            size="small"
            sx={{
              bgcolor: "accent.main",
              "&:hover": { bgcolor: "accent.dark" }
            }}
          >
            Apply filters
          </Button>
          <Button variant="outlined" color="inherit" onClick={onResetFilters} size="small">
            Reset
          </Button>
        </div>
      </div>
    </div>
  );
}
