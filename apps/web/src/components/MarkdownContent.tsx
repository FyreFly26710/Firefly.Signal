import ReactMarkdown from "react-markdown";

type MarkdownContentProps = {
  content: string | null;
};

export function MarkdownContent({ content }: MarkdownContentProps) {
  if (!content) return null;

  return (
    <div className="markdown-content text-sm leading-7 text-foreground [&_h1]:mb-2 [&_h1]:text-lg [&_h1]:font-semibold [&_h2]:mb-1 [&_h2]:text-base [&_h2]:font-semibold [&_li]:ml-4 [&_li]:list-disc [&_ol]:mb-2 [&_ol]:space-y-0.5 [&_p]:mb-2 [&_p:last-child]:mb-0 [&_strong]:font-semibold [&_ul]:mb-2 [&_ul]:space-y-0.5">
      <ReactMarkdown>{content}</ReactMarkdown>
    </div>
  );
}
