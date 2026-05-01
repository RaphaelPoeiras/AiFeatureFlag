namespace AiFeatureFlags.Api.Contracts;

/// <summary>
/// Stable machine-readable codes returned in <see cref="ApiProblemResponse"/> for clients and automation.
/// </summary>
public static class ApiErrorCode
{
    public const string ValidationFailed = "VALIDATION_FAILED";
    public const string InvalidRequestBody = "INVALID_REQUEST_BODY";
    public const string Conflict = "CONFLICT";
    public const string NotFound = "NOT_FOUND";
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
    public const string MissingIdentity = "MISSING_IDENTITY";
    public const string InternalError = "INTERNAL_ERROR";
}
