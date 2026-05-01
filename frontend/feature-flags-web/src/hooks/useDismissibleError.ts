import { useEffect, useState } from "react";

export function useDismissibleError(autoDismissMs = 9000): [string | null, (message: string | null) => void] {
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!error) return;
    const id = window.setTimeout(() => setError(null), autoDismissMs);
    return () => window.clearTimeout(id);
  }, [error, autoDismissMs]);

  return [error, setError];
}
