type WorkspaceHeaderSectionProps = {
  title: string;
  description: string;
};

export function WorkspaceHeaderSection({ title, description }: WorkspaceHeaderSectionProps) {
  return (
    <section className="mb-8">
      <h1 className="font-serif text-4xl font-semibold text-foreground">{title}</h1>
      <p className="mt-3 text-lg text-foreground-secondary">{description}</p>
    </section>
  );
}
