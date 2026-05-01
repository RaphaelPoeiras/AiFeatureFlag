using AiFeatureFlags.Domain.Common;
using AiFeatureFlags.Domain.Entities;
using AiFeatureFlags.Infrastructure.Repositories;

namespace AiFeatureFlags.Infrastructure.Tests;

public sealed class SqlRepositoriesTests : IClassFixture<SqliteFixture>
{
    private readonly SqliteFixture _fx;

    public SqlRepositoriesTests(SqliteFixture fx)
    {
        _fx = fx;
    }

    [Fact]
    public async Task Users_and_feature_flags_round_trip_with_foreign_keys()
    {
        var users = new SqlUserAccountRepository(_fx.Factory);
        var flags = new SqlFeatureFlagRepository(_fx.Factory);

        var userId = Guid.NewGuid();
        await users.InsertAsync(new UserAccount
        {
            Id = userId,
            Email = $"integration-{userId:N}@example.com",
            PasswordHash = "hash",
            DisplayName = "Integration User",
            CreatedAtUtc = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var fetchedUser = await users.GetByEmailAsync($"integration-{userId:N}@example.com", CancellationToken.None);
        Assert.NotNull(fetchedUser);
        Assert.Equal(userId, fetchedUser!.Id);

        var flagId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        await flags.InsertAsync(new FeatureFlag
        {
            Id = flagId,
            OwnerUserId = userId,
            Key = $"integration.flag.{userId:N}",
            Description = "integration test flag",
            IsEnabled = true,
            Environment = DeploymentEnvironment.Development,
            AiIntegrationHintsJson = "{}",
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        }, CancellationToken.None);

        var mine = await flags.ListForOwnerAsync(userId, CancellationToken.None);
        Assert.Single(mine);
        Assert.Equal(flagId, mine[0].Id);

        var summaries = await flags.ListPublicSummariesAsync(CancellationToken.None);
        Assert.Contains(summaries, s => s.Key == $"integration.flag.{userId:N}");

        var updated = mine[0] with { Description = "updated", UpdatedAtUtc = DateTimeOffset.UtcNow };
        Assert.True(await flags.UpdateAsync(updated, CancellationToken.None));

        Assert.True(await flags.DeleteAsync(flagId, userId, CancellationToken.None));
    }
}
