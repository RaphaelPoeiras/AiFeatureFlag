import type { FeatureFlagResponse } from "../../types";
import { FlagRow } from "./FlagRow";

type FlagsTableProps = {
  flags: FeatureFlagResponse[];
  editingId: string | null;
  onEditingChange: (id: string | null) => void;
  busy: boolean;
  onSave: (row: FeatureFlagResponse) => void;
  onDelete: (id: string) => void;
};

export function FlagsTable({ flags, editingId, onEditingChange, busy, onSave, onDelete }: FlagsTableProps) {
  return (
    <div className="table-wrap">
      <div className="table-scroll">
        <table className="data-table">
          <thead>
            <tr>
              <th>Key</th>
              <th>Environment</th>
              <th>State</th>
              <th>Description</th>
              <th>AI hints</th>
              <th style={{ width: "1%" }} aria-label="Actions" />
            </tr>
          </thead>
          <tbody>
            {flags.map((row) => (
              <FlagRow
                key={row.id}
                row={row}
                busy={busy}
                editing={editingId === row.id}
                onToggleEdit={() => onEditingChange(editingId === row.id ? null : row.id)}
                onSave={onSave}
                onDelete={() => onDelete(row.id)}
              />
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
