import type { AuthMode } from "../../types/auth";

type AuthFormProps = {
  mode: AuthMode;
  onModeChange: (mode: AuthMode) => void;
  email: string;
  onEmailChange: (value: string) => void;
  password: string;
  onPasswordChange: (value: string) => void;
  displayName: string;
  onDisplayNameChange: (value: string) => void;
  busy: boolean;
  onSubmit: () => void;
};

export function AuthForm({
  mode,
  onModeChange,
  email,
  onEmailChange,
  password,
  onPasswordChange,
  displayName,
  onDisplayNameChange,
  busy,
  onSubmit,
}: AuthFormProps) {
  return (
    <div className="field-grid">
      <div className="segment" role="tablist" aria-label="Authentication mode">
        <button
          type="button"
          data-active={mode === "login" ? "true" : "false"}
          role="tab"
          aria-selected={mode === "login"}
          onClick={() => onModeChange("login")}
        >
          Sign in
        </button>
        <button
          type="button"
          data-active={mode === "register" ? "true" : "false"}
          role="tab"
          aria-selected={mode === "register"}
          onClick={() => onModeChange("register")}
        >
          Register
        </button>
      </div>

      <div className="field">
        <label htmlFor="email">Email</label>
        <input
          id="email"
          className="input"
          value={email}
          onChange={(e) => onEmailChange(e.target.value)}
          autoComplete="email"
        />
      </div>
      <div className="field">
        <label htmlFor="password">Password</label>
        <input
          id="password"
          className="input"
          value={password}
          onChange={(e) => onPasswordChange(e.target.value)}
          type="password"
          autoComplete={mode === "login" ? "current-password" : "new-password"}
        />
      </div>
      {mode === "register" ? (
        <div className="field">
          <label htmlFor="displayName">Display name</label>
          <input
            id="displayName"
            className="input"
            value={displayName}
            onChange={(e) => onDisplayNameChange(e.target.value)}
            autoComplete="name"
          />
        </div>
      ) : null}

      <button type="button" className="btn btn-primary" disabled={busy} onClick={onSubmit}>
        {mode === "login" ? "Continue to console" : "Create account"}
      </button>

      <div className="demo-box">
        <strong>Demo credentials</strong> (Development seed):{" "}
        <span style={{ fontFamily: "var(--font-mono)", fontSize: "0.8rem", color: "var(--cyan)" }}>demo@example.com</span> ·{" "}
        <span style={{ fontFamily: "var(--font-mono)", fontSize: "0.8rem", color: "var(--cyan)" }}>Demo12345!</span>
      </div>
    </div>
  );
}
