export function envBadgeClass(env: string): string {
  switch (env) {
    case "Development":
      return "badge badge--env-dev";
    case "Staging":
      return "badge badge--env-staging";
    case "Production":
      return "badge badge--env-prod";
    default:
      return "badge badge--off";
  }
}
