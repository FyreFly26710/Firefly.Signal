import { useState } from "react";
import { Button, Paper, TextField } from "@mui/material";
import SearchRoundedIcon from "@mui/icons-material/SearchRounded";

type SearchFormProps = {
  initialPostcode: string;
  initialKeyword: string;
  isSubmitting: boolean;
  onSubmit: (postcode: string, keyword: string) => Promise<void>;
};

export function SearchForm({
  initialPostcode,
  initialKeyword,
  isSubmitting,
  onSubmit
}: SearchFormProps) {
  const [postcode, setPostcode] = useState(initialPostcode);
  const [keyword, setKeyword] = useState(initialKeyword);
  const [postcodeError, setPostcodeError] = useState<string | null>(null);
  const [keywordError, setKeywordError] = useState<string | null>(null);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const normalizedPostcode = postcode.trim();
    const normalizedKeyword = keyword.trim();

    const nextPostcodeError = normalizedPostcode ? null : "Enter a postcode.";
    const nextKeywordError = normalizedKeyword ? null : "Enter a keyword.";

    setPostcodeError(nextPostcodeError);
    setKeywordError(nextKeywordError);

    if (nextPostcodeError || nextKeywordError) {
      return;
    }

    await onSubmit(normalizedPostcode, normalizedKeyword);
  }

  return (
    <Paper
      className="h-fit rounded-[28px] border border-white/60 bg-white/90 p-5 shadow-[0_18px_80px_rgba(15,23,42,0.08)] sm:p-6"
      elevation={0}
    >
      <div className="mb-6">
        <p className="text-sm font-semibold uppercase tracking-[0.2em] text-emerald-700">Search</p>
        <h2 className="mt-2 text-2xl font-bold text-slate-950">Find the next promising lead.</h2>
        <p className="mt-2 text-sm leading-6 text-slate-600">
          Start with a postcode and a role keyword. The first version stays intentionally direct.
        </p>
      </div>

      <form
        className="flex flex-col gap-4"
        onSubmit={(event) => {
          void handleSubmit(event);
        }}
      >
        <TextField
          label="UK postcode"
          name="postcode"
          value={postcode}
          onChange={(event) => setPostcode(event.target.value)}
          error={Boolean(postcodeError)}
          helperText={postcodeError ?? "Example: SW1A 1AA"}
          autoComplete="postal-code"
          fullWidth
        />

        <TextField
          label="Job keyword"
          name="keyword"
          value={keyword}
          onChange={(event) => setKeyword(event.target.value)}
          error={Boolean(keywordError)}
          helperText={keywordError ?? "Example: .NET, platform engineer, product analyst"}
          fullWidth
        />

        <Button
          type="submit"
          variant="contained"
          size="large"
          disabled={isSubmitting}
          startIcon={<SearchRoundedIcon />}
          sx={{
            mt: 1,
            minHeight: 52,
            bgcolor: "brand.main",
            "&:hover": {
              bgcolor: "brand.dark"
            }
          }}
        >
          {isSubmitting ? "Searching..." : "Search jobs"}
        </Button>
      </form>
    </Paper>
  );
}
