namespace AiFeatureFlags.Domain.Common;

public static class DeploymentEnvironment
{
    public const string Development = "Development";
    public const string Staging = "Staging";
    public const string Production = "Production";

    public static readonly IReadOnlyCollection<string> All =
        new[] { Development, Staging, Production };

    public static bool IsKnown(string value) =>
        All.Contains(value, StringComparer.Ordinal);
}
