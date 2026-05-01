using Microsoft.Data.Sqlite;

namespace AiFeatureFlags.Infrastructure.Data;

public interface IDbConnectionFactory
{
    Task<SqliteConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken);
}
