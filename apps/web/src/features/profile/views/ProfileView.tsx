import SaveRoundedIcon from "@mui/icons-material/SaveRounded";
import { Alert, Box, Button, CircularProgress, Stack, TextField } from "@mui/material";
import type { FormEvent } from "react";
import { useEffect, useState } from "react";
import { AppHeader } from "@/components/AppHeader";
import { SectionCard } from "@/components/SectionCard";
import { getCurrentProfile, upsertCurrentProfile } from "@/api/profile/profile.api";
import type { UserProfileRequestDto, UserProfileResponseDto } from "@/api/profile/profile.types";
import { ApiError } from "@/lib/http/api-error";

type ProfileFormState = {
  fullName: string;
  preferredTitle: string;
  primaryLocationPostcode: string;
  linkedInUrl: string;
  githubUrl: string;
  portfolioUrl: string;
  summary: string;
  skillsText: string;
  experienceText: string;
  preferencesJson: string;
};

const emptyProfileFormState: ProfileFormState = {
  fullName: "",
  preferredTitle: "",
  primaryLocationPostcode: "",
  linkedInUrl: "",
  githubUrl: "",
  portfolioUrl: "",
  summary: "",
  skillsText: "",
  experienceText: "",
  preferencesJson: "{}"
};

function mapProfileToFormState(profile: UserProfileResponseDto): ProfileFormState {
  return {
    fullName: profile.fullName ?? "",
    preferredTitle: profile.preferredTitle ?? "",
    primaryLocationPostcode: profile.primaryLocationPostcode ?? "",
    linkedInUrl: profile.linkedInUrl ?? "",
    githubUrl: profile.githubUrl ?? "",
    portfolioUrl: profile.portfolioUrl ?? "",
    summary: profile.summary ?? "",
    skillsText: profile.skillsText ?? "",
    experienceText: profile.experienceText ?? "",
    preferencesJson: profile.preferencesJson || "{}"
  };
}

function mapFormStateToRequest(input: ProfileFormState): UserProfileRequestDto {
  const normalize = (value: string) => {
    const trimmed = value.trim();
    return trimmed ? trimmed : null;
  };

  return {
    fullName: normalize(input.fullName),
    preferredTitle: normalize(input.preferredTitle),
    primaryLocationPostcode: normalize(input.primaryLocationPostcode),
    linkedInUrl: normalize(input.linkedInUrl),
    githubUrl: normalize(input.githubUrl),
    portfolioUrl: normalize(input.portfolioUrl),
    summary: normalize(input.summary),
    skillsText: normalize(input.skillsText),
    experienceText: normalize(input.experienceText),
    preferencesJson: normalize(input.preferencesJson) ?? "{}"
  };
}

