import LogoutRoundedIcon from "@mui/icons-material/LogoutRounded";
import VerifiedUserRoundedIcon from "@mui/icons-material/VerifiedUserRounded";
import {
  Avatar,
  Button,
  Chip,
  Paper
} from "@mui/material";
import { SearchForm } from "@/features/search/components/SearchForm";
import { SearchResults } from "@/features/search/components/SearchResults";
import { useMockJobSearch } from "@/features/search/hooks/useMockJobSearch";
import { useSessionStore } from "@/store/session.store";
import { useNavigate } from "react-router-dom";
import { useState } from "react";

export function WorkspaceExperience() {
  const navigate = useNavigate();
  const user = useSessionStore((state) => state.user);
  const signOut = useSessionStore((state) => state.signOut);
  const [lastSubmittedPostcode, setLastSubmittedPostcode] = useState("SW1A");
  const [lastSubmittedKeyword, setLastSubmittedKeyword] = useState(".NET");
  const { status, data, errorMessage, runSearch } = useMockJobSearch();

  async function handleSearch(postcode: string, keyword: string) {
    setLastSubmittedPostcode(postcode);
    setLastSubmittedKeyword(keyword);
    await runSearch(postcode, keyword);
  }

  function handleSignOut() {
    signOut();
    void navigate("/login", { replace: true });
  }

  return (
    <div className="flex flex-col gap-6">
      <Paper
        className="rounded-[30px] border border-white/70 bg-white/92 p-5 shadow-[0_18px_80px_rgba(15,23,42,0.08)] sm:p-6"
        elevation={0}
      >
        <div className="flex flex-col gap-5 xl:flex-row xl:items-start xl:justify-between">
          <div className="max-w-3xl">
            <div className="flex flex-wrap items-center gap-3">
              <Chip
                icon={<VerifiedUserRoundedIcon />}
                label="Protected app preview"
                sx={{
                  bgcolor: "rgba(20,83,45,0.08)",
                  color: "#14532d",
                  "& .MuiChip-icon": { color: "#14532d" }
                }}
              />
              <Chip
                label="Mock data only"
                sx={{
                  bgcolor: "rgba(15,23,42,0.06)",
                  color: "#0f172a"
                }}
              />
            </div>

            <h1 className="mt-4 text-4xl font-bold tracking-tight text-slate-950 sm:text-5xl">
              Welcome back, {user?.displayName ?? "operator"}.
            </h1>
            <p className="mt-3 max-w-2xl text-base leading-7 text-slate-600 sm:text-lg">
              This workspace is the UI-first version of the protected app flow. The search pane,
              status panels, and signed-in shell are ready before we swap in live auth and API calls.
            </p>
          </div>

          <div className="flex items-start gap-4 rounded-[28px] border border-slate-200 bg-slate-50 px-4 py-4">
            <Avatar sx={{ bgcolor: "brand.main" }}>
              {(user?.displayName ?? "U").slice(0, 1)}
            </Avatar>
            <div className="min-w-0">
              <p className="font-semibold text-slate-950">{user?.displayName ?? "Unknown user"}</p>
              <p className="text-sm text-slate-600">{user?.email ?? "No email"}</p>
              <p className="mt-1 text-xs uppercase tracking-[0.2em] text-emerald-800">
                {user?.role ?? "user"}
              </p>
            </div>
            <Button
              variant="outlined"
              color="inherit"
              startIcon={<LogoutRoundedIcon />}
              onClick={handleSignOut}
              sx={{ borderColor: "rgba(15,23,42,0.12)" }}
            >
              Sign out
            </Button>
          </div>
        </div>
      </Paper>

      <div className="grid gap-6 xl:grid-cols-[minmax(0,1.6fr)_minmax(300px,380px)]">
        <div className="flex flex-col gap-6">
          <div className="grid gap-6 lg:grid-cols-[minmax(0,420px)_minmax(0,1fr)]">
            <SearchForm
              initialKeyword={lastSubmittedKeyword}
              initialPostcode={lastSubmittedPostcode}
              isSubmitting={status === "loading"}
              onSubmit={handleSearch}
            />
            <SearchResults
              data={data}
              errorMessage={errorMessage}
              keyword={lastSubmittedKeyword}
              postcode={lastSubmittedPostcode}
              status={status}
            />
          </div>
        </div>

        <aside className="flex flex-col gap-6">
          <WorkspacePanel
            eyebrow="Current focus"
            title="Keep the first protected workflow small."
            body="Land sign-in, route protection, and a stable search surface first. The next pass can replace the mock action with the real identity and gateway calls."
          />
          <WorkspacePanel
            eyebrow="Try these"
            title="Useful mock searches"
            body="Use `SW1A + .NET` for a strong result, `M1 + analyst` for another hit, or include `error` in the keyword to exercise the error state."
          />
          <WorkspacePanel
            eyebrow="Why this layout"
            title="Portable across later clients"
            body="The signed-in shell, search states, and account summary are shaped to map onto later Android and iOS experiences without forcing identical UI code."
          />
        </aside>
      </div>
    </div>
  );
}

function WorkspacePanel({
  eyebrow,
  title,
  body
}: {
  eyebrow: string;
  title: string;
  body: string;
}) {
  return (
    <Paper
      className="rounded-[28px] border border-white/70 bg-white/92 p-5 shadow-[0_18px_80px_rgba(15,23,42,0.08)]"
      elevation={0}
    >
      <p className="text-sm font-semibold uppercase tracking-[0.2em] text-emerald-800">{eyebrow}</p>
      <h2 className="mt-2 text-2xl font-bold text-slate-950">{title}</h2>
      <p className="mt-3 text-sm leading-7 text-slate-600">{body}</p>
    </Paper>
  );
}
