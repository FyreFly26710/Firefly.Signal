import { getJson, putJson } from "@/lib/http/client";
import type { UserProfileRequestDto, UserProfileResponseDto } from "@/api/profile/profile.types";

export async function getCurrentProfile(): Promise<UserProfileResponseDto> {
  return getJson<UserProfileResponseDto>("/api/job-search/profile");
}

export async function upsertCurrentProfile(
  input: UserProfileRequestDto
): Promise<UserProfileResponseDto> {
  return putJson<UserProfileResponseDto, UserProfileRequestDto>("/api/job-search/profile", input);
}
