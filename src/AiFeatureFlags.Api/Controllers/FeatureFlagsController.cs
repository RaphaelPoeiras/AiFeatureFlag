using System.Security.Claims;
using AiFeatureFlags.Api.Contracts;
using AiFeatureFlags.Application.Exceptions;
using AiFeatureFlags.Application.FeatureFlags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiFeatureFlags.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/feature-flags")]
public sealed class FeatureFlagsController : ControllerBase
{
    private readonly FeatureFlagService _flags;

    public FeatureFlagsController(FeatureFlagService flags)
    {
        _flags = flags;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<FeatureFlagResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiProblemResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListMine(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _flags.ListMineAsync(userId, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(FeatureFlagResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiProblemResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiProblemResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiProblemResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateFeatureFlagRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var created = await _flags.CreateAsync(userId, request, cancellationToken).ConfigureAwait(false);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(FeatureFlagResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiProblemResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiProblemResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiProblemResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFeatureFlagRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var updated = await _flags.UpdateAsync(userId, id, request, cancellationToken).ConfigureAwait(false);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiProblemResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiProblemResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _flags.DeleteAsync(userId, id, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out var id))
            throw new IdentityResolutionException("Authenticated user identity is missing or invalid.");

        return id;
    }
}
