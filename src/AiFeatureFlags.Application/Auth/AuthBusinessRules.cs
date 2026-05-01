using System.Net.Mail;

namespace AiFeatureFlags.Application.Auth;

public static class AuthBusinessRules
{
    public static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new Exceptions.ApplicationValidationException("Email is required.");

        try
        {
            _ = new MailAddress(email.Trim());
        }
        catch
        {
            throw new Exceptions.ApplicationValidationException("Email format is invalid.");
        }
    }

    public static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new Exceptions.ApplicationValidationException("Password is required.");

        if (password.Length < 8)
            throw new Exceptions.ApplicationValidationException("Password must be at least 8 characters.");

        if (password.Length > 128)
            throw new Exceptions.ApplicationValidationException("Password cannot exceed 128 characters.");
    }

    public static void ValidateDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new Exceptions.ApplicationValidationException("Display name is required.");

        if (displayName.Trim().Length > 120)
            throw new Exceptions.ApplicationValidationException("Display name cannot exceed 120 characters.");
    }
}
