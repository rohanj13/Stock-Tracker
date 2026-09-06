using Portfolio.Domain.Enums;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities;

public sealed class ExchangeTradedFund : Security
{
    private readonly List<EtfConstituent> _constituents = [];

    public ExchangeTradedFund(string symbol, string name, CountryCode domicileCountry)
        : base(symbol, name, domicileCountry, SecurityType.ExchangeTradedFund)
    {
    }

    public CountryCode DomicileCountry => Country;

    public IReadOnlyCollection<EtfConstituent> Constituents => _constituents.AsReadOnly();

    public EtfConstituent AddConstituent(Guid securityId, decimal weight)
    {
        var constituent = new EtfConstituent(Id, securityId, weight);
        _constituents.Add(constituent);
        Touch();
        return constituent;
    }
}