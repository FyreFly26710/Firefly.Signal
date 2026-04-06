export type LoginRequestDto = {
  userAccount: string;
  password: string;
};

export type AuthenticatedUserResponseDto = {
  userId: number;
  userAccount: string;
  displayName: string | null;
  email: string | null;
  role: string;
};

export type LoginResponseDto = {
  accessToken: string;
  tokenType: string;
  expiresAtUtc: string;
  user: AuthenticatedUserResponseDto;
};