export function ProfileView() {
  const [formState, setFormState] = useState<ProfileFormState>(emptyProfileFormState);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [isEmptyProfile, setIsEmptyProfile] = useState(false);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [saveError, setSaveError] = useState<string | null>(null);
  const [saveMessage, setSaveMessage] = useState<string | null>(null);
  const [reloadKey, setReloadKey] = useState(0);

  useEffect(() => {
    let cancelled = false;

    async function loadProfile() {
      setIsLoading(true);
      setLoadError(null);
      setSaveMessage(null);

      try {
        const profile = await getCurrentProfile();

        if (cancelled) {
          return;
        }

        setFormState(mapProfileToFormState(profile));
        setIsEmptyProfile(false);
      } catch (error) {
        if (cancelled) {
          return;
        }

        if (error instanceof ApiError && error.status === 404) {
          setFormState(emptyProfileFormState);
          setIsEmptyProfile(true);
        } else {
          setLoadError("We couldn't load your profile right now.");
        }
      } finally {
        if (!cancelled) {
          setIsLoading(false);
        }
      }
    }

    void loadProfile();

    return () => {
      cancelled = true;
    };
  }, [reloadKey]);

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    void (async () => {
      setIsSaving(true);
      setSaveError(null);
      setSaveMessage(null);

      try {
        const savedProfile = await upsertCurrentProfile(mapFormStateToRequest(formState));
        setFormState(mapProfileToFormState(savedProfile));
        setIsEmptyProfile(false);
        setSaveMessage("Profile saved.");
      } catch {
        setSaveError("We couldn't save your profile right now.");
      } finally {
        setIsSaving(false);
      }
    })();
  }

  function updateField<Key extends keyof ProfileFormState>(key: Key, value: ProfileFormState[Key]) {
    setFormState((current) => ({
      ...current,
      [key]: value
    }));
  }

  return (
    <div className="min-h-screen bg-background">
      <AppHeader variant="authenticated" />

      <div className="mx-auto max-w-5xl px-5 py-8 sm:px-8">
        <SectionCard className="p-6 sm:p-8">
          <div className="max-w-3xl">
            <h1 className="font-serif text-3xl font-semibold text-foreground">Your profile</h1>
            <p className="mt-3 text-sm leading-7 text-foreground-secondary">
              Keep your stored personal context up to date so job search, saved applications, and
              later AI assistance can use the right details.
            </p>
          </div>
        </SectionCard>

        <SectionCard className="mt-6 p-6 sm:p-8">
          {isLoading ? (
            <Box className="flex min-h-56 items-center justify-center">
              <CircularProgress aria-label="Loading profile" />
            </Box>
          ) : loadError ? (
            <Stack spacing={2}>
              <Alert severity="error">{loadError}</Alert>
              <div>
                <Button variant="outlined" onClick={() => setReloadKey((current) => current + 1)}>
                  Retry
                </Button>
              </div>
            </Stack>
          ) : (
            <form onSubmit={handleSubmit}>
              <Stack spacing={3}>
                {isEmptyProfile ? (
                  <Alert severity="info">
                    No profile has been saved yet. Fill in what matters most and save when you are ready.
                  </Alert>
                ) : null}
                {saveError ? <Alert severity="error">{saveError}</Alert> : null}
                {saveMessage ? <Alert severity="success">{saveMessage}</Alert> : null}

                <Stack direction={{ xs: "column", md: "row" }} spacing={2}>
                  <TextField
                    fullWidth
                    label="Full name"
                    value={formState.fullName}
                    onChange={(event) => updateField("fullName", event.target.value)}
                  />
                  <TextField
                    fullWidth
                    label="Preferred title"
                    value={formState.preferredTitle}
                    onChange={(event) => updateField("preferredTitle", event.target.value)}
                  />
                </Stack>

                <TextField
                  label="Primary UK postcode"
                  value={formState.primaryLocationPostcode}
                  onChange={(event) => updateField("primaryLocationPostcode", event.target.value)}
                  helperText="Used as your default location anchor for future distance-based search."
                />

                <Stack direction={{ xs: "column", md: "row" }} spacing={2}>
                  <TextField
                    fullWidth
                    label="LinkedIn URL"
                    value={formState.linkedInUrl}
                    onChange={(event) => updateField("linkedInUrl", event.target.value)}
                  />
                  <TextField
                    fullWidth
                    label="GitHub URL"
                    value={formState.githubUrl}
                    onChange={(event) => updateField("githubUrl", event.target.value)}
                  />
                </Stack>

                <TextField
                  fullWidth
                  label="Portfolio URL"
                  value={formState.portfolioUrl}
                  onChange={(event) => updateField("portfolioUrl", event.target.value)}
                />

                <TextField
                  fullWidth
                  multiline
                  minRows={4}
                  label="Summary"
                  value={formState.summary}
                  onChange={(event) => updateField("summary", event.target.value)}
                />

                <TextField
                  fullWidth
                  multiline
                  minRows={4}
                  label="Skills"
                  value={formState.skillsText}
                  onChange={(event) => updateField("skillsText", event.target.value)}
                />

                <TextField
                  fullWidth
                  multiline
                  minRows={5}
                  label="Experience"
                  value={formState.experienceText}
                  onChange={(event) => updateField("experienceText", event.target.value)}
                />

                <TextField
                  fullWidth
                  multiline
                  minRows={3}
                  label="Preferences JSON"
                  value={formState.preferencesJson}
                  onChange={(event) => updateField("preferencesJson", event.target.value)}
                  helperText="Keep this light for now. JSON is stored as-is for MVP flexibility."
                />

                <div>
                  <Button
                    type="submit"
                    variant="contained"
                    startIcon={<SaveRoundedIcon />}
                    disabled={isSaving}
                    sx={{
                      bgcolor: "accent.main",
                      "&:hover": { bgcolor: "accent.dark" }
                    }}
                  >
                    {isSaving ? "Saving..." : "Save profile"}
                  </Button>
                </div>
              </Stack>
            </form>
          )}
        </SectionCard>
      </div>
    </div>
  );
}
