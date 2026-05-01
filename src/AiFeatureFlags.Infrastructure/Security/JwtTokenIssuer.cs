using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AiFeatureFlags.Application.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace AiFeatureFlags.Infrastructure.Security;

public sealed class JwtTokenIssuer : IJwtTokenIssuer
{
    private readonly JwtSettings _settings;

    public JwtTokenIssuer(JwtSettings settings)
    {
        _settings = settings;
    }

    public string IssueAccessToken(Guid userId, string email, string displayName)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_settings.SigningKey);
        if (keyBytes.Length < 32)
            throw new InvalidOperationException("Jwt:SigningKey must be at least 32 UTF-8 bytes.");

        var signingKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new("display_name", displayName)
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
