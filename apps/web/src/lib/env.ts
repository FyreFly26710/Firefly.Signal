const DEFAULT_API_BASE_URL = "http://localhost:5080";

export function getApiBaseUrl(): string {
  const configuredValue = import.meta.env.VITE_API_BASE_URL as string | undefined;
  return typeof configuredValue === "string" && configuredValue.length > 0
    ? configuredValue
    : DEFAULT_API_BASE_URL;
}
