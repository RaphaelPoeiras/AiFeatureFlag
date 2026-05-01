using AiFeatureFlags.Application.Abstractions;
using AiFeatureFlags.Domain.Entities;
using AiFeatureFlags.Infrastructure.Data;
using Microsoft.Data.Sqlite;

namespace AiFeatureFlags.Infrastructure.Repositories;

public sealed class SqlFeatureFlagRepository : IFeatureFlagRepository
{
    private readonly IDbConnectionFactory _factory;

    public SqlFeatureFlagRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IReadOnlyList<FeatureFlag>> ListForOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken)
    {
        await using var conn = await _factory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT Id, OwnerUserId, Key, Description, IsEnabled, Environment, AiIntegrationHintsJson, CreatedAtUtc, UpdatedAtUtc
            FROM FeatureFlags
            WHERE OwnerUserId = $ownerUserId
            ORDER BY Environment, Key
            """;
        cmd.Parameters.AddWithValue("$ownerUserId", ownerUserId.ToString());

        var results = new List<FeatureFlag>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            results.Add(Map(reader));

        return results;
    }

    public async Task<IReadOnlyList<PublicFeatureFlagSummary>> ListPublicSummariesAsync(CancellationToken cancellationToken)
    {
        await using var conn = await _factory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT Key, IsEnabled, Environment
            FROM FeatureFlags
            ORDER BY Environment, Key
            """;

        var results = new List<PublicFeatureFlagSummary>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            results.Add(new PublicFeatureFlagSummary(
                reader.GetString(0),
                reader.GetInt64(1) == 1,
                reader.GetString(2)));
        }

        return results;
    }

    public async Task<FeatureFlag?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var conn = await _factory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT Id, OwnerUserId, Key, Description, IsEnabled, Environment, AiIntegrationHintsJson, CreatedAtUtc, UpdatedAtUtc
            FROM FeatureFlags
            WHERE Id = $id
            LIMIT 1
            """;
        cmd.Parameters.AddWithValue("$id", id.ToString());

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return null;

        return Map(reader);
    }

    public async Task<FeatureFlag?> GetByOwnerAndKeyAsync(Guid ownerUserId, string key, CancellationToken cancellationToken)
    {
        await using var conn = await _factory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT Id, OwnerUserId, Key, Description, IsEnabled, Environment, AiIntegrationHintsJson, CreatedAtUtc, UpdatedAtUtc
            FROM FeatureFlags
            WHERE OwnerUserId = $ownerUserId AND Key = $key
            LIMIT 1
            """;
        cmd.Parameters.AddWithValue("$ownerUserId", ownerUserId.ToString());
        cmd.Parameters.AddWithValue("$key", key);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return null;

        return Map(reader);
    }

    public async Task InsertAsync(FeatureFlag flag, CancellationToken cancellationToken)
    {
        await using var conn = await _factory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO FeatureFlags (
              Id, OwnerUserId, Key, Description, IsEnabled, Environment, AiIntegrationHintsJson, CreatedAtUtc, UpdatedAtUtc)
            VALUES (
              $id, $ownerUserId, $key, $description, $isEnabled, $environment, $aiHints, $createdAtUtc, $updatedAtUtc)
            """;
        AddFlagParameters(cmd, flag);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> UpdateAsync(FeatureFlag flag, CancellationToken cancellationToken)
    {
        await using var conn = await _factory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            UPDATE FeatureFlags
            SET Description = $description,
                IsEnabled = $isEnabled,
                Environment = $environment,
                AiIntegrationHintsJson = $aiHints,
                UpdatedAtUtc = $updatedAtUtc
            WHERE Id = $id AND OwnerUserId = $ownerUserId
            """;
        cmd.Parameters.AddWithValue("$id", flag.Id.ToString());
        cmd.Parameters.AddWithValue("$ownerUserId", flag.OwnerUserId.ToString());
        cmd.Parameters.AddWithValue("$description", flag.Description);
        cmd.Parameters.AddWithValue("$isEnabled", flag.IsEnabled ? 1 : 0);
        cmd.Parameters.AddWithValue("$environment", flag.Environment);
        cmd.Parameters.AddWithValue("$aiHints", flag.AiIntegrationHintsJson);
        cmd.Parameters.AddWithValue("$updatedAtUtc", flag.UpdatedAtUtc.ToString("O"));

        var affected = await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid ownerUserId, CancellationToken cancellationToken)
    {
        await using var conn = await _factory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            DELETE FROM FeatureFlags
            WHERE Id = $id AND OwnerUserId = $ownerUserId
            """;
        cmd.Parameters.AddWithValue("$id", id.ToString());
        cmd.Parameters.AddWithValue("$ownerUserId", ownerUserId.ToString());

        var affected = await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        return affected > 0;
    }

    private static void AddFlagParameters(SqliteCommand cmd, FeatureFlag flag)
    {
        cmd.Parameters.AddWithValue("$id", flag.Id.ToString());
        cmd.Parameters.AddWithValue("$ownerUserId", flag.OwnerUserId.ToString());
        cmd.Parameters.AddWithValue("$key", flag.Key);
        cmd.Parameters.AddWithValue("$description", flag.Description);
        cmd.Parameters.AddWithValue("$isEnabled", flag.IsEnabled ? 1 : 0);
        cmd.Parameters.AddWithValue("$environment", flag.Environment);
        cmd.Parameters.AddWithValue("$aiHints", flag.AiIntegrationHintsJson);
        cmd.Parameters.AddWithValue("$createdAtUtc", flag.CreatedAtUtc.ToString("O"));
        cmd.Parameters.AddWithValue("$updatedAtUtc", flag.UpdatedAtUtc.ToString("O"));
    }

    private static FeatureFlag Map(SqliteDataReader reader)
    {
        return new FeatureFlag
        {
            Id = Guid.Parse(reader.GetString(0)),
            OwnerUserId = Guid.Parse(reader.GetString(1)),
            Key = reader.GetString(2),
            Description = reader.GetString(3),
            IsEnabled = reader.GetInt64(4) == 1,
            Environment = reader.GetString(5),
            AiIntegrationHintsJson = reader.GetString(6),
            CreatedAtUtc = DateTimeOffset.Parse(reader.GetString(7), null, System.Globalization.DateTimeStyles.RoundtripKind),
            UpdatedAtUtc = DateTimeOffset.Parse(reader.GetString(8), null, System.Globalization.DateTimeStyles.RoundtripKind)
        };
    }
}
