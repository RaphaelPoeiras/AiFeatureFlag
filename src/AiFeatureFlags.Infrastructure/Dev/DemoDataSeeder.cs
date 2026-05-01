using AiFeatureFlags.Application.Abstractions;
using AiFeatureFlags.Domain.Common;
using AiFeatureFlags.Domain.Entities;

namespace AiFeatureFlags.Infrastructure.Dev;

public sealed class DemoDataSeeder
{
    private readonly IUserAccountRepository _users;
    private readonly IFeatureFlagRepository _flags;
    private readonly IPasswordHasher _passwordHasher;

    public DemoDataSeeder(IUserAccountRepository users, IFeatureFlagRepository flags, IPasswordHasher passwordHasher)
    {
        _users = users;
        _flags = flags;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        const string demoEmail = "demo@example.com";
        if (await _users.EmailExistsAsync(demoEmail, cancellationToken).ConfigureAwait(false))
            return;

        var demoUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var demoUser = new UserAccount
        {
            Id = demoUserId,
            Email = demoEmail,
            PasswordHash = _passwordHasher.Hash("Demo12345!"),
            DisplayName = "Demo User",
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        await _users.InsertAsync(demoUser, cancellationToken).ConfigureAwait(false);

        var now = DateTimeOffset.UtcNow;
        var flags = new[]
        {
            new FeatureFlag
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                OwnerUserId = demoUserId,
                Key = "ai.prompt_experiment.v2",
                Description = "Routes eligible tenants to the upgraded prompt template.",
                IsEnabled = true,
                Environment = DeploymentEnvironment.Development,
                AiIntegrationHintsJson = """{"model":"gpt-family","risk":"medium"}""",
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            },
            new FeatureFlag
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                OwnerUserId = demoUserId,
                Key = "ai.streaming_responses",
                Description = "Enables streaming responses for assistant endpoints.",
                IsEnabled = false,
                Environment = DeploymentEnvironment.Staging,
                AiIntegrationHintsJson = "{}",
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            }
        };

        foreach (var flag in flags)
            await _flags.InsertAsync(flag, cancellationToken).ConfigureAwait(false);
    }
}
