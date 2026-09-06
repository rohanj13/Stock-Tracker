namespace Portfolio.Application.DTOs;

/// <summary>
/// Represents a stock security for API responses.
/// </summary>
public sealed class StockDto
{
    public Guid Id { get; set; }

    public string Symbol { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string Sector { get; set; } = string.Empty;

    public string Industry { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>
/// Request payload for creating a new stock.
/// </summary>
public sealed class CreateStockRequest
{
    public string Symbol { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string Sector { get; set; } = string.Empty;

    public string Industry { get; set; } = string.Empty;
}

/// <summary>
/// Request payload for updating an existing stock.
/// </summary>
public sealed class UpdateStockRequest
{
    public string Name { get; set; } = string.Empty;

    public string Sector { get; set; } = string.Empty;

    public string Industry { get; set; } = string.Empty;
}
