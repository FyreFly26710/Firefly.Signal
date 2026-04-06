import { Button } from "@mui/material";
import { useState } from "react";
import { SearchInput } from "@/features/search/components/SearchInput";

type SearchResultsToolbarProps = {
  initialKeyword: string;
  initialPostcode: string;
  onSearch: (keyword: string, postcode: string) => void;
};

export function SearchResultsToolbar({
  initialKeyword,
  initialPostcode,
  onSearch
}: SearchResultsToolbarProps) {
  const [draftKeyword, setDraftKeyword] = useState(initialKeyword);
  const [draftPostcode, setDraftPostcode] = useState(initialPostcode);

  function handleSearch() {
    onSearch(draftKeyword, draftPostcode);
  }

  return (
    <>
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
    </>
  );
}
