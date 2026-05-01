export function initialsFromEmail(email: string): string {
  const local = email.split("@")[0]?.trim() ?? "?";
  if (local.length >= 2) return (local[0] + local[1]).toUpperCase();
  return (local[0] ?? "?").toUpperCase();
}
