import { useCallback, useEffect, useState } from "react";
import type { FeatureFlagResponse } from "../types";
import { apiCreateFlag, apiDeleteFlag, apiListFlags, apiUpdateFlag } from "../api/client";

export type FeatureFlagDraft = {
  key: string;
  description: string;
  isEnabled: boolean;
  environment: string;
  aiIntegrationHintsJson: string;
};

const initialDraft: FeatureFlagDraft = {
  key: "ai.my_new_capability",
  description: "Describe what this flag gates for AI rollout.",
  isEnabled: true,
  environment: "Development",
  aiIntegrationHintsJson: '{"model":"gpt-family","risk":"low"}',
};

export function useFeatureFlags(
  token: string | null,
  reportError: (message: string | null) => void,
  refreshPublicCatalog: () => Promise<void>,
) {
  const [flags, setFlags] = useState<FeatureFlagResponse[]>([]);
  const [draft, setDraft] = useState<FeatureFlagDraft>(initialDraft);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  useEffect(() => {
    if (!token) {
      setFlags([]);
      setEditingId(null);
      return;
    }

    let cancelled = false;
    (async () => {
      try {
        setBusy(true);
        const rows = await apiListFlags(token);
        if (!cancelled) setFlags(rows);
      } catch (e) {
        if (!cancelled) reportError(e instanceof Error ? e.message : "Failed to load flags.");
      } finally {
        if (!cancelled) setBusy(false);
      }
    })();

    return () => {
      cancelled = true;
    };
  }, [token, reportError]);

  const createFlag = useCallback(async () => {
    if (!token) return;
    reportError(null);
    setBusy(true);
    try {
      const created = await apiCreateFlag(token, draft);
      setFlags((prev) => [created, ...prev]);
      setDraft((d) => ({
        ...d,
        key: `ai.${crypto.randomUUID().replaceAll("-", "").slice(0, 10)}`,
      }));
      await refreshPublicCatalog();
    } catch (e) {
      reportError(e instanceof Error ? e.message : "Create failed.");
    } finally {
      setBusy(false);
    }
  }, [token, draft, reportError, refreshPublicCatalog]);

  const updateFlag = useCallback(
    async (row: FeatureFlagResponse) => {
      if (!token) return;
      reportError(null);
      setBusy(true);
      try {
        const updated = await apiUpdateFlag(token, row.id, {
          description: row.description,
          isEnabled: row.isEnabled,
          environment: row.environment,
          aiIntegrationHintsJson: row.aiIntegrationHintsJson,
        });
        setFlags((prev) => prev.map((x) => (x.id === updated.id ? updated : x)));
        setEditingId(null);
        await refreshPublicCatalog();
      } catch (e) {
        reportError(e instanceof Error ? e.message : "Update failed.");
      } finally {
        setBusy(false);
      }
    },
    [token, reportError, refreshPublicCatalog],
  );

  const deleteFlag = useCallback(
    async (id: string) => {
      if (!token) return;
      reportError(null);
      setBusy(true);
      try {
        await apiDeleteFlag(token, id);
        setFlags((prev) => prev.filter((x) => x.id !== id));
        await refreshPublicCatalog();
      } catch (e) {
        reportError(e instanceof Error ? e.message : "Delete failed.");
      } finally {
        setBusy(false);
      }
    },
    [token, reportError, refreshPublicCatalog],
  );

  return {
    flags,
    draft,
    setDraft,
    editingId,
    setEditingId,
    busy,
    createFlag,
    updateFlag,
    deleteFlag,
  };
}
