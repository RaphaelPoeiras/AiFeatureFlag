import { useFeatureFlags } from "../../hooks/useFeatureFlags";
import { CreateFlagForm } from "./CreateFlagForm";
import { EmptyFlagsPlaceholder } from "./EmptyFlagsPlaceholder";
import { FlagsTable } from "./FlagsTable";

type FeatureFlagsPanelProps = {
  token: string | null;
  reportError: (message: string | null) => void;
  refreshPublicCatalog: () => Promise<void>;
};

export function FeatureFlagsPanel({ token, reportError, refreshPublicCatalog }: FeatureFlagsPanelProps) {
  const { flags, draft, setDraft, editingId, setEditingId, busy, createFlag, updateFlag, deleteFlag } = useFeatureFlags(
    token,
    reportError,
    refreshPublicCatalog,
  );

  return (
    <div className="card">
      <div className="card__header">
        <h2 className="card__title">Feature flags</h2>
      </div>
      <div className="card__body">
        {!token ? (
          <p className="panel-hint">
            <strong>Sign in</strong> to create, edit, and delete flags via{" "}
            <span style={{ fontFamily: "var(--font-mono)", fontSize: "0.82rem", opacity: 0.85 }}>/api/feature-flags</span>.
          </p>
        ) : (
          <>
            <CreateFlagForm draft={draft} onDraftChange={setDraft} busy={busy} onCreate={() => void createFlag()} />

            <div className="section-tag" style={{ marginTop: 28 }}>
              Your flags
            </div>

            {flags.length === 0 ? (
              <EmptyFlagsPlaceholder />
            ) : (
              <FlagsTable
                flags={flags}
                editingId={editingId}
                onEditingChange={setEditingId}
                busy={busy}
                onSave={(row) => void updateFlag(row)}
                onDelete={(id) => void deleteFlag(id)}
              />
            )}
          </>
        )}
      </div>
    </div>
  );
}
