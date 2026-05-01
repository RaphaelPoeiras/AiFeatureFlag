using AiFeatureFlags.Infrastructure.Data;
using Microsoft.Data.Sqlite;

namespace AiFeatureFlags.Infrastructure.Tests;

public sealed class SqliteFixture : IDisposable
{
    private readonly string _databasePath;

    public SqliteFixture()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"aff-infra-{Guid.NewGuid():n}.db");

        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = _databasePath,
            ForeignKeys = true
        }.ToString();

        Factory = new SqliteConnectionFactory(connectionString);
        Migrator = new SqliteDatabaseMigrator(Factory);
        Migrator.MigrateAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    public SqliteConnectionFactory Factory { get; }
    public SqliteDatabaseMigrator Migrator { get; }

    public void Dispose()
    {
        TryDelete(_databasePath);
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            // Best-effort cleanup for temp SQLite files on Windows (handles occasionally stay open briefly).
        }
    }
}
