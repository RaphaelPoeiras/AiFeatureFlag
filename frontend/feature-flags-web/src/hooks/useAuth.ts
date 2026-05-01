import { useEffect, useState } from "react";
import type { AuthMode } from "../types/auth";
import { apiLogin, apiRegister } from "../api/client";
import { loadToken, persistToken } from "../auth/tokenStorage";

export type AuthResult = { ok: true } | { ok: false; message: string };

export function useAuth() {
  const [token, setToken] = useState<string | null>(() => loadToken());
  const [mode, setMode] = useState<AuthMode>("login");
  const [email, setEmail] = useState("demo@example.com");
  const [password, setPassword] = useState("Demo12345!");
  const [displayName, setDisplayName] = useState("Demo User");
  const [authBusy, setAuthBusy] = useState(false);

  useEffect(() => {
    persistToken(token);
  }, [token]);

  async function authenticate(): Promise<AuthResult> {
    setAuthBusy(true);
    try {
      const auth =
        mode === "login"
          ? await apiLogin({ email, password })
          : await apiRegister({ email, password, displayName });
      setToken(auth.accessToken);
      return { ok: true };
    } catch (e) {
      return { ok: false, message: e instanceof Error ? e.message : "Authentication failed." };
    } finally {
      setAuthBusy(false);
    }
  }

  return {
    token,
    setToken,
    mode,
    setMode,
    email,
    setEmail,
    password,
    setPassword,
    displayName,
    setDisplayName,
    authBusy,
    authenticate,
  };
}
