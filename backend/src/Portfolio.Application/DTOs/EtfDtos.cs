namespace Portfolio.Application.DTOs;

/// <summary>
/// Represents an exchange traded fund for API responses.
/// </summary>
public sealed class EtfDto
{
    public Guid Id { get; set; }

    public string Symbol { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string DomicileCountry { get; set; } = string.Empty;

    public IReadOnlyCollection<EtfConstituentDto> Constituents { get; set; } = [];

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>
/// Represents a single constituent holding within an ETF.
/// </summary>
public sealed class EtfConstituentDto
{
    public Guid Id { get; set; }

    public Guid SecurityId { get; set; }

    public decimal Weight { get; set; }
}

/// <summary>
/// Request payload for creating a new ETF.
/// </summary>
public sealed class CreateEtfRequest
{
    public string Symbol { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string DomicileCountry { get; set; } = string.Empty;
}

/// <summary>
/// Request payload for updating an existing ETF.
/// </summary>
public sealed class UpdateEtfRequest
{
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Request payload for adding a constituent to an ETF.
/// </summary>
public sealed class AddEtfConstituentRequest
{
    public Guid SecurityId { get; set; }

    public decimal Weight { get; set; }
}
