namespace Portfolio.Domain.Entities;

public sealed class PortfolioPosition : Entity
{
    public PortfolioPosition(Guid portfolioId, Guid securityId, decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Position quantity must be greater than zero.");
        }

        PortfolioId = portfolioId;
        SecurityId = securityId;
        Quantity = quantity;
    }

    public Guid PortfolioId { get; }

    public Guid SecurityId { get; }

    public decimal Quantity { get; }
}