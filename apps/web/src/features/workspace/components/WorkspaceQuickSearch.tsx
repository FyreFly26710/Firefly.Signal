import SearchRoundedIcon from "@mui/icons-material/SearchRounded";
import { Button } from "@mui/material";
import { SearchInput } from "@/components/SearchInput";
import { SectionCard } from "@/components/SectionCard";

type WorkspaceQuickSearchProps = {
  keyword: string;
  onKeywordChange: (value: string) => void;
  onSearch: () => void;
};

export function WorkspaceQuickSearch({
  keyword,
  onKeywordChange,
  onSearch
}: WorkspaceQuickSearchProps) {
  return (
    <SectionCard className="mb-8 p-6">
      <div className="flex flex-col gap-3 lg:flex-row">
        <div className="flex-1">
          <SearchInput
            placeholder="Quick search: roles, companies, skills..."
            value={keyword}
            onChange={onKeywordChange}
            onSubmit={onSearch}
            large
          />
        </div>
        <Button
          variant="contained"
          onClick={onSearch}
          sx={{
            minWidth: 132,
            bgcolor: "accent.main",
            "&:hover": { bgcolor: "accent.dark" }
          }}
          startIcon={<SearchRoundedIcon />}
        >
          Search
        </Button>
      </div>
    </SectionCard>
  );
}
