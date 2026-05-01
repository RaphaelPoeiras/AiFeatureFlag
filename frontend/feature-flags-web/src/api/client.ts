import type { AuthResponse, FeatureFlagResponse, ApiProblemResponse, PublicFeatureFlagSummary } from "../types";

async function readProblem(response: Response): Promise<string> {
  try {
    const problem = (await response.json()) as Partial<ApiProblemResponse>;
    const code = problem.errorCode ? ` [${problem.errorCode}]` : "";
    return `${problem.detail ?? problem.title ?? response.statusText}${code}`;
  } catch {
    return await response.text();
  }
}

export async function apiRegister(payload: {
  email: string;
  password: string;
  displayName: string;
}): Promise<AuthResponse> {
  const response = await fetch("/api/auth/register", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });

  if (!response.ok) throw new Error(await readProblem(response));
  return (await response.json()) as AuthResponse;
}

export async function apiLogin(payload: { email: string; password: string }): Promise<AuthResponse> {
  const response = await fetch("/api/auth/login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });

  if (!response.ok) throw new Error(await readProblem(response));
  return (await response.json()) as AuthResponse;
}

export async function apiMe(token: string): Promise<{ userId: string; email: string; displayName: string }> {
  const response = await fetch("/api/profile/me", {
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) throw new Error(await readProblem(response));
  return (await response.json()) as { userId: string; email: string; displayName: string };
}

export async function apiListFlags(token: string): Promise<FeatureFlagResponse[]> {
  const response = await fetch("/api/feature-flags", {
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) throw new Error(await readProblem(response));
  return (await response.json()) as FeatureFlagResponse[];
}

export async function apiCreateFlag(
  token: string,
  payload: {
    key: string;
    description: string;
    isEnabled: boolean;
    environment: string;
    aiIntegrationHintsJson: string;
  },
): Promise<FeatureFlagResponse> {
  const response = await fetch("/api/feature-flags", {
    method: "POST",
    headers: { "Content-Type": "application/json", Authorization: `Bearer ${token}` },
    body: JSON.stringify(payload),
  });

  if (!response.ok) throw new Error(await readProblem(response));
  return (await response.json()) as FeatureFlagResponse;
}

export async function apiUpdateFlag(
  token: string,
  id: string,
  payload: {
    description: string;
    isEnabled: boolean;
    environment: string;
    aiIntegrationHintsJson: string;
  },
): Promise<FeatureFlagResponse> {
  const response = await fetch(`/api/feature-flags/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json", Authorization: `Bearer ${token}` },
    body: JSON.stringify(payload),
  });

  if (!response.ok) throw new Error(await readProblem(response));
  return (await response.json()) as FeatureFlagResponse;
}

export async function apiDeleteFlag(token: string, id: string): Promise<void> {
  const response = await fetch(`/api/feature-flags/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) throw new Error(await readProblem(response));
}

export async function apiPublicSummaries(): Promise<PublicFeatureFlagSummary[]> {
  const response = await fetch("/api/public/feature-flags/summaries");
  if (!response.ok) throw new Error(await readProblem(response));
  return (await response.json()) as PublicFeatureFlagSummary[];
}
