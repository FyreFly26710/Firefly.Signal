import SearchRoundedIcon from "@mui/icons-material/SearchRounded";

type SearchInputProps = {
  id?: string;
  name?: string;
  ariaLabel?: string;
  placeholder?: string;
  value: string;
  onChange: (value: string) => void;
  onSubmit?: () => void;
  autoFocus?: boolean;
  large?: boolean;
};

export function SearchInput({
  id,
  name,
  ariaLabel,
  placeholder = "Search",
  value,
  onChange,
  onSubmit,
  autoFocus = false,
  large = false
}: SearchInputProps) {
  return (
    <div className="group relative">
      <SearchRoundedIcon
        className={`absolute left-4 top-1/2 -translate-y-1/2 text-foreground-tertiary transition-colors group-focus-within:text-accent-primary ${
          large ? "text-[20px]" : "text-[18px]"
        }`}
      />
      <input
        id={id}
        name={name}
        aria-label={ariaLabel}
        type="text"
        value={value}
        onChange={(event) => onChange(event.target.value)}
        onKeyDown={(event) => {
          if (event.key === "Enter") {
            onSubmit?.();
          }
        }}
        placeholder={placeholder}
        autoFocus={autoFocus}
        className={`w-full rounded-md border border-input-border bg-input-background text-foreground transition-all placeholder:text-muted-foreground focus:border-input-focus focus:outline-none focus:ring-2 focus:ring-accent-primary/20 ${
          large ? "px-4 py-4 pl-12 text-lg" : "px-4 py-3 pl-10 text-base"
        }`}
      />
    </div>
  );
}
