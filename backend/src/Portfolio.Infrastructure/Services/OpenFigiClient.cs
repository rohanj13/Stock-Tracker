using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Portfolio.Application.Exceptions;
using Portfolio.Application.Interfaces;

namespace Portfolio.Infrastructure.Services;

/// <summary>
/// HTTP client implementation of IInstrumentIdentifierResolver that calls the OpenFIGI API.
/// </summary>
public class OpenFigiClient : IInstrumentIdentifierResolver
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenFigiClient> _logger;

    public OpenFigiClient(HttpClient httpClient, ILogger<OpenFigiClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<InstrumentLookupResult> ResolveAsync(InstrumentLookupRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Validate();

        try
        {
            var apiRequest = MapRequestToApiFormat(request);
            var response = await PostToOpenFigiAsync(apiRequest, cancellationToken);
            return MapResponseToResult(response, request);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling OpenFIGI API");
            throw new InstrumentIdentifierServiceException(
                "Failed to communicate with OpenFIGI service",
                serviceError: ex.Message,
                innerException: ex);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "OpenFIGI request timed out");
            throw new InstrumentIdentifierServiceException(
                "OpenFIGI service request timed out",
                serviceError: ex.Message,
                innerException: ex);
        }
    }

    private static OpenFigiApiRequest MapRequestToApiFormat(InstrumentLookupRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Figi))
        {
            return new OpenFigiApiRequest
            {
                new OpenFigiApiLookup
                {
                    IdType = "ID_BB_GLOBAL",
                    IdValue = request.Figi.ToUpperInvariant(),
                    MicCode = null
                }
            };
        }
        else if (!string.IsNullOrWhiteSpace(request.Ticker) && !string.IsNullOrWhiteSpace(request.MicCode))
        {
            return new OpenFigiApiRequest
            {
                new OpenFigiApiLookup
                {
                    IdType = "TICKER",
                    IdValue = request.Ticker.ToUpperInvariant(),
                    MicCode = request.MicCode.ToUpperInvariant()
                }
            };
        }
        else
        {
            throw new InvalidInstrumentLookupException(
                "Either Figi or (Ticker + MicCode) must be provided");
        }
    }

    private async Task<OpenFigiApiResponse> PostToOpenFigiAsync(OpenFigiApiRequest request, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PostAsJsonAsync("/v3/mapping", request, cancellationToken);

        // Handle HTTP errors
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new InstrumentIdentifierServiceException(
                "OpenFIGI API key is invalid or expired",
                serviceError: "Unauthorized",
                httpStatusCode: 401);
        }

        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            throw new InstrumentIdentifierServiceException(
                "OpenFIGI rate limit exceeded",
                serviceError: "Too Many Requests",
                httpStatusCode: 429);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("OpenFIGI API error: {StatusCode} {Content}", response.StatusCode, errorContent);

            throw new InstrumentIdentifierServiceException(
                $"OpenFIGI service returned error: {response.StatusCode}",
                serviceError: errorContent,
                httpStatusCode: (int)response.StatusCode);
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<OpenFigiApiResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return result ?? throw new InstrumentIdentifierServiceException("Empty response from OpenFIGI API");
    }

    private static InstrumentLookupResult MapResponseToResult(OpenFigiApiResponse response, InstrumentLookupRequest request)
    {
        if (response.Data == null || response.Data.Count == 0)
        {
            var identifier = !string.IsNullOrWhiteSpace(request.Figi)
                ? request.Figi
                : $"{request.Ticker}/{request.MicCode}";
            throw new InstrumentNotResolvedException(identifier);
        }

        if (response.Data.Count > 1)
        {
            var identifier = !string.IsNullOrWhiteSpace(request.Figi)
                ? request.Figi
                : $"{request.Ticker}/{request.MicCode}";
            throw new AmbiguousInstrumentException(identifier, response.Data.Count);
        }

        var data = response.Data[0];

        // Validate venue FIGI presence
        if (string.IsNullOrWhiteSpace(data.Figi))
        {
            throw new InstrumentIdentifierServiceException(
                "OpenFIGI response missing venue FIGI",
                serviceError: "Invalid OpenFIGI response format");
        }

        var figiIdentifiers = new Portfolio.Domain.ValueObjects.FigiIdentifiers(
            figi: data.Figi,
            compositeFigi: data.CompositeFigi,
            shareClassFigi: data.ShareClassFigi);

        return new InstrumentLookupResult
        {
            Ticker = data.Ticker ?? request.Ticker ?? string.Empty,
            Name = data.Name ?? string.Empty,
            FigiIdentifiers = figiIdentifiers
        };
    }

    // OpenFIGI API DTOs
    private sealed class OpenFigiApiRequest : List<OpenFigiApiLookup> { }

    private sealed record OpenFigiApiLookup
    {
        public string? IdType { get; init; }
        public string? IdValue { get; init; }
        public string? MicCode { get; init; }
    }

    private sealed record OpenFigiApiResponse
    {
        public List<OpenFigiApiData>? Data { get; init; }
        public string? Warning { get; init; }
    }

    private sealed record OpenFigiApiData
    {
        public string? Figi { get; init; }
        public string? SecurityType { get; init; }
        public string? MarketSector { get; init; }
        public string? Ticker { get; init; }
        public string? Name { get; init; }
        public string? ExchCode { get; init; }
        public string? CompositeFigi { get; init; }
        public string? ShareClassFigi { get; init; }
    }
}
