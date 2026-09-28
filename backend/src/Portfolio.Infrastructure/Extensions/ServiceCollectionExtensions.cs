using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Portfolio.Application.Interfaces;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Infrastructure.Repositories;
using Portfolio.Infrastructure.Services;

namespace Portfolio.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering infrastructure services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds infrastructure services including Entity Framework Core DbContext and OpenFIGI HTTP client.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">Application configuration for database connection strings and OpenFIGI settings.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register ApplicationDbContext with PostgreSQL provider
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name)));

        // Register repository implementations
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<IEtfRepository, EtfRepository>();

        // Register OpenFIGI HTTP client with resilience policies
        AddOpenFigiClient(services, configuration);

        return services;
    }

    private static void AddOpenFigiClient(IServiceCollection services, IConfiguration configuration)
    {
        var openFigiBaseUrl = configuration["OpenFigi:BaseUrl"] ?? "https://api.openfigi.com";
        var openFigiTimeout = TimeSpan.Parse(configuration["OpenFigi:Timeout"] ?? "30s");
        var openFigiApiKey = configuration["OpenFigi:ApiKey"]; // Optional; can come from environment variable

        // Build Polly resilience policy for rate limits and transient failures
        var retryPolicy = Policy
            .Handle<HttpRequestException>(ex => ex.InnerException is TimeoutException || ex.Message.Contains("503"))
            .Or<HttpRequestException>(ex => ex.Message.Contains("429"))
            .OrResult<HttpResponseMessage>(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests || r.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff: 2, 4, 8 seconds
                onRetry: (outcome, timeSpan, retryCount, context) =>
                {
                    System.Diagnostics.Debug.WriteLine($"OpenFIGI retry #{retryCount} after {timeSpan.TotalSeconds}s");
                });

        var circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode && r.StatusCode != System.Net.HttpStatusCode.BadRequest)
            .CircuitBreakerAsync<HttpResponseMessage>(
                5, // handledEventsAllowedBeforeBreaking
                TimeSpan.FromSeconds(30)); // durationOfBreak

        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(openFigiTimeout);

        var combinedPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);

        // Configure typed HTTP client
        services.AddHttpClient<OpenFigiClient>(client =>
        {
            client.BaseAddress = new Uri(openFigiBaseUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "StockTracker/1.0");
            if (!string.IsNullOrWhiteSpace(openFigiApiKey))
            {
                client.DefaultRequestHeaders.Add("X-OPENFIGI-APIKEY", openFigiApiKey);
            }
        })
        .AddPolicyHandler(combinedPolicy);

        // Register the resolver port with the HTTP client implementation
        services.AddScoped<IInstrumentIdentifierResolver>(provider =>
            provider.GetRequiredService<OpenFigiClient>());
    }
}
