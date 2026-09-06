namespace Portfolio.Domain.Entities;

/// <summary>
/// Placeholder for domain entities.
/// Domain entities represent core business concepts and should remain independent of:
/// - ASP.NET Core
/// - Entity Framework Core
/// - AWS SDKs
/// - HTTP concerns
/// 
/// Example entities that will be added:
/// - Portfolio
/// - Holding
/// - Position
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Base class for domain entities with unique identifier.
    /// </summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; protected set; } = DateTimeOffset.UtcNow;

    protected void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
}
