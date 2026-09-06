using Microsoft.Extensions.DependencyInjection;

namespace Portfolio.Application.Extensions;

/// <summary>
/// Extension methods for registering application services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds application layer services.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Application services will be registered here as the domain model evolves.
        // This includes business logic services, validators, DTOs, etc.

        return services;
    }
}
