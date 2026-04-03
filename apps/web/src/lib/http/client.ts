import { getApiBaseUrl } from "@/lib/env";
import { ApiError } from "@/lib/http/api-error";

export async function getJson<TResponse>(
  path: string,
  init?: Omit<RequestInit, "method">
): Promise<TResponse> {
  const response = await fetch(`${getApiBaseUrl()}${path}`, {
    ...init,
    headers: {
      Accept: "application/json",
      ...init?.headers
    }
  });

  if (!response.ok) {
    throw new ApiError(await getErrorMessage(response), response.status);
  }

  return (await response.json()) as TResponse;
}

async function getErrorMessage(response: Response): Promise<string> {
  try {
    const body = (await response.json()) as { detail?: string; title?: string };
    return body.detail ?? body.title ?? "Request failed.";
  } catch {
    return "Request failed.";
  }
}
