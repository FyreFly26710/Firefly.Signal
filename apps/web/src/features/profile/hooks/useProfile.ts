import { useQuery } from "@tanstack/react-query";
import { getCurrentProfile } from "@/api/profile/profile.api";
import type { UserProfileResponseDto } from "@/api/profile/profile.types";
import { ApiError } from "@/lib/http/api-error";

export function useProfile() {
  const { data, isPending, isError, error, refetch } = useQuery<UserProfileResponseDto | null>({
    queryKey: ["profile"],
    queryFn: async () => {
      try {
        return await getCurrentProfile();
      } catch (err) {
        if (err instanceof ApiError && err.status === 404) {
          return null;
        }
        throw err;
      }
    }
  });

  return {
    profile: data ?? null,
    isLoading: isPending,
    isError,
    errorMessage: isError ? (error instanceof Error ? error.message : "Failed to load profile.") : null,
    refetch
  };
}
