import { useEffect, useState } from "react";
import type { FeatureFlagResponse } from "../../types";
import { envBadgeClass } from "../../utils/envBadge";

export type FlagRowProps = {
  row: FeatureFlagResponse;
  editing: boolean;
  busy: boolean;
  onToggleEdit: () => void;
  onSave: (row: FeatureFlagResponse) => void;
  onDelete: () => void;
};

export function FlagRow({ row, editing, busy, onToggleEdit, onSave, onDelete }: FlagRowProps) {
  const [local, setLocal] = useState(row);

  useEffect(() => {
    setLocal(row);
  }, [row]);

  if (!editing) {
    return (
      <tr>
        <td className="cell-key">{row.key}</td>
        <td>
          <span className={envBadgeClass(row.environment)}>{row.environment}</span>
        </td>
        <td>
          <span className={row.isEnabled ? "badge badge--on" : "badge badge--off"}>{row.isEnabled ? "On" : "Off"}</span>
        </td>
        <td>{row.description}</td>
        <td className="cell-json">{row.aiIntegrationHintsJson}</td>
        <td className="cell-actions">
          <div className="btn-row">
            <button type="button" className="btn btn-ghost" disabled={busy} onClick={onToggleEdit}>
              Edit
            </button>
            <button type="button" className="btn btn-danger" disabled={busy} onClick={onDelete}>
              Delete
            </button>
          </div>
        </td>
      </tr>
    );
  }

  return (
    <tr>
      <td className="cell-key">{local.key}</td>
      <td>
        <select className="select" value={local.environment} onChange={(e) => setLocal((r) => ({ ...r, environment: e.target.value }))}>
          <option>Development</option>
          <option>Staging</option>
          <option>Production</option>
        </select>
      </td>
      <td>
        <select
          className="select"
          value={local.isEnabled ? "true" : "false"}
          onChange={(e) => setLocal((r) => ({ ...r, isEnabled: e.target.value === "true" }))}
        >
          <option value="true">On</option>
          <option value="false">Off</option>
        </select>
      </td>
      <td>
        <textarea className="textarea" value={local.description} onChange={(e) => setLocal((r) => ({ ...r, description: e.target.value }))} rows={3} />
      </td>
      <td>
        <textarea
          className="textarea mono-input"
          value={local.aiIntegrationHintsJson}
          onChange={(e) => setLocal((r) => ({ ...r, aiIntegrationHintsJson: e.target.value }))}
          rows={4}
          spellCheck={false}
        />
      </td>
      <td className="cell-actions">
        <div className="btn-row">
          <button type="button" className="btn btn-primary" disabled={busy} onClick={() => onSave(local)}>
            Save
          </button>
          <button type="button" className="btn btn-ghost" disabled={busy} onClick={onToggleEdit}>
            Cancel
          </button>
        </div>
      </td>
    </tr>
  );
}
