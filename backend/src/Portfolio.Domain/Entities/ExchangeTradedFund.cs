using Portfolio.Domain.Enums;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities;

public sealed class ExchangeTradedFund : Security
{
    private readonly List<EtfConstituent> _constituents = [];

    public ExchangeTradedFund(
        string symbol,
        string name,
        CountryCode domicileCountry,
        FigiIdentifiers figiIdentifiers,
        string? sourceMicCode = null)
        : base(symbol, name, domicileCountry, SecurityType.ExchangeTradedFund, figiIdentifiers, sourceMicCode)
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

    public void RemoveConstituent(Guid constituentId)
    {
        var constituent = _constituents.FirstOrDefault(c => c.Id == constituentId);
        if (constituent != null)
        {
            _constituents.Remove(constituent);
            Touch();
        }
    }
}