export type UserProfileRequestDto = {
  fullName: string | null;
  preferredTitle: string | null;
  primaryLocationPostcode: string | null;
  linkedInUrl: string | null;
  githubUrl: string | null;
  portfolioUrl: string | null;
  summary: string | null;
  skillsText: string | null;
  experienceText: string | null;
  preferencesJson: string | null;
};

export type UserProfileResponseDto = {
  id: number;
  userAccountId: number;
  fullName: string | null;
  preferredTitle: string | null;
  primaryLocationPostcode: string | null;
  linkedInUrl: string | null;
  githubUrl: string | null;
  portfolioUrl: string | null;
  summary: string | null;
  skillsText: string | null;
  experienceText: string | null;
  preferencesJson: string;
  createdAtUtc: string;
  updatedAtUtc: string;
};
