import { LogoIcon } from "./icons/LogoIcon";

type SiteHeaderProps = {
  authed: boolean;
  onSignOut: () => void;
};

export function SiteHeader({ authed, onSignOut }: SiteHeaderProps) {
  return (
    <header className="site-header">
      <div className="brand-block">
        <div className="logo-mark">
          <LogoIcon />
        </div>
        <div className="brand-text">
          <h1>AI Feature Flags Console</h1>
          <p>
            Register flags, attach structured AI integration hints, and roll features safely across environments —
            built for teams wiring LLMs into production.
          </p>
        </div>
      </div>

      <div className="header-actions">
        <span className={`status-chip ${authed ? "status-chip--live" : ""}`}>{authed ? "Authenticated" : "Guest mode"}</span>
        {authed ? (
          <button type="button" className="btn btn-ghost" onClick={onSignOut}>
            Sign out
          </button>
        ) : null}
      </div>
    </header>
  );
}
