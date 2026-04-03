import { Alert, Chip, CircularProgress, Paper } from "@mui/material";
import LaunchRoundedIcon from "@mui/icons-material/LaunchRounded";
import type { SearchStatus, SearchViewModel } from "@/features/search/types/search.types";

type SearchResultsProps = {
  status: SearchStatus;
  data: SearchViewModel | null;
  errorMessage: string | null;
  postcode: string;
  keyword: string;
};

export function SearchResults({ status, data, errorMessage, postcode, keyword }: SearchResultsProps) {
  return (
    <Paper
      className="rounded-[28px] border border-white/60 bg-slate-950 p-5 text-slate-50 shadow-[0_18px_80px_rgba(15,23,42,0.18)] sm:p-6"
      elevation={0}
    >
      <div className="flex flex-col gap-2 border-b border-white/10 pb-5">
        <p className="text-sm font-semibold uppercase tracking-[0.2em] text-emerald-300">Results</p>
        <h2 className="text-2xl font-bold">What looks promising around {postcode}</h2>
        <p className="text-sm leading-6 text-slate-300">
          Keyword focus: <span className="font-semibold text-white">{keyword}</span>
        </p>
      </div>

      <div className="pt-5">
        {status === "idle" ? (
          <EmptyPanel message="Search to populate this view with the latest results from your backend." />
        ) : null}

        {status === "loading" ? (
          <div className="flex min-h-64 flex-col items-center justify-center gap-4 text-center text-slate-300">
            <CircularProgress color="inherit" size={32} />
            <p>Looking up matching roles and preparing the first page of results.</p>
          </div>
        ) : null}

        {status === "error" ? (
          <Alert severity="error" variant="filled">
            {errorMessage ?? "The search could not be completed."}
          </Alert>
        ) : null}

        {status === "empty" ? (
          <EmptyPanel message="No matching jobs came back for that combination yet. Try a nearby postcode or a broader keyword." />
        ) : null}

        {status === "success" && data ? (
          <div className="flex flex-col gap-4">
            <div className="flex items-center justify-between gap-3 text-sm text-slate-300">
              <p>
                <span className="font-semibold text-white">{data.totalCount}</span> results returned
              </p>
              <Chip
                label={`${data.postcode} · ${data.keyword}`}
                size="small"
                sx={{
                  bgcolor: "rgba(255,255,255,0.08)",
                  color: "#f8fafc",
                  fontWeight: 600
                }}
              />
            </div>

            {data.jobs.map((job) => (
              <article
                key={job.id}
                className="rounded-3xl border border-white/10 bg-white/[0.04] p-5 transition hover:border-emerald-400/40 hover:bg-white/[0.06]"
              >
                <div className="flex flex-col gap-4">
                  <div className="flex flex-col gap-3 lg:flex-row lg:items-start lg:justify-between">
                    <div>
                      <h3 className="text-xl font-semibold text-white">{job.title}</h3>
                      <p className="mt-1 text-sm text-slate-300">
                        {job.company} · {job.locationLabel}
                      </p>
                    </div>
                    <div className="flex flex-wrap gap-2">
                      <Chip
                        label={job.sourceLabel}
                        size="small"
                        sx={{ bgcolor: "rgba(16,185,129,0.14)", color: "#d1fae5" }}
                      />
                      <Chip
                        label={job.postedLabel}
                        size="small"
                        sx={{ bgcolor: "rgba(255,255,255,0.08)", color: "#f8fafc" }}
                      />
                    </div>
                  </div>

                  <p className="max-w-3xl text-sm leading-7 text-slate-200">{job.summary}</p>

                  <div>
                    <a
                      className="inline-flex items-center gap-2 text-sm font-semibold text-amber-300 transition hover:text-amber-200"
                      href={job.url}
                      target="_blank"
                      rel="noreferrer"
                    >
                      View job source
                      <LaunchRoundedIcon fontSize="inherit" />
                    </a>
                  </div>
                </div>
              </article>
            ))}
          </div>
        ) : null}
      </div>
    </Paper>
  );
}

function EmptyPanel({ message }: { message: string }) {
  return (
    <div className="flex min-h-64 items-center justify-center rounded-[24px] border border-dashed border-white/15 bg-white/[0.03] p-6 text-center text-sm leading-7 text-slate-300">
      <p className="max-w-xl">{message}</p>
    </div>
  );
}
