using AiFeatureFlags.Api.Contracts;
using AiFeatureFlags.Application.Abstractions;
using AiFeatureFlags.Application.FeatureFlags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiFeatureFlags.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/public/feature-flags")]
public sealed class PublicFeatureFlagsController : ControllerBase
{
    private readonly FeatureFlagService _flags;

    public PublicFeatureFlagsController(FeatureFlagService flags)
    {
        _flags = flags;
    }

    [HttpGet("summaries")]
    [ProducesResponseType(typeof(IReadOnlyList<PublicFeatureFlagSummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiProblemResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Summaries(CancellationToken cancellationToken)
    {
        var result = await _flags.ListPublicSummariesAsync(cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }
}
