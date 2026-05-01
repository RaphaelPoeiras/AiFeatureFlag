using AiFeatureFlags.Application.Abstractions;
using AiFeatureFlags.Application.Auth;
using AiFeatureFlags.Application.Exceptions;
using AiFeatureFlags.Domain.Entities;
using Moq;

namespace AiFeatureFlags.Application.Tests;

public sealed class AuthServiceTests
{
    private readonly Mock<IUserAccountRepository> _users = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<IJwtTokenIssuer> _jwt = new();

    private AuthService CreateSut() => new(_users.Object, _hasher.Object, _jwt.Object);

    [Fact]
    public async Task LoginAsync_throws_when_password_invalid()
    {
        var sut = CreateSut();
        var email = "user@example.com";
        var user = new UserAccount
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = "hash",
            DisplayName = "User",
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        _users.Setup(u => u.GetByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("wrong", user.PasswordHash)).Returns(false);

        await Assert.ThrowsAsync<UnauthorizedCredentialsException>(() =>
            sut.LoginAsync(new LoginRequest(email, "wrong"), CancellationToken.None));
    }

    [Fact]
    public async Task RegisterAsync_throws_when_email_already_exists()
    {
        var sut = CreateSut();
        _users.Setup(u => u.EmailExistsAsync("dup@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(() =>
            sut.RegisterAsync(new RegisterUserRequest("dup@example.com", "password123", "Dup User"),
                CancellationToken.None));
    }
}
