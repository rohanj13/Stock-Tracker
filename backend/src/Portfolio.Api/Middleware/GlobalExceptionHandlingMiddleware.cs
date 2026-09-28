using System.Net;
using System.Text.Json;
using Portfolio.Application.Exceptions;

namespace Portfolio.Api.Middleware;

/// <summary>
/// Global exception handling middleware that catches unhandled exceptions
/// and returns consistent JSON error responses with proper HTTP status codes.
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {ExceptionMessage}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = MapExceptionToResponse(exception);

        var response = new ProblemDetailsResponse
        {
            Type = GetExceptionType(exception),
            Title = GetExceptionTitle(exception),
            Status = statusCode,
            Detail = message,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return context.Response.WriteAsJsonAsync(response, options);
    }

    private static (int StatusCode, string Message) MapExceptionToResponse(Exception exception)
    {
        return exception switch
        {
            // Validation errors (400)
            ArgumentNullException or ArgumentException or InvalidOperationException
                => (StatusCode.Status400BadRequest, exception.Message),

            InvalidInstrumentLookupException
                => (StatusCode.Status400BadRequest, exception.Message),

            // Resource not found / unresolvable instruments (422 - Unprocessable Entity)
            InstrumentNotResolvedException
                => (StatusCode.Status422UnprocessableEntity, exception.Message),

            // Ambiguous identifier (422 - Unprocessable Entity)
            AmbiguousInstrumentException
                => (StatusCode.Status422UnprocessableEntity, exception.Message),

            // Duplicate FIGI (409 - Conflict)
            DuplicateFigiException
                => (StatusCode.Status409Conflict, exception.Message),

            // External service unavailable (503)
            InstrumentIdentifierServiceException serviceEx when serviceEx.HttpStatusCode == 401
                => (StatusCode.Status401Unauthorized, "OpenFIGI API authentication failed"),

            InstrumentIdentifierServiceException serviceEx when serviceEx.HttpStatusCode == 429
                => (StatusCode.Status429TooManyRequests, "OpenFIGI rate limit exceeded. Please retry after a delay."),

            InstrumentIdentifierServiceException serviceEx when serviceEx.HttpStatusCode >= 500
                => (StatusCode.Status503ServiceUnavailable, "OpenFIGI service is currently unavailable. Please try again later."),

            InstrumentIdentifierServiceException
                => (StatusCode.Status503ServiceUnavailable, exception.Message),

            // Unexpected errors (500)
            _ => (StatusCode.Status500InternalServerError, "An unexpected error occurred. Please contact support.")
        };
    }

    private static string GetExceptionType(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException or ArgumentException => "validation_error",
            InvalidInstrumentLookupException => "validation_error",
            InstrumentNotResolvedException => "unresolvable_instrument",
            AmbiguousInstrumentException => "ambiguous_instrument",
            DuplicateFigiException => "duplicate_identifier",
            InstrumentIdentifierServiceException => "external_service_error",
            _ => "internal_error"
        };
    }

    private static string GetExceptionTitle(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException or ArgumentException => "Validation Error",
            InvalidInstrumentLookupException => "Invalid Lookup Request",
            InstrumentNotResolvedException => "Instrument Not Found",
            AmbiguousInstrumentException => "Ambiguous Instrument",
            DuplicateFigiException => "Duplicate FIGI",
            InstrumentIdentifierServiceException => "External Service Error",
            _ => "Internal Server Error"
        };
    }

    /// <summary>
    /// RFC 7807 Problem Details response format
    /// </summary>
    private class ProblemDetailsResponse
    {
        public string? Type { get; set; }
        public string? Title { get; set; }
        public int Status { get; set; }
        public string? Detail { get; set; }
        public string? Instance { get; set; }
    }
}
