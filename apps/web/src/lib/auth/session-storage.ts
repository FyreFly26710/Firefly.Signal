const ACCESS_TOKEN_KEY = "firefly.signal.access-token";
const SESSION_USER_KEY = "firefly.signal.session-user";

export type StoredSessionUser = {
  userAccount: string;
  displayName: string;
  role: "admin" | "user";
  email: string;
};

function getStorage(): Storage | null {
  return typeof window === "undefined" ? null : window.sessionStorage;
}

export function readAccessToken(): string | null {
  return getStorage()?.getItem(ACCESS_TOKEN_KEY) ?? null;
}

export function writeAccessToken(token: string): void {
  getStorage()?.setItem(ACCESS_TOKEN_KEY, token);
}

export function clearAccessToken(): void {
  getStorage()?.removeItem(ACCESS_TOKEN_KEY);
}

export function readStoredUser(): StoredSessionUser | null {
  const value = getStorage()?.getItem(SESSION_USER_KEY);
  if (!value) {
    return null;
  }

  try {
    return JSON.parse(value) as StoredSessionUser;
  } catch {
    clearStoredSession();
    return null;
  }
}

export function writeStoredUser(user: StoredSessionUser): void {
  getStorage()?.setItem(SESSION_USER_KEY, JSON.stringify(user));
}

export function clearStoredSession(): void {
  clearAccessToken();
  getStorage()?.removeItem(SESSION_USER_KEY);
}
