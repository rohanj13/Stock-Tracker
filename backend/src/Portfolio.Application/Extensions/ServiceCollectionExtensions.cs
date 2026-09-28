using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Services;

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
        // Register instrument identifier resolver port (implementation provided by Infrastructure)
        // services.AddScoped<IInstrumentIdentifierResolver, ...>(); // Registered in Infrastructure

        // Register stock and ETF services with their repository ports
        services.AddScoped<IStockService, StockService>();
        services.AddScoped<IEtfService, EtfService>();

        // Repository ports are registered in Infrastructure layer
        // services.AddScoped<IStockRepository, ...>();
        // services.AddScoped<IEtfRepository, ...>();

        return services;
    }
}
