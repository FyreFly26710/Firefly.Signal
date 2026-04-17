import TableRowsRoundedIcon from "@mui/icons-material/TableRowsRounded";
import ViewAgendaRoundedIcon from "@mui/icons-material/ViewAgendaRounded";
import { Button, IconButton, MenuItem, TextField, Tooltip } from "@mui/material";
import { useState } from "react";
import { SearchInput } from "@/components/SearchInput";
import type { DatePosted, SearchSortBy, SearchViewMode } from "@/features/search/types/search.types";

const DATE_POSTED_OPTIONS: { value: DatePosted; label: string }[] = [
  { value: "anytime",  label: "Anytime" },
  { value: "today",   label: "Today" },
  { value: "3days",   label: "Last three days" },
  { value: "1week",   label: "Last week" },
  { value: "2weeks",  label: "Last two weeks" }
];

const SORT_OPTIONS: { value: SearchSortBy; label: string }[] = [
  { value: "date-desc",    label: "Newest first" },
  { value: "date-asc",     label: "Oldest first" },
  { value: "salary-desc",  label: "Salary: High to low" },
  { value: "salary-asc",   label: "Salary: Low to high" }
];

type SearchResultsToolbarProps = {
  initialKeyword: string;
  initialPostcode: string;
  initialSalaryMin: number | null;
  initialSalaryMax: number | null;
  datePosted: DatePosted;
  sortBy: SearchSortBy;
  viewMode: SearchViewMode;
  onSearch: (keyword: string, postcode: string, salaryMin: number | null, salaryMax: number | null) => void;
  onDatePostedChange: (value: DatePosted) => void;
  onSortChange: (value: SearchSortBy) => void;
  onViewModeChange: (mode: SearchViewMode) => void;
};

export function SearchResultsToolbar({
  initialKeyword,
  initialPostcode,
  initialSalaryMin,
  initialSalaryMax,
  datePosted,
  sortBy,
  viewMode,
  onSearch,
  onDatePostedChange,
  onSortChange,
  onViewModeChange
}: SearchResultsToolbarProps) {
  const [draftKeyword, setDraftKeyword] = useState(initialKeyword);
  const [draftPostcode, setDraftPostcode] = useState(initialPostcode);
  const [draftSalaryMin, setDraftSalaryMin] = useState(initialSalaryMin !== null ? String(initialSalaryMin) : "");
  const [draftSalaryMax, setDraftSalaryMax] = useState(initialSalaryMax !== null ? String(initialSalaryMax) : "");

  function parseSalary(value: string): number | null {
    const n = parseInt(value.replace(/[^0-9]/g, ""), 10);
    return Number.isFinite(n) && n >= 0 ? n : null;
  }

  function handleSearch() {
    onSearch(draftKeyword, draftPostcode, parseSalary(draftSalaryMin), parseSalary(draftSalaryMax));
  }

  return (
    <div className="flex flex-col gap-3">
      {/* Row 1: What / Where / Search / View toggle */}
      <div className="grid gap-3 lg:grid-cols-[minmax(0,1fr)_280px_auto_auto]">
        <SearchInput
          ariaLabel="Job title, skill or keyword"
          placeholder="e.g. developer"
          value={draftKeyword}
          onChange={setDraftKeyword}
          onSubmit={handleSearch}
        />
        <SearchInput
          ariaLabel="Town or postcode"
          placeholder="Town or postcode"
          value={draftPostcode}
          onChange={setDraftPostcode}
          onSubmit={handleSearch}
        />
        <Button
          variant="contained"
          onClick={handleSearch}
          sx={{
            minHeight: 48,
            bgcolor: "accent.main",
            "&:hover": { bgcolor: "accent.dark" }
          }}
        >
          Search
        </Button>
        <div className="flex items-center gap-1">
          <Tooltip title="Card view">
            <IconButton
              onClick={() => onViewModeChange("card")}
              size="small"
              sx={{
                color: viewMode === "card" ? "var(--color-accent-primary)" : "var(--color-foreground-tertiary)",
                border: "1px solid",
                borderColor: viewMode === "card" ? "var(--color-accent-primary)" : "var(--color-border)",
                borderRadius: 1,
                p: "10px"
              }}
            >
              <ViewAgendaRoundedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip title="Table view">
            <IconButton
              onClick={() => onViewModeChange("table")}
              size="small"
              sx={{
                color: viewMode === "table" ? "var(--color-accent-primary)" : "var(--color-foreground-tertiary)",
                border: "1px solid",
                borderColor: viewMode === "table" ? "var(--color-accent-primary)" : "var(--color-border)",
                borderRadius: 1,
                p: "10px"
              }}
            >
              <TableRowsRoundedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        </div>
      </div>

      {/* Row 2: Salary range / Date posted / Sort by */}
      <div className="grid gap-3 sm:grid-cols-[1fr_1fr_200px_200px]">
        <TextField
          size="small"
          label="Min salary"
          placeholder="e.g. 30000"
          value={draftSalaryMin}
          onChange={(e) => setDraftSalaryMin(e.target.value)}
          onKeyDown={(e) => { if (e.key === "Enter") handleSearch(); }}
          inputProps={{ inputMode: "numeric", "aria-label": "Minimum salary" }}
        />
        <TextField
          size="small"
          label="Max salary"
          placeholder="e.g. 80000"
          value={draftSalaryMax}
          onChange={(e) => setDraftSalaryMax(e.target.value)}
          onKeyDown={(e) => { if (e.key === "Enter") handleSearch(); }}
          inputProps={{ inputMode: "numeric", "aria-label": "Maximum salary" }}
        />
        <TextField
          select
          size="small"
          label="Date posted"
          value={datePosted}
          onChange={(e) => onDatePostedChange(e.target.value as DatePosted)}
        >
          {DATE_POSTED_OPTIONS.map((opt) => (
            <MenuItem key={opt.value} value={opt.value}>{opt.label}</MenuItem>
          ))}
        </TextField>
        <TextField
          select
          size="small"
          label="Sort by"
          value={sortBy}
          onChange={(e) => onSortChange(e.target.value as SearchSortBy)}
        >
          {SORT_OPTIONS.map((opt) => (
            <MenuItem key={opt.value} value={opt.value}>{opt.label}</MenuItem>
          ))}
        </TextField>
      </div>
    </div>
  );
}
