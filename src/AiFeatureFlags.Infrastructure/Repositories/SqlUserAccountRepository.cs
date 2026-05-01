using AiFeatureFlags.Application.Abstractions;
using AiFeatureFlags.Domain.Entities;
using AiFeatureFlags.Infrastructure.Data;
using Microsoft.Data.Sqlite;

namespace AiFeatureFlags.Infrastructure.Repositories;

public sealed class SqlUserAccountRepository : IUserAccountRepository
{
    private readonly IDbConnectionFactory _factory;

    public SqlUserAccountRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        await using var conn = await _factory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT Id, Email, PasswordHash, DisplayName, CreatedAtUtc
            FROM Users
            WHERE Email = $email
            LIMIT 1
            """;
        cmd.Parameters.AddWithValue("$email", email.Trim().ToLowerInvariant());

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return null;

        return Map(reader);
    }

    public async Task<UserAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var conn = await _factory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT Id, Email, PasswordHash, DisplayName, CreatedAtUtc
            FROM Users
            WHERE Id = $id
            LIMIT 1
            """;
        cmd.Parameters.AddWithValue("$id", id.ToString());

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return null;

        return Map(reader);
    }

    public async Task InsertAsync(UserAccount user, CancellationToken cancellationToken)
    {
        await using var conn = await _factory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO Users (Id, Email, PasswordHash, DisplayName, CreatedAtUtc)
            VALUES ($id, $email, $passwordHash, $displayName, $createdAtUtc)
            """;
        cmd.Parameters.AddWithValue("$id", user.Id.ToString());
        cmd.Parameters.AddWithValue("$email", user.Email);
        cmd.Parameters.AddWithValue("$passwordHash", user.PasswordHash);
        cmd.Parameters.AddWithValue("$displayName", user.DisplayName);
        cmd.Parameters.AddWithValue("$createdAtUtc", user.CreatedAtUtc.ToString("O"));

        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        await using var conn = await _factory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT 1
            FROM Users
            WHERE Email = $email
            LIMIT 1
            """;
        cmd.Parameters.AddWithValue("$email", email.Trim().ToLowerInvariant());

        var result = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return result is not null;
    }

    private static UserAccount Map(SqliteDataReader reader)
    {
        return new UserAccount
        {
            Id = Guid.Parse(reader.GetString(0)),
            Email = reader.GetString(1),
            PasswordHash = reader.GetString(2),
            DisplayName = reader.GetString(3),
            CreatedAtUtc = DateTimeOffset.Parse(reader.GetString(4), null, System.Globalization.DateTimeStyles.RoundtripKind)
        };
    }
}
