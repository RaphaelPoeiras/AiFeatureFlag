import type { Dispatch, SetStateAction } from "react";
import type { FeatureFlagDraft } from "../../hooks/useFeatureFlags";

type CreateFlagFormProps = {
  draft: FeatureFlagDraft;
  onDraftChange: Dispatch<SetStateAction<FeatureFlagDraft>>;
  busy: boolean;
  onCreate: () => void;
};

export function CreateFlagForm({ draft, onDraftChange, busy, onCreate }: CreateFlagFormProps) {
  return (
    <>
      <div className="section-tag">Create flag</div>
      <div className="form-split">
        <div className="field">
          <label htmlFor="flag-key">Flag key</label>
          <input
            id="flag-key"
            className="input mono-input"
            value={draft.key}
            onChange={(e) => onDraftChange((d) => ({ ...d, key: e.target.value }))}
            spellCheck={false}
          />
        </div>
        <div className="field">
          <label htmlFor="flag-env">Environment</label>
          <select
            id="flag-env"
            className="select"
            value={draft.environment}
            onChange={(e) => onDraftChange((d) => ({ ...d, environment: e.target.value }))}
          >
            <option>Development</option>
            <option>Staging</option>
            <option>Production</option>
          </select>
        </div>
        <div className="field">
          <label htmlFor="flag-enabled">Enabled</label>
          <select
            id="flag-enabled"
            className="select"
            value={draft.isEnabled ? "true" : "false"}
            onChange={(e) => onDraftChange((d) => ({ ...d, isEnabled: e.target.value === "true" }))}
          >
            <option value="true">Yes</option>
            <option value="false">No</option>
          </select>
        </div>
        <div className="field field--full">
          <label htmlFor="flag-desc">Description</label>
          <textarea
            id="flag-desc"
            className="textarea"
            value={draft.description}
            onChange={(e) => onDraftChange((d) => ({ ...d, description: e.target.value }))}
            rows={3}
          />
        </div>
        <div className="field field--full">
          <label htmlFor="flag-json">AI integration hints (JSON)</label>
          <textarea
            id="flag-json"
            className="textarea mono-input"
            value={draft.aiIntegrationHintsJson}
            onChange={(e) => onDraftChange((d) => ({ ...d, aiIntegrationHintsJson: e.target.value }))}
            rows={4}
            spellCheck={false}
          />
        </div>
        <div className="field field--full">
          <button type="button" className="btn btn-primary" disabled={busy} onClick={onCreate}>
            Create flag
          </button>
        </div>
      </div>
    </>
  );
}
