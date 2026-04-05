import { readAccessToken } from "@/lib/auth/session-storage";
import { getApiBaseUrl } from "@/lib/env";
import { ApiError } from "@/lib/http/api-error";

export async function getJson<TResponse>(
  path: string,
  init?: Omit<RequestInit, "method">
): Promise<TResponse> {
  return requestJson<TResponse>(path, {
    ...init,
    method: "GET"
  });
}

export async function postJson<TResponse, TBody>(
  path: string,
  body: TBody,
  init?: Omit<RequestInit, "method" | "body">
): Promise<TResponse> {
  return requestJson<TResponse>(path, {
    ...init,
    method: "POST",
    body: JSON.stringify(body)
  });
}

async function requestJson<TResponse>(path: string, init: RequestInit): Promise<TResponse> {
  const accessToken = readAccessToken();
  const response = await fetch(`${getApiBaseUrl()}${path}`, {
    ...init,
    headers: {
      Accept: "application/json",
      ...(init.body ? { "Content-Type": "application/json" } : {}),
      ...(accessToken ? { Authorization: `Bearer ${accessToken}` } : {}),
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
