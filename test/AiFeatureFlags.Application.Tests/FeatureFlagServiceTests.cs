using AiFeatureFlags.Application.Abstractions;
using AiFeatureFlags.Application.Exceptions;
using AiFeatureFlags.Application.FeatureFlags;
using AiFeatureFlags.Domain.Common;
using AiFeatureFlags.Domain.Entities;
using Moq;

namespace AiFeatureFlags.Application.Tests;

public sealed class FeatureFlagServiceTests
{
    private readonly Mock<IFeatureFlagRepository> _repo = new();
    private readonly FeatureFlagService _sut;

    public FeatureFlagServiceTests()
    {
        _sut = new FeatureFlagService(_repo.Object);
    }

    [Fact]
    public async Task CreateAsync_throws_conflict_when_owner_already_has_key()
    {
        var ownerId = Guid.NewGuid();
        _repo.Setup(r => r.GetByOwnerAndKeyAsync(ownerId, "my.flag", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FeatureFlag
            {
                Id = Guid.NewGuid(),
                OwnerUserId = ownerId,
                Key = "my.flag",
                Description = "x",
                IsEnabled = true,
                Environment = DeploymentEnvironment.Development,
                AiIntegrationHintsJson = "{}",
                CreatedAtUtc = DateTimeOffset.UtcNow,
                UpdatedAtUtc = DateTimeOffset.UtcNow
            });

        await Assert.ThrowsAsync<ConflictException>(() =>
            _sut.CreateAsync(ownerId,
                new CreateFeatureFlagRequest("my.flag", "desc", true, DeploymentEnvironment.Development, "{}"),
                CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_throws_not_found_when_delete_returns_false()
    {
        var ownerId = Guid.NewGuid();
        var id = Guid.NewGuid();
        _repo.Setup(r => r.DeleteAsync(id, ownerId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(ownerId, id, CancellationToken.None));
    }
}
