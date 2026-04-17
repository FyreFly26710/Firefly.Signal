import TableRowsRoundedIcon from "@mui/icons-material/TableRowsRounded";
import ViewAgendaRoundedIcon from "@mui/icons-material/ViewAgendaRounded";
import { Button, IconButton, MenuItem, TextField, Tooltip } from "@mui/material";
import { useState } from "react";
import { SearchInput } from "@/components/SearchInput";
import type { SearchSortBy, SearchViewMode } from "@/features/search/types/search.types";

const sortOptions: { value: SearchSortBy; label: string }[] = [
  { value: "date-desc", label: "Newest first" },
  { value: "date-asc", label: "Oldest first" },
  { value: "salary-desc", label: "Salary: High to low" },
  { value: "salary-asc", label: "Salary: Low to high" }
];

type SearchResultsToolbarProps = {
  initialKeyword: string;
  initialPostcode: string;
  initialCompany: string;
  sortBy: SearchSortBy;
  viewMode: SearchViewMode;
  onSearch: (keyword: string, postcode: string, company: string) => void;
  onSortChange: (sortBy: SearchSortBy) => void;
  onViewModeChange: (mode: SearchViewMode) => void;
};

export function SearchResultsToolbar({
  initialKeyword,
  initialPostcode,
  initialCompany,
  sortBy,
  viewMode,
  onSearch,
  onSortChange,
  onViewModeChange
}: SearchResultsToolbarProps) {
  const [draftKeyword, setDraftKeyword] = useState(initialKeyword);
  const [draftPostcode, setDraftPostcode] = useState(initialPostcode);
  const [draftCompany, setDraftCompany] = useState(initialCompany);

  function handleSearch() {
    onSearch(draftKeyword, draftPostcode, draftCompany);
  }

  return (
    <div className="col-span-full flex flex-col gap-3">
      <div className="grid gap-3 lg:grid-cols-[minmax(0,1fr)_300px_auto_auto]">
        <SearchInput
          ariaLabel="Search roles, companies, skills"
          placeholder="Search roles, companies, skills..."
          value={draftKeyword}
          onChange={setDraftKeyword}
          onSubmit={handleSearch}
        />
        <SearchInput
          ariaLabel="Postcode or area"
          placeholder="Postcode or area"
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

      <div className="grid gap-3 sm:grid-cols-[minmax(0,1fr)_220px]">
        <SearchInput
          ariaLabel="Company name"
          placeholder="Company name"
          value={draftCompany}
          onChange={setDraftCompany}
          onSubmit={handleSearch}
        />
        <TextField
          select
          size="small"
          label="Sort by"
          value={sortBy}
          onChange={(e) => onSortChange(e.target.value as SearchSortBy)}
          sx={{ minHeight: 48 }}
        >
          {sortOptions.map((opt) => (
            <MenuItem key={opt.value} value={opt.value}>
              {opt.label}
            </MenuItem>
          ))}
        </TextField>
      </div>
    </div>
  );
}
