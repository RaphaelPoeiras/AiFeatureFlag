import { useCallback } from "react";
import { AuthForm } from "./components/auth/AuthForm";
import { SessionCard } from "./components/auth/SessionCard";
import { PublicCatalog } from "./components/catalog/PublicCatalog";
import { FeatureFlagsPanel } from "./components/flags/FeatureFlagsPanel";
import { AppBackground } from "./components/layout/AppBackground";
import { ErrorToast } from "./components/layout/ErrorToast";
import { Sidebar } from "./components/sidebar/Sidebar";
import { SiteHeader } from "./components/SiteHeader";
import { useAuth } from "./hooks/useAuth";
import { useDismissibleError } from "./hooks/useDismissibleError";
import { usePublicSummaries } from "./hooks/usePublicSummaries";

export default function App() {
  const [error, setError] = useDismissibleError();
  const auth = useAuth();
  const catalog = usePublicSummaries();

  const reportError = useCallback((message: string | null) => {
    setError(message);
  }, [setError]);

  async function handleAuthenticate() {
    setError(null);
    const result = await auth.authenticate();
    if (!result.ok) setError(result.message);
  }

  const authed = Boolean(auth.token);

  return (
    <>
      <AppBackground />

      <div className="shell">
        <SiteHeader authed={authed} onSignOut={() => auth.setToken(null)} />

        <div className="layout-grid">
          <Sidebar
            authTitle={authed ? "Your session" : "Sign in"}
            authContent={
              authed ? (
                <SessionCard email={auth.email} />
              ) : (
                <AuthForm
                  mode={auth.mode}
                  onModeChange={auth.setMode}
                  email={auth.email}
                  onEmailChange={auth.setEmail}
                  password={auth.password}
                  onPasswordChange={auth.setPassword}
                  displayName={auth.displayName}
                  onDisplayNameChange={auth.setDisplayName}
                  busy={auth.authBusy}
                  onSubmit={() => void handleAuthenticate()}
                />
              )
            }
            catalogContent={<PublicCatalog summaries={catalog.summaries} />}
          />

          <main>
            <FeatureFlagsPanel token={auth.token} reportError={reportError} refreshPublicCatalog={catalog.refresh} />
          </main>
        </div>
      </div>

      <ErrorToast message={error} onDismiss={() => setError(null)} />
    </>
  );
}
