namespace Portfolio.Domain.Entities;

public sealed class EtfConstituent : Entity
{
    public EtfConstituent(Guid exchangeTradedFundId, Guid securityId, decimal weight)
    {
        if (weight <= 0 || weight > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(weight), "Constituent weight must be greater than zero and no more than one.");
        }

        ExchangeTradedFundId = exchangeTradedFundId;
        SecurityId = securityId;
        Weight = weight;
    }

    public Guid ExchangeTradedFundId { get; }

    public Guid SecurityId { get; }

    public decimal Weight { get; }
}