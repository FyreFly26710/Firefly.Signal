import { getJson, postJson } from "@/lib/http/client";
import type { StoredSessionUser } from "@/lib/auth/session-storage";

type LoginRequestDto = {
  userAccount: string;
  password: string;
};

type AuthenticatedUserResponseDto = {
  userId: number;
  userAccount: string;
  displayName: string | null;
  email: string | null;
  role: string;
};

type LoginResponseDto = {
  accessToken: string;
  tokenType: string;
  expiresAtUtc: string;
  user: AuthenticatedUserResponseDto;
};

export type LoginResult = {
  accessToken: string;
  user: StoredSessionUser;
};

export async function login(userAccount: string, password: string): Promise<LoginResult> {
  const response = await postJson<LoginResponseDto, LoginRequestDto>("/api/auth/login", {
    userAccount,
    password
  });

  return {
    accessToken: response.accessToken,
    user: mapUser(response.user)
  };
}

export async function getCurrentUser(): Promise<StoredSessionUser> {
  const response = await getJson<AuthenticatedUserResponseDto>("/api/auth/me");
  return mapUser(response);
}

function mapUser(response: AuthenticatedUserResponseDto): StoredSessionUser {
  return {
    userAccount: response.userAccount,
    displayName: response.displayName ?? response.userAccount,
    role: response.role === "admin" ? "admin" : "user",
    email: response.email ?? ""
  };
}
