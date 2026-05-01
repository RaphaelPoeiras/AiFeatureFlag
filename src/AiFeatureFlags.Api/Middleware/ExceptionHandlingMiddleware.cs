using System.Net;
using System.Text.Json;
using AiFeatureFlags.Api.Contracts;
using AiFeatureFlags.Application.Exceptions;

namespace AiFeatureFlags.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex).ConfigureAwait(false);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var mapped = Map(exception);

        _logger.LogWarning(exception, "Handled exception: {ErrorCode} — {Title}", mapped.ErrorCode, mapped.Title);

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = mapped.Status;

        var problem = new ApiProblemResponse(
            mapped.Title,
            mapped.Detail,
            mapped.Status,
            mapped.ErrorCode,
            context.TraceIdentifier);

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions)).ConfigureAwait(false);
    }

    private static (int Status, string Title, string Detail, string ErrorCode) Map(Exception exception)
    {
        return exception switch
        {
            ApplicationValidationException ex => (
                (int)HttpStatusCode.BadRequest,
                "Validation failed",
                ex.Message,
                ApiErrorCode.ValidationFailed),

            ConflictException ex => (
                (int)HttpStatusCode.Conflict,
                "Conflict",
                ex.Message,
                ApiErrorCode.Conflict),

            NotFoundException ex => (
                (int)HttpStatusCode.NotFound,
                "Not found",
                ex.Message,
                ApiErrorCode.NotFound),

            UnauthorizedCredentialsException ex => (
                (int)HttpStatusCode.Unauthorized,
                "Unauthorized",
                ex.Message,
                ApiErrorCode.InvalidCredentials),

            IdentityResolutionException ex => (
                (int)HttpStatusCode.Unauthorized,
                "Unauthorized",
                ex.Message,
                ApiErrorCode.MissingIdentity),

            ArgumentNullException ex => (
                (int)HttpStatusCode.BadRequest,
                "Invalid request",
                string.IsNullOrWhiteSpace(ex.ParamName)
                    ? "A required value was missing."
                    : $"Required parameter '{ex.ParamName}' was missing.",
                ApiErrorCode.InvalidRequestBody),

            _ => (
                (int)HttpStatusCode.InternalServerError,
                "Unexpected error",
                "An unexpected error occurred.",
                ApiErrorCode.InternalError)
        };
    }
}
