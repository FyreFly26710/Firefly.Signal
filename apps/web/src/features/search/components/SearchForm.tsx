import { Button } from "@mui/material";
import { useState } from "react";
import { SearchInput } from "@/features/search/components/SearchInput";

type SearchFormProps = {
  initialKeyword?: string;
  initialPostcode?: string;
  isSubmitting?: boolean;
  onSubmit: (postcode: string, keyword: string) => void;
};

export function SearchForm({
  initialKeyword = "",
  initialPostcode = "",
  isSubmitting = false,
  onSubmit
}: SearchFormProps) {
  const [keyword, setKeyword] = useState(initialKeyword);
  const [postcode, setPostcode] = useState(initialPostcode);
  const [formError, setFormError] = useState<string | null>(null);

  function handleSubmit() {
    const normalizedKeyword = keyword.trim();
    const normalizedPostcode = postcode.trim();

    if (!normalizedKeyword && !normalizedPostcode) {
      setFormError("Enter a keyword or postcode to start searching.");
      return;
    }

    setFormError(null);
    onSubmit(normalizedPostcode, normalizedKeyword);
  }

  return (
    <div className="rounded-lg border border-border-strong bg-background-elevated p-8 shadow-sm">
      <div className="grid gap-4">
        <div>
          <label className="mb-2 block text-sm font-medium text-foreground">
            What role are you looking for?
          </label>
          <SearchInput
            id="search-keyword"
            name="keyword"
            placeholder="e.g. Product Designer, Data Scientist, Engineer"
            value={keyword}
            onChange={setKeyword}
            onSubmit={handleSubmit}
            large
          />
        </div>

        <div>
          <label className="mb-2 block text-sm font-medium text-foreground">
            Where? (UK postcode or area)
          </label>
          <SearchInput
            id="search-postcode"
            name="postcode"
            placeholder="e.g. EC2A, Shoreditch, London"
            value={postcode}
            onChange={setPostcode}
            onSubmit={handleSubmit}
            large
          />
        </div>
      </div>

      {formError ? <p className="mt-4 text-sm text-destructive">{formError}</p> : null}

      <Button
        fullWidth
        variant="contained"
        size="large"
        disabled={isSubmitting}
        onClick={handleSubmit}
        sx={{
          mt: 3,
          minHeight: 56,
          bgcolor: "accent.main",
          "&:hover": { bgcolor: "accent.dark" }
        }}
      >
        {isSubmitting ? "Searching..." : "Search UK jobs"}
      </Button>
    </div>
  );
}
