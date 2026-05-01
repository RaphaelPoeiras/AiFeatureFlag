namespace AiFeatureFlags.Application.Exceptions;

public sealed class UnauthorizedCredentialsException : Exception
{
    public UnauthorizedCredentialsException(string message) : base(message)
    {
    }
}
