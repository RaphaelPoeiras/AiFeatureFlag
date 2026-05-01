namespace AiFeatureFlags.Application.Exceptions;

/// <summary>
/// Thrown when an authenticated request cannot be tied to a resolvable user identity (e.g. missing claims).
/// </summary>
public sealed class IdentityResolutionException : Exception
{
    public IdentityResolutionException(string message) : base(message)
    {
    }
}
