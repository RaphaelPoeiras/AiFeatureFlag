namespace AiFeatureFlags.Application.Abstractions;

public interface IJwtTokenIssuer
{
    string IssueAccessToken(Guid userId, string email, string displayName);
}
