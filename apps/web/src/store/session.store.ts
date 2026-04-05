import { create } from "zustand";

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
  reset: () => void;
};

const mockUsers: Record<string, { password: string; user: SessionUser }> = {
  admin: {
    password: "Admin123!",
    user: {
      userAccount: "admin",
      displayName: "Firefly Admin",
      role: "admin",
      email: "admin@firefly.local"
    }
  },
  analyst: {
    password: "Analyst123!",
    user: {
      userAccount: "analyst",
      displayName: "Sample Analyst",
      role: "user",
      email: "analyst@firefly.local"
    }
  }
};

const wait = (durationMs: number) => new Promise((resolve) => window.setTimeout(resolve, durationMs));

export const useSessionStore = create<SessionStore>((set) => ({
  user: null,
  isAuthenticated: false,
  async signIn({ userAccount, password }) {
    await wait(350);

    const record = mockUsers[userAccount.trim().toLowerCase()];
    if (record?.password !== password) {
      throw new Error("Those credentials do not match the current MVP mock sign-in.");
    }

    set({
      user: record.user,
      isAuthenticated: true
    });

    return record.user;
  },
  signOut() {
    set({
      user: null,
      isAuthenticated: false
    });
  },
  reset() {
    set({
      user: null,
      isAuthenticated: false
    });
  }
}));
