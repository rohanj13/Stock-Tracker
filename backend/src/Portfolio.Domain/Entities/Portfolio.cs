namespace Portfolio.Domain.Entities;

public sealed class Portfolio : Entity
{
    private readonly List<PortfolioPosition> _positions = [];

    public Portfolio(Guid userId, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        UserId = userId;
        Name = name;
    }

    public Guid UserId { get; }

    public string Name { get; }

    public IReadOnlyCollection<PortfolioPosition> Positions => _positions.AsReadOnly();

    public PortfolioPosition AddPosition(Guid securityId, decimal quantity)
    {
        var position = new PortfolioPosition(Id, securityId, quantity);
        _positions.Add(position);
        return position;
    }
}