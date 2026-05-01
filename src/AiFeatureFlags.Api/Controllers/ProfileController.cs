using System.Security.Claims;
using AiFeatureFlags.Api.Contracts;
using AiFeatureFlags.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiFeatureFlags.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class ProfileController : ControllerBase
{
    [HttpGet("me")]
    [ProducesResponseType(typeof(ProfileMeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiProblemResponse), StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var displayName = User.FindFirstValue("display_name");

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(email))
            throw new IdentityResolutionException("Authenticated identity is incomplete.");

        return Ok(new ProfileMeResponse(userId, email, displayName ?? string.Empty));
    }
}

public sealed record ProfileMeResponse(string UserId, string Email, string DisplayName);
