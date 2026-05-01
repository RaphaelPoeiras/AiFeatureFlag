using AiFeatureFlags.Domain.Entities;

namespace AiFeatureFlags.Application.Abstractions;

public interface IFeatureFlagRepository
{
    Task<IReadOnlyList<FeatureFlag>> ListForOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken);
    Task<IReadOnlyList<PublicFeatureFlagSummary>> ListPublicSummariesAsync(CancellationToken cancellationToken);
    Task<FeatureFlag?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<FeatureFlag?> GetByOwnerAndKeyAsync(Guid ownerUserId, string key, CancellationToken cancellationToken);
    Task InsertAsync(FeatureFlag flag, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(FeatureFlag flag, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, Guid ownerUserId, CancellationToken cancellationToken);
}

public sealed record PublicFeatureFlagSummary(string Key, bool IsEnabled, string Environment);
