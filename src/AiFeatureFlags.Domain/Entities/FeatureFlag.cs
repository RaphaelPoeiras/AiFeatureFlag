namespace AiFeatureFlags.Domain.Entities;

public sealed record FeatureFlag
{
    public required Guid Id { get; init; }
    public required Guid OwnerUserId { get; init; }
    public required string Key { get; init; }
    public required string Description { get; init; }
    public required bool IsEnabled { get; init; }
    public required string Environment { get; init; }
    public required string AiIntegrationHintsJson { get; init; }
    public required DateTimeOffset CreatedAtUtc { get; init; }
    public required DateTimeOffset UpdatedAtUtc { get; init; }
}
