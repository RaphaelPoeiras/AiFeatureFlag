using System.Text.Json;
using System.Text.RegularExpressions;
using AiFeatureFlags.Domain.Common;

namespace AiFeatureFlags.Application.FeatureFlags;

public static class FeatureFlagBusinessRules
{
    private static readonly Regex KeyRegex = new(
        "^[a-z][a-z0-9_.-]{2,63}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static void ValidateKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new Exceptions.ApplicationValidationException("Flag key is required.");

        var trimmed = key.Trim();
        if (!KeyRegex.IsMatch(trimmed))
            throw new Exceptions.ApplicationValidationException(
                "Flag key must start with a lowercase letter and contain 3-64 chars of lowercase letters, digits, dots, underscores, or hyphens.");
    }

    public static void ValidateEnvironment(string environment)
    {
        if (string.IsNullOrWhiteSpace(environment))
            throw new Exceptions.ApplicationValidationException("Environment is required.");

        if (!DeploymentEnvironment.IsKnown(environment.Trim()))
            throw new Exceptions.ApplicationValidationException(
                $"Environment must be one of: {string.Join(", ", DeploymentEnvironment.All)}.");
    }

    public static void ValidateAiHintsJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return;

        try
        {
            JsonDocument.Parse(json);
        }
        catch (JsonException ex)
        {
            throw new Exceptions.ApplicationValidationException($"AI integration hints must be valid JSON. {ex.Message}");
        }
    }

    public static void ValidateDescription(string description)
    {
        if (description.Length > 500)
            throw new Exceptions.ApplicationValidationException("Description cannot exceed 500 characters.");
    }
}
