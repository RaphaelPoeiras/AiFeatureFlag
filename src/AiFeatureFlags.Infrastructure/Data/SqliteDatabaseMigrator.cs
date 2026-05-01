using Microsoft.Data.Sqlite;

namespace AiFeatureFlags.Infrastructure.Data;

public sealed class SqliteDatabaseMigrator
{
    private readonly IDbConnectionFactory _factory;

    public SqliteDatabaseMigrator(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        await using var conn = await _factory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);

        await using (var pragma = conn.CreateCommand())
        {
            pragma.CommandText = "PRAGMA foreign_keys = ON;";
            await pragma.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = """
                CREATE TABLE IF NOT EXISTS Users (
                  Id TEXT PRIMARY KEY NOT NULL,
                  Email TEXT NOT NULL UNIQUE COLLATE NOCASE,
                  PasswordHash TEXT NOT NULL,
                  DisplayName TEXT NOT NULL,
                  CreatedAtUtc TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS FeatureFlags (
                  Id TEXT PRIMARY KEY NOT NULL,
                  OwnerUserId TEXT NOT NULL,
                  Key TEXT NOT NULL,
                  Description TEXT NOT NULL,
                  IsEnabled INTEGER NOT NULL,
                  Environment TEXT NOT NULL,
                  AiIntegrationHintsJson TEXT NOT NULL,
                  CreatedAtUtc TEXT NOT NULL,
                  UpdatedAtUtc TEXT NOT NULL,
                  UNIQUE (OwnerUserId, Key),
                  FOREIGN KEY (OwnerUserId) REFERENCES Users(Id) ON DELETE CASCADE
                );

                CREATE INDEX IF NOT EXISTS IX_FeatureFlags_Owner ON FeatureFlags(OwnerUserId);
                CREATE INDEX IF NOT EXISTS IX_FeatureFlags_Environment ON FeatureFlags(Environment);
                """;
            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
