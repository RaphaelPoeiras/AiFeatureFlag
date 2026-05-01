import { useCallback, useEffect, useState } from "react";
import type { PublicFeatureFlagSummary } from "../types";
import { apiPublicSummaries } from "../api/client";

export function usePublicSummaries() {
  const [summaries, setSummaries] = useState<PublicFeatureFlagSummary[]>([]);

  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        const data = await apiPublicSummaries();
        if (!cancelled) setSummaries(data);
      } catch {
        if (!cancelled) setSummaries([]);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, []);

  const refresh = useCallback(async () => {
    try {
      setSummaries(await apiPublicSummaries());
    } catch {
      // Non-fatal: catalog can stay stale until next refresh.
    }
  }, []);

  return { summaries, refresh };
}
