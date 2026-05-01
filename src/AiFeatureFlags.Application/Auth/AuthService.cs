using AiFeatureFlags.Application.Abstractions;
using AiFeatureFlags.Application.Exceptions;
using AiFeatureFlags.Domain.Entities;

namespace AiFeatureFlags.Application.Auth;

public sealed class AuthService
{
    private readonly IUserAccountRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenIssuer _jwt;

    public AuthService(IUserAccountRepository users, IPasswordHasher passwordHasher, IJwtTokenIssuer jwt)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _jwt = jwt;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterUserRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        AuthBusinessRules.ValidateEmail(request.Email);
        AuthBusinessRules.ValidatePassword(request.Password);
        AuthBusinessRules.ValidateDisplayName(request.DisplayName);

        var email = request.Email.Trim().ToLowerInvariant();

        if (await _users.EmailExistsAsync(email, ct).ConfigureAwait(false))
            throw new ConflictException("Email is already registered.");

        var displayName = request.DisplayName.Trim();
        var user = new UserAccount
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            DisplayName = displayName,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        await _users.InsertAsync(user, ct).ConfigureAwait(false);

        var token = _jwt.IssueAccessToken(user.Id, user.Email, user.DisplayName);
        return new AuthResponse(user.Id, user.Email, user.DisplayName, token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        AuthBusinessRules.ValidateEmail(request.Email);
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ApplicationValidationException("Password is required.");

        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _users.GetByEmailAsync(email, ct).ConfigureAwait(false);
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedCredentialsException("Invalid email or password.");

        var token = _jwt.IssueAccessToken(user.Id, user.Email, user.DisplayName);
        return new AuthResponse(user.Id, user.Email, user.DisplayName, token);
    }
}
