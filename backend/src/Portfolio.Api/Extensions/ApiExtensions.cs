using Portfolio.Api.Middleware;

namespace Portfolio.Api.Extensions;

/// <summary>
/// Extension methods for configuring API middleware and services.
/// </summary>
public static class ApiExtensions
{
    /// <summary>
    /// Adds API-specific middleware to the application pipeline.
    /// </summary>
    /// <param name="app">The web application builder.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication UseApiMiddleware(this WebApplication app)
    {
        // Add global exception handling middleware
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

        // Add HTTP redirection middleware
        app.UseHttpsRedirection();

        // Add CORS middleware
        app.UseCors("AllowConfiguredOrigins");

        // Add logging middleware
        app.UseRequestLogging();

        return app;
    }

    /// <summary>
    /// Adds request logging middleware to log HTTP requests and responses.
    /// </summary>
    /// <param name="app">The web application builder.</param>
    /// <returns>The web application for chaining.</returns>
    private static WebApplication UseRequestLogging(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation(
                "Incoming request: {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await next();

            logger.LogInformation(
                "Response: {StatusCode} for {Method} {Path}",
                context.Response.StatusCode,
                context.Request.Method,
                context.Request.Path);
        });

        return app;
    }

    /// <summary>
    /// Configures CORS policy from configuration settings.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowConfiguredOrigins", policy =>
            {
                if (allowedOrigins.Any())
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
                else
                {
                    // Default to localhost for development if no origins configured
                    policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Adds health check services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApiHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }
}
