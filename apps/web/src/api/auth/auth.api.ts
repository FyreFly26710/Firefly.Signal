import { getJson, postJson } from "@/lib/http/client";
import type {
  AuthenticatedUserResponseDto,
  LoginRequestDto,
  LoginResponseDto
} from "@/api/auth/auth.types";

export async function login(userAccount: string, password: string): Promise<LoginResponseDto> {
  return postJson<LoginResponseDto, LoginRequestDto>("/api/auth/login", {
    userAccount,
    password
  });
}

export async function getCurrentUser(): Promise<AuthenticatedUserResponseDto> {
  return getJson<AuthenticatedUserResponseDto>("/api/auth/me");
}
