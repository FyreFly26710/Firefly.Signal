import { useState } from "react";
import { TextField } from "@mui/material";
import { MarkdownContent } from "@/components/MarkdownContent";

type MarkdownFieldProps = {
  label: string;
  value: string;
  onChange: (value: string) => void;
  minRows?: number;
};

export function MarkdownField({ label, value, onChange, minRows = 4 }: MarkdownFieldProps) {
  const [showPreview, setShowPreview] = useState(false);

  return (
    <div>
      <div className="mb-1.5 flex items-center justify-between">
        <span className="text-xs font-medium uppercase tracking-wide text-foreground-secondary">
          {label}
        </span>
        <div className="flex overflow-hidden rounded border border-border text-xs">
          <button
            type="button"
            aria-pressed={!showPreview}
            onClick={() => setShowPreview(false)}
            className={`px-2.5 py-1 transition-colors ${!showPreview ? "bg-muted text-foreground" : "text-foreground-secondary hover:text-foreground"}`}
          >
            Write
          </button>
          <button
            type="button"
            aria-pressed={showPreview}
            onClick={() => setShowPreview(true)}
            className={`border-l border-border px-2.5 py-1 transition-colors ${showPreview ? "bg-muted text-foreground" : "text-foreground-secondary hover:text-foreground"}`}
          >
            Preview
          </button>
        </div>
      </div>
      {showPreview ? (
        <div className="min-h-24 rounded border border-border bg-background p-3">
          {value.trim() ? (
            <MarkdownContent content={value} />
          ) : (
            <span className="text-sm text-foreground-tertiary">Nothing to preview.</span>
          )}
        </div>
      ) : (
        <TextField
          fullWidth
          multiline
          minRows={minRows}
          label={label}
          value={value}
          onChange={(e) => onChange(e.target.value)}
        />
      )}
    </div>
  );
}
