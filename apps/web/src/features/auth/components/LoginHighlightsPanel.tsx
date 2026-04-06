type LoginHighlightsPanelProps = {
  title: string;
  description: string;
};

export function LoginHighlightsPanel({ title, description }: LoginHighlightsPanelProps) {
  return (
    <div className="rounded-md border border-border bg-muted p-5">
      <h3 className="font-serif text-xl font-semibold text-foreground">{title}</h3>
      <p className="mt-3 text-sm leading-7 text-foreground-secondary">{description}</p>
    </div>
  );
}
