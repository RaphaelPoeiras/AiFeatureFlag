using AiFeatureFlags.Application.Abstractions;
using AiFeatureFlags.Application.Exceptions;
using AiFeatureFlags.Domain.Entities;

namespace AiFeatureFlags.Application.FeatureFlags;

public sealed class FeatureFlagService
{
    private readonly IFeatureFlagRepository _flags;

    public FeatureFlagService(IFeatureFlagRepository flags)
    {
        _flags = flags;
    }

    public async Task<IReadOnlyList<FeatureFlagResponse>> ListMineAsync(Guid ownerUserId, CancellationToken ct)
    {
        var rows = await _flags.ListForOwnerAsync(ownerUserId, ct).ConfigureAwait(false);
        return rows.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyList<PublicFeatureFlagSummary>> ListPublicSummariesAsync(CancellationToken ct) =>
        await _flags.ListPublicSummariesAsync(ct).ConfigureAwait(false);

    public async Task<FeatureFlagResponse> CreateAsync(Guid ownerUserId, CreateFeatureFlagRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        FeatureFlagBusinessRules.ValidateKey(request.Key);
        FeatureFlagBusinessRules.ValidateEnvironment(request.Environment);
        FeatureFlagBusinessRules.ValidateDescription(request.Description ?? string.Empty);
        FeatureFlagBusinessRules.ValidateAiHintsJson(request.AiIntegrationHintsJson ?? "{}");

        var key = request.Key.Trim();
        var env = request.Environment.Trim();

        var existing = await _flags.GetByOwnerAndKeyAsync(ownerUserId, key, ct).ConfigureAwait(false);
        if (existing is not null)
            throw new ConflictException($"You already own flag '{key}'.");

        var now = DateTimeOffset.UtcNow;
        var entity = new FeatureFlag
        {
            Id = Guid.NewGuid(),
            OwnerUserId = ownerUserId,
            Key = key,
            Description = (request.Description ?? string.Empty).Trim(),
            IsEnabled = request.IsEnabled,
            Environment = env,
            AiIntegrationHintsJson = string.IsNullOrWhiteSpace(request.AiIntegrationHintsJson)
                ? "{}"
                : request.AiIntegrationHintsJson.Trim(),
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        await _flags.InsertAsync(entity, ct).ConfigureAwait(false);
        return ToResponse(entity);
    }

    public async Task<FeatureFlagResponse> UpdateAsync(Guid ownerUserId, Guid id, UpdateFeatureFlagRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        FeatureFlagBusinessRules.ValidateEnvironment(request.Environment);
        FeatureFlagBusinessRules.ValidateDescription(request.Description ?? string.Empty);
        FeatureFlagBusinessRules.ValidateAiHintsJson(request.AiIntegrationHintsJson ?? "{}");

        var current = await _flags.GetByIdAsync(id, ct).ConfigureAwait(false);
        if (current is null || current.OwnerUserId != ownerUserId)
            throw new NotFoundException("Feature flag was not found.");

        var env = request.Environment.Trim();
        var hints = string.IsNullOrWhiteSpace(request.AiIntegrationHintsJson)
            ? "{}"
            : request.AiIntegrationHintsJson.Trim();

        var updated = current with
        {
            Description = (request.Description ?? string.Empty).Trim(),
            IsEnabled = request.IsEnabled,
            Environment = env,
            AiIntegrationHintsJson = hints,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        var ok = await _flags.UpdateAsync(updated, ct).ConfigureAwait(false);
        if (!ok)
            throw new NotFoundException("Feature flag was not found.");

        return ToResponse(updated);
    }

    public async Task DeleteAsync(Guid ownerUserId, Guid id, CancellationToken ct)
    {
        var ok = await _flags.DeleteAsync(id, ownerUserId, ct).ConfigureAwait(false);
        if (!ok)
            throw new NotFoundException("Feature flag was not found.");
    }

    private static FeatureFlagResponse ToResponse(FeatureFlag f) =>
        new(
            f.Id,
            f.OwnerUserId,
            f.Key,
            f.Description,
            f.IsEnabled,
            f.Environment,
            f.AiIntegrationHintsJson,
            f.CreatedAtUtc,
            f.UpdatedAtUtc);
}
