namespace AiFeatureFlags.Application.FeatureFlags;

public sealed record FeatureFlagResponse(
    Guid Id,
    Guid OwnerUserId,
    string Key,
    string Description,
    bool IsEnabled,
    string Environment,
    string AiIntegrationHintsJson,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record CreateFeatureFlagRequest(
    string Key,
    string Description,
    bool IsEnabled,
    string Environment,
    string AiIntegrationHintsJson);

public sealed record UpdateFeatureFlagRequest(
    string Description,
    bool IsEnabled,
    string Environment,
    string AiIntegrationHintsJson);
