import EditRoundedIcon from "@mui/icons-material/EditRounded";
import SaveRoundedIcon from "@mui/icons-material/SaveRounded";
import { Alert, Box, Button, CircularProgress, Stack, TextField } from "@mui/material";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { upsertCurrentProfile } from "@/api/profile/profile.api";
import type { UserProfileResponseDto } from "@/api/profile/profile.types";
import { AppHeader } from "@/components/AppHeader";
import { MarkdownContent } from "@/components/MarkdownContent";
import { MarkdownField } from "@/components/MarkdownField";
import { SectionCard } from "@/components/SectionCard";
import { useProfile } from "@/features/profile/hooks/useProfile";

type ProfileFormState = {
  fullName: string;
  preferredTitle: string;
  primaryLocationPostcode: string;
  linkedInUrl: string;
  gitHubUrl: string;
  portfolioUrl: string;
  summary: string;
  skillsText: string;
  experienceText: string;
  preferencesText: string;
};

function toFormState(profile: UserProfileResponseDto | null): ProfileFormState {
  return {
    fullName: profile?.fullName ?? "",
    preferredTitle: profile?.preferredTitle ?? "",
    primaryLocationPostcode: profile?.primaryLocationPostcode ?? "",
    linkedInUrl: profile?.linkedInUrl ?? "",
    gitHubUrl: profile?.gitHubUrl ?? "",
    portfolioUrl: profile?.portfolioUrl ?? "",
    summary: profile?.summary ?? "",
    skillsText: profile?.skillsText ?? "",
    experienceText: profile?.experienceText ?? "",
    preferencesText: profile?.preferencesText ?? ""
  };
}

function normalize(value: string): string | null {
  const trimmed = value.trim();
  return trimmed || null;
}

export function ProfileView() {
  const queryClient = useQueryClient();
  const { profile, isLoading, isError, refetch } = useProfile();
  const [isEditing, setIsEditing] = useState(false);
  const [formState, setFormState] = useState<ProfileFormState>(toFormState(null));
  const [saveSuccessful, setSaveSuccessful] = useState(false);

  const mutation = useMutation({
    mutationFn: upsertCurrentProfile,
    onSuccess: (updatedProfile) => {
      queryClient.setQueryData(["profile"], updatedProfile);
      setIsEditing(false);
      setSaveSuccessful(true);
    }
  });

  function enterEditMode() {
    setFormState(toFormState(profile));
    setSaveSuccessful(false);
    setIsEditing(true);
  }

  function cancelEdit() {
    setIsEditing(false);
    mutation.reset();
  }

  function handleSave() {
    mutation.mutate({
      fullName: normalize(formState.fullName),
      preferredTitle: normalize(formState.preferredTitle),
      primaryLocationPostcode: normalize(formState.primaryLocationPostcode),
      linkedInUrl: normalize(formState.linkedInUrl),
      gitHubUrl: normalize(formState.gitHubUrl),
      portfolioUrl: normalize(formState.portfolioUrl),
      summary: normalize(formState.summary),
      skillsText: normalize(formState.skillsText),
      experienceText: normalize(formState.experienceText),
      preferencesText: normalize(formState.preferencesText)
    });
  }

  function setField<K extends keyof ProfileFormState>(key: K, value: string) {
    setFormState((prev) => ({ ...prev, [key]: value }));
  }

  return (
    <div className="min-h-screen bg-background">
      <AppHeader variant="authenticated" />

      <div className="mx-auto max-w-4xl px-5 py-8 sm:px-8">
        {isLoading ? (
          <Box className="flex min-h-56 items-center justify-center">
            <CircularProgress aria-label="Loading profile" />
          </Box>
        ) : isError ? (
          <SectionCard className="p-6 sm:p-8">
            <Stack spacing={2}>
              <Alert severity="error">We couldn't load your profile right now.</Alert>
              <div>
                <Button variant="outlined" onClick={() => void refetch()}>
                  Retry
                </Button>
              </div>
            </Stack>
          </SectionCard>
        ) : isEditing ? (
          <EditForm
            formState={formState}
            setField={setField}
            onCancel={cancelEdit}
            onSave={handleSave}
            isSaving={mutation.isPending}
            saveError={mutation.isError ? "We couldn't save your profile right now." : null}
          />
        ) : (
          <ViewProfile
            profile={profile}
            saveSuccessful={saveSuccessful}
            onEdit={enterEditMode}
          />
        )}
      </div>
    </div>
  );
}

