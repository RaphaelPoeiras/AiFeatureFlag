import type { PublicFeatureFlagSummary } from "../../types";
import { envBadgeClass } from "../../utils/envBadge";

type PublicCatalogProps = {
  summaries: PublicFeatureFlagSummary[];
};

export function PublicCatalog({ summaries }: PublicCatalogProps) {
  return (
    <>
      <p style={{ margin: "0 0 14px", fontSize: "0.84rem", color: "var(--text-muted)", lineHeight: 1.55 }}>
        Anonymous summaries — key, state, and environment only.
      </p>
      {summaries.length === 0 ? (
        <div className="empty-state">Start the API or add flags to see the public catalog.</div>
      ) : (
        <div className="catalog-scroll">
          {summaries.slice(0, 24).map((s) => (
            <div key={`${s.environment}:${s.key}`} className="catalog-item">
              <span className="catalog-key">{s.key}</span>
              <span className={s.isEnabled ? "badge badge--on" : "badge badge--off"}>{s.isEnabled ? "On" : "Off"}</span>
              <div className="catalog-meta">
                <span className={envBadgeClass(s.environment)}>{s.environment}</span>
              </div>
            </div>
          ))}
        </div>
      )}
    </>
  );
}
