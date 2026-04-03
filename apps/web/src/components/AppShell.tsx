import type { PropsWithChildren } from "react";

type AppShellProps = PropsWithChildren<{
  eyebrow?: string;
  title: string;
  subtitle: string;
}>;

export function AppShell({ eyebrow, title, subtitle, children }: AppShellProps) {
  return (
    <div className="min-h-screen bg-[radial-gradient(circle_at_top,_rgba(20,83,45,0.18),_transparent_34%),linear-gradient(180deg,_#f7fbf9_0%,_#edf4ef_100%)] text-slate-900">
      <div className="mx-auto flex min-h-screen w-full max-w-7xl flex-col px-4 py-6 sm:px-6 lg:px-8">
        <header className="rounded-[28px] border border-white/70 bg-white/85 px-6 py-8 shadow-[0_18px_80px_rgba(15,23,42,0.08)] backdrop-blur">
          {eyebrow ? (
            <p className="text-sm font-semibold uppercase tracking-[0.22em] text-emerald-800/80">
              {eyebrow}
            </p>
          ) : null}
          <div className="mt-3 flex max-w-3xl flex-col gap-3">
            <h1 className="font-[Segoe_UI] text-4xl font-bold tracking-tight text-slate-950 sm:text-5xl">
              {title}
            </h1>
            <p className="max-w-2xl text-base leading-7 text-slate-600 sm:text-lg">{subtitle}</p>
          </div>
        </header>

        <main className="flex-1 py-6">{children}</main>
      </div>
    </div>
  );
}
