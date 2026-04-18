export type UserProfileRequestDto = {
  fullName: string | null;
  preferredTitle: string | null;
  primaryLocationPostcode: string | null;
  linkedInUrl: string | null;
  gitHubUrl: string | null;
  portfolioUrl: string | null;
  summary: string | null;
  skillsText: string | null;
  experienceText: string | null;
  preferencesText: string | null;
};

export type UserProfileResponseDto = {
  id: number;
  userAccountId: number;
  fullName: string | null;
  preferredTitle: string | null;
  primaryLocationPostcode: string | null;
  linkedInUrl: string | null;
  gitHubUrl: string | null;
  portfolioUrl: string | null;
  summary: string | null;
  skillsText: string | null;
  experienceText: string | null;
  preferencesText: string | null;
  createdAtUtc: string;
  updatedAtUtc: string;
};