type ViewProfileProps = {
  profile: UserProfileResponseDto | null;
  saveSuccessful: boolean;
  onEdit: () => void;
};

function ViewProfile({ profile, saveSuccessful, onEdit }: ViewProfileProps) {
  const isEmpty = profile === null;

  return (
    <Stack spacing={3}>
      {saveSuccessful && <Alert severity="success">Profile saved.</Alert>}

      <SectionCard className="p-6 sm:p-8">
        {isEmpty ? (
          <div className="flex flex-col items-start gap-4">
            <div>
              <h1 className="font-serif text-2xl font-semibold text-foreground">No profile yet</h1>
              <p className="mt-1 text-sm text-foreground-secondary">
                Add your details so AI analysis can use them as context.
              </p>
            </div>
            <Button
              variant="contained"
              startIcon={<EditRoundedIcon />}
              onClick={onEdit}
              sx={{ bgcolor: "accent.main", "&:hover": { bgcolor: "accent.dark" } }}
            >
              Set up your profile
            </Button>
          </div>
        ) : (
          <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
            <div className="flex items-start gap-4">
              <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-full bg-accent-muted text-lg font-semibold text-foreground">
                {profile.fullName?.charAt(0).toUpperCase() ?? "?"}
              </div>
              <div>
                <h1 className="font-serif text-2xl font-semibold text-foreground">
                  {profile.fullName ?? <span className="text-foreground-secondary italic">No name set</span>}
                </h1>
                {profile.preferredTitle && (
                  <p className="mt-0.5 text-sm font-medium text-foreground-secondary">
                    {profile.preferredTitle}
                  </p>
                )}
                {profile.primaryLocationPostcode && (
                  <p className="mt-0.5 text-sm text-metadata">{profile.primaryLocationPostcode}</p>
                )}
                {(profile.linkedInUrl != null || profile.gitHubUrl != null || profile.portfolioUrl != null) && (
                  <div className="mt-2 flex flex-wrap gap-3 text-xs text-foreground-secondary">
                    {profile.linkedInUrl && (
                      <a
                        href={profile.linkedInUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="hover:text-foreground"
                      >
                        LinkedIn
                      </a>
                    )}
                    {profile.gitHubUrl && (
                      <a
                        href={profile.gitHubUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="hover:text-foreground"
                      >
                        GitHub
                      </a>
                    )}
                    {profile.portfolioUrl && (
                      <a
                        href={profile.portfolioUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="hover:text-foreground"
                      >
                        Portfolio
                      </a>
                    )}
                  </div>
                )}
              </div>
            </div>
            <div className="shrink-0">
              <Button
                variant="outlined"
                size="small"
                startIcon={<EditRoundedIcon />}
                onClick={onEdit}
              >
                Edit profile
              </Button>
            </div>
          </div>
        )}
      </SectionCard>

      {!isEmpty && (
        <>
          {profile.summary && (
            <ProfileSection label="Summary">
              <MarkdownContent content={profile.summary} />
            </ProfileSection>
          )}
          {profile.skillsText && (
            <ProfileSection label="Skills">
              <MarkdownContent content={profile.skillsText} />
            </ProfileSection>
          )}
          {profile.experienceText && (
            <ProfileSection label="Experience">
              <MarkdownContent content={profile.experienceText} />
            </ProfileSection>
          )}
          {profile.preferencesText && (
            <ProfileSection label="Preferences">
              <MarkdownContent content={profile.preferencesText} />
            </ProfileSection>
          )}
        </>
      )}
    </Stack>
  );
}

function ProfileSection({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <SectionCard className="p-6">
      <h2 className="mb-3 text-xs font-semibold uppercase tracking-widest text-foreground-secondary">
        {label}
      </h2>
      {children}
    </SectionCard>
  );
}

type EditFormProps = {
  formState: ProfileFormState;
  setField: <K extends keyof ProfileFormState>(key: K, value: string) => void;
  onCancel: () => void;
  onSave: () => void;
  isSaving: boolean;
  saveError: string | null;
};

function EditForm({ formState, setField, onCancel, onSave, isSaving, saveError }: EditFormProps) {
  return (
    <Stack spacing={3}>
      <SectionCard className="p-6 sm:p-8">
        <div className="mb-6 flex items-center justify-between">
          <h1 className="font-serif text-2xl font-semibold text-foreground">Edit profile</h1>
          <div className="flex gap-2">
            <Button variant="outlined" onClick={onCancel} disabled={isSaving}>
              Cancel
            </Button>
            <Button
              variant="contained"
              startIcon={<SaveRoundedIcon />}
              onClick={onSave}
              disabled={isSaving}
              sx={{ bgcolor: "accent.main", "&:hover": { bgcolor: "accent.dark" } }}
            >
              {isSaving ? "Saving..." : "Save profile"}
            </Button>
          </div>
        </div>

        {saveError && <Alert severity="error" className="mb-4">{saveError}</Alert>}

        <Stack spacing={4}>
          <div>
            <h2 className="mb-3 text-xs font-semibold uppercase tracking-widest text-foreground-secondary">
              Basic info
            </h2>
            <Stack spacing={2}>
              <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
                <TextField
                  fullWidth
                  label="Full name"
                  value={formState.fullName}
                  onChange={(e) => setField("fullName", e.target.value)}
                />
                <TextField
                  fullWidth
                  label="Preferred title"
                  value={formState.preferredTitle}
                  onChange={(e) => setField("preferredTitle", e.target.value)}
                />
              </Stack>
              <TextField
                label="Primary UK postcode"
                value={formState.primaryLocationPostcode}
                onChange={(e) => setField("primaryLocationPostcode", e.target.value)}
                helperText="Used as your default location anchor for job search."
              />
              <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
                <TextField
                  fullWidth
                  label="LinkedIn URL"
                  value={formState.linkedInUrl}
                  onChange={(e) => setField("linkedInUrl", e.target.value)}
                />
                <TextField
                  fullWidth
                  label="GitHub URL"
                  value={formState.gitHubUrl}
                  onChange={(e) => setField("gitHubUrl", e.target.value)}
                />
              </Stack>
              <TextField
                fullWidth
                label="Portfolio URL"
                value={formState.portfolioUrl}
                onChange={(e) => setField("portfolioUrl", e.target.value)}
              />
            </Stack>
          </div>

          <div>
            <h2 className="mb-3 text-xs font-semibold uppercase tracking-widest text-foreground-secondary">
              Profile content
            </h2>
            <Stack spacing={3}>
              <MarkdownField
                label="Summary"
                value={formState.summary}
                onChange={(v) => setField("summary", v)}
                minRows={4}
              />
              <MarkdownField
                label="Skills"
                value={formState.skillsText}
                onChange={(v) => setField("skillsText", v)}
                minRows={4}
              />
              <MarkdownField
                label="Experience"
                value={formState.experienceText}
                onChange={(v) => setField("experienceText", v)}
                minRows={5}
              />
              <MarkdownField
                label="Preferences"
                value={formState.preferencesText}
                onChange={(v) => setField("preferencesText", v)}
                minRows={3}
              />
            </Stack>
          </div>
        </Stack>
      </SectionCard>
    </Stack>
  );
}
