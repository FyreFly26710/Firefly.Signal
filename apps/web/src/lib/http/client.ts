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

export async function putJson<TResponse, TBody>(
  path: string,
  body: TBody,
  init?: Omit<RequestInit, "method" | "body">
): Promise<TResponse> {
  return requestJson<TResponse>(path, {
    ...init,
    method: "PUT",
    body: JSON.stringify(body)
  });
}

export async function postFormData<TResponse>(
  path: string,
  body: FormData,
  init?: Omit<RequestInit, "method" | "body">
): Promise<TResponse> {
  return requestJson<TResponse>(path, {
    ...init,
    method: "POST",
    body
  });
}

export async function getBlob(
  path: string,
  init?: Omit<RequestInit, "method">
): Promise<Blob> {
  const response = await request(path, {
    ...init,
    method: "GET"
  });

  return response.blob();
}

export async function deleteRequest(
  path: string,
  init?: Omit<RequestInit, "method">
): Promise<void> {
  await request(path, {
    ...init,
    method: "DELETE"
  });
}

export async function deleteJson<TResponse, TBody>(
  path: string,
  body: TBody,
  init?: Omit<RequestInit, "method" | "body">
): Promise<TResponse> {
  return requestJson<TResponse>(path, {
    ...init,
    method: "DELETE",
    body: JSON.stringify(body)
  });
}

async function requestJson<TResponse>(path: string, init: RequestInit): Promise<TResponse> {
  const response = await request(path, init);

  if (response.status === 204) {
    return null as TResponse;
  }

  return (await response.json()) as TResponse;
}

async function request(path: string, init: RequestInit): Promise<Response> {
  const accessToken = readAccessToken();
  const response = await fetch(`${getApiBaseUrl()}${path}`, {
    ...init,
    headers: {
      Accept: "application/json",
      ...(init.body && !(init.body instanceof FormData) ? { "Content-Type": "application/json" } : {}),
      ...(accessToken ? { Authorization: `Bearer ${accessToken}` } : {}),
      ...init?.headers
    }
  });

  if (!response.ok) {
    throw new ApiError(await getErrorMessage(response), response.status);
  }

  return response;
}

async function getErrorMessage(response: Response): Promise<string> {
  try {
    const body = (await response.json()) as { detail?: string; title?: string };
    return body.detail ?? body.title ?? "Request failed.";
  } catch {
    return "Request failed.";
  }
}
