namespace AiFeatureFlags.Api.Contracts;

/// <summary>
/// RFC 7807-style problem payload (subset) returned as <c>application/problem+json</c> for domain failures.
/// </summary>
public sealed record ApiProblemResponse(
    string Title,
    string Detail,
    int Status,
    string ErrorCode,
    string? TraceId);
