namespace Portfolio.Domain.Entities;

public sealed class User : Entity
{
    private readonly List<Portfolio> _portfolios = [];

    public User(string externalAuthUserId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(externalAuthUserId);
        ExternalAuthUserId = externalAuthUserId;
    }

    public string ExternalAuthUserId { get; }

    public IReadOnlyCollection<Portfolio> Portfolios => _portfolios.AsReadOnly();

    public Portfolio CreatePortfolio(string name)
    {
        var portfolio = new Portfolio(Id, name);
        _portfolios.Add(portfolio);
        return portfolio;
    }
}