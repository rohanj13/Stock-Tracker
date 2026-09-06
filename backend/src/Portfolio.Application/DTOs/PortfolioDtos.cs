namespace Portfolio.Application.DTOs;

/// <summary>
/// Represents a portfolio for API responses.
/// </summary>
public sealed class PortfolioDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public IReadOnlyCollection<PortfolioPositionDto> Positions { get; set; } = [];

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>
/// Represents a single position within a portfolio.
/// </summary>
public sealed class PortfolioPositionDto
{
    public Guid Id { get; set; }

    public Guid SecurityId { get; set; }

    public decimal Quantity { get; set; }
}

/// <summary>
/// Request payload for creating a new portfolio.
/// </summary>
public sealed class CreatePortfolioRequest
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Request payload for updating an existing portfolio.
/// </summary>
public sealed class UpdatePortfolioRequest
{
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Request payload for adding a position to a portfolio.
/// </summary>
public sealed class AddPortfolioPositionRequest
{
    public Guid SecurityId { get; set; }

    public decimal Quantity { get; set; }
}
