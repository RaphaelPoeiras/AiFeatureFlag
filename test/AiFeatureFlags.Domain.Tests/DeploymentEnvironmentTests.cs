using AiFeatureFlags.Domain.Common;

namespace AiFeatureFlags.Domain.Tests;

public sealed class DeploymentEnvironmentTests
{
    [Theory]
    [InlineData("Development", true)]
    [InlineData("Staging", true)]
    [InlineData("Production", true)]
    [InlineData("development", false)]
    [InlineData("", false)]
    [InlineData("QA", false)]
    public void IsKnown_reflects_catalog(string value, bool expected)
    {
        Assert.Equal(expected, DeploymentEnvironment.IsKnown(value));
    }
}
