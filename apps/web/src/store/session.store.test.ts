import { beforeEach, describe, expect, it, vi } from "vitest";
import { getCurrentUser, login } from "@/api/auth/auth.api";
import { ApiError } from "@/lib/http/api-error";
import { readAccessToken, readStoredUser, writeAccessToken, writeStoredUser } from "@/lib/auth/session-storage";
import { useSessionStore } from "@/store/session.store";

vi.mock("@/api/auth/auth.api", () => ({
  login: vi.fn(),
  getCurrentUser: vi.fn()
}));

describe("useSessionStore", () => {
  beforeEach(() => {
    window.sessionStorage.clear();
    useSessionStore.getState().reset();
    vi.clearAllMocks();
  });

  it("signs in, stores the session, and updates the store", async () => {
    vi.mocked(login).mockResolvedValueOnce({
      accessToken: "token-123",
      user: {
        userAccount: "ada",
        displayName: "Ada Lovelace",
        role: "admin",
        email: "ada@example.com"
      }
    });

    const user = await useSessionStore.getState().signIn({
      userAccount: "ada",
      password: "secret"
    });

    expect(user).toEqual({
      userAccount: "ada",
      displayName: "Ada Lovelace",
      role: "admin",
      email: "ada@example.com"
    });
    expect(useSessionStore.getState().isAuthenticated).toBe(true);
    expect(readAccessToken()).toBe("token-123");
    expect(readStoredUser()).toEqual(user);
  });

  it("maps unauthorized sign-in responses to a friendly error and clears session data", async () => {
    window.sessionStorage.setItem("firefly.signal.access-token", "stale-token");
    window.sessionStorage.setItem(
      "firefly.signal.session-user",
      JSON.stringify({
        userAccount: "old-user",
        displayName: "Old User",
        role: "user",
        email: "old@example.com"
      })
    );
    vi.mocked(login).mockRejectedValueOnce(new ApiError("Unauthorized", 401));

    await expect(
      useSessionStore.getState().signIn({
        userAccount: "ada",
        password: "wrong-password"
      })
    ).rejects.toThrow("Those credentials were rejected by the identity API.");

    expect(useSessionStore.getState().isAuthenticated).toBe(false);
    expect(readAccessToken()).toBeNull();
    expect(readStoredUser()).toBeNull();
  });

  it("hydrates the current user when an access token exists", async () => {
    writeAccessToken("token-123");
    vi.mocked(getCurrentUser).mockResolvedValueOnce({
      userAccount: "sam",
      displayName: null,
      role: "user",
      email: null
    });

    await useSessionStore.getState().hydrate();

    expect(useSessionStore.getState().isAuthenticated).toBe(true);
    expect(useSessionStore.getState().user).toEqual({
      userAccount: "sam",
      displayName: "sam",
      role: "user",
      email: ""
    });
    expect(readStoredUser()).toEqual({
      userAccount: "sam",
      displayName: "sam",
      role: "user",
      email: ""
    });
  });

  it("clears stale session state when hydration fails", async () => {
    writeAccessToken("token-123");
    writeStoredUser({
      userAccount: "stale",
      displayName: "Stale User",
      role: "user",
      email: "stale@example.com"
    });
    vi.mocked(getCurrentUser).mockRejectedValueOnce(new Error("Server unavailable"));

    await useSessionStore.getState().hydrate();

    expect(useSessionStore.getState().user).toBeNull();
    expect(useSessionStore.getState().isAuthenticated).toBe(false);
    expect(readAccessToken()).toBeNull();
    expect(readStoredUser()).toBeNull();
  });
});
