using System.Reflection;

namespace Portfolio.Api.Endpoints;

/// <summary>
/// Health and system endpoints for the Portfolio API.
/// Provides basic health checks and API metadata.
/// </summary>
public static class HealthEndpoints
{
    /// <summary>
    /// Registers health and system endpoints with the application.
    /// </summary>
    /// <param name="app">The web application.</param>
    public static void MapHealthEndpoints(this WebApplication app)
    {
        // Health check endpoint
        app.MapGet("/health", HealthCheck)
            .WithName("HealthCheck")
            .WithOpenApi()
            .Produces<HealthResponse>(StatusCodes.Status200OK)
            .WithSummary("Check API health status");

        // API info endpoint
        app.MapGet("/", GetApiInfo)
            .WithName("GetApiInfo")
            .WithOpenApi()
            .Produces<ApiInfoResponse>(StatusCodes.Status200OK)
            .WithSummary("Get API information");
    }

    private static HealthResponse HealthCheck()
    {
        return new HealthResponse { Status = "healthy" };
    }

    private static ApiInfoResponse GetApiInfo()
    {
        var version = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ?? "1.0.0";

        return new ApiInfoResponse
        {
            Name = "Portfolio Diversification API",
            Version = version,
            Environment = GetEnvironment()
        };
    }

    private static string GetEnvironment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
    }
}

/// <summary>
/// Response model for health check endpoint.
/// </summary>
public class HealthResponse
{
    /// <summary>
    /// Status of the API.
    /// </summary>
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Response model for API info endpoint.
/// </summary>
public class ApiInfoResponse
{
    /// <summary>
    /// Name of the API.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Version of the API.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Current environment.
    /// </summary>
    public string Environment { get; set; } = string.Empty;
}
