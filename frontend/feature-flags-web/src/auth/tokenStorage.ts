export const TOKEN_KEY = "aff.accessToken";

export function loadToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

export function persistToken(token: string | null): void {
  if (!token) localStorage.removeItem(TOKEN_KEY);
  else localStorage.setItem(TOKEN_KEY, token);
}
