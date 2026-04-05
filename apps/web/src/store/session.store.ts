import { create } from "zustand";
import { getCurrentUser, login } from "@/features/auth/api/auth.api";
import {
  clearStoredSession,
  readAccessToken,
  readStoredUser,
  writeAccessToken,
  writeStoredUser
} from "@/lib/auth/session-storage";
import { ApiError } from "@/lib/http/api-error";

export type SessionUser = {
  userAccount: string;
  displayName: string;
  role: "admin" | "user";
  email: string;
};

type SignInInput = {
  userAccount: string;
  password: string;
};

type SessionStore = {
  user: SessionUser | null;
  isAuthenticated: boolean;
  signIn: (input: SignInInput) => Promise<SessionUser>;
  signOut: () => void;
  hydrate: () => Promise<void>;
  reset: () => void;
};

const storedUser = readStoredUser();
const storedToken = readAccessToken();

export const useSessionStore = create<SessionStore>((set) => ({
  user: storedUser,
  isAuthenticated: Boolean(storedUser && storedToken),
  async signIn({ userAccount, password }) {
    try {
      const result = await login(userAccount, password);

      writeAccessToken(result.accessToken);
      writeStoredUser(result.user);

      set({
        user: result.user,
        isAuthenticated: true
      });

      return result.user;
    } catch (error) {
      clearStoredSession();
      set({
        user: null,
        isAuthenticated: false
      });

      if (error instanceof ApiError && error.status === 401) {
        throw new Error("Those credentials were rejected by the identity API.");
      }

      throw error;
    }
  },
  signOut() {
    clearStoredSession();
    set({
      user: null,
      isAuthenticated: false
    });
  },
  async hydrate() {
    if (!readAccessToken()) {
      return;
    }

    try {
      const user = await getCurrentUser();
      writeStoredUser(user);

      set({
        user,
        isAuthenticated: true
      });
    } catch {
      clearStoredSession();
      set({
        user: null,
        isAuthenticated: false
      });
    }
  },
  reset() {
    clearStoredSession();
    set({
      user: null,
      isAuthenticated: false
    });
  }
}));
