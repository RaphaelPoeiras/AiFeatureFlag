using AiFeatureFlags.Application.Exceptions;
using AiFeatureFlags.Application.FeatureFlags;

namespace AiFeatureFlags.Application.Tests;

public sealed class FeatureFlagBusinessRulesTests
{
    [Fact]
    public void ValidateKey_rejects_invalid_patterns()
    {
        Assert.Throws<ApplicationValidationException>(() => FeatureFlagBusinessRules.ValidateKey(""));
        Assert.Throws<ApplicationValidationException>(() => FeatureFlagBusinessRules.ValidateKey("INVALIDCASE"));
        Assert.Throws<ApplicationValidationException>(() => FeatureFlagBusinessRules.ValidateKey("ab"));
    }

    [Fact]
    public void ValidateKey_accepts_expected_pattern()
    {
        FeatureFlagBusinessRules.ValidateKey("ai.prompt.experiment.v2");
    }

    [Fact]
    public void ValidateAiHintsJson_rejects_invalid_json()
    {
        Assert.Throws<ApplicationValidationException>(() =>
            FeatureFlagBusinessRules.ValidateAiHintsJson("{"));
    }

    [Fact]
    public void ValidateAiHintsJson_accepts_empty_or_valid_json()
    {
        FeatureFlagBusinessRules.ValidateAiHintsJson("");
        FeatureFlagBusinessRules.ValidateAiHintsJson("{\"model\":\"gpt-family\"}");
    }
}
