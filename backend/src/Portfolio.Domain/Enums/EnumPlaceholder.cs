namespace Portfolio.Domain.Enums;

/// <summary>
/// Placeholder for domain enumerations.
/// Examples of enums that will be added:
/// - HoldingType (Equity, ETF, Bond, etc.)
/// - ExposureCategory (Company, Sector, Country, Region)
/// - PortfolioStatus (Active, Archived, etc.)
/// </summary>
public enum SecurityType
{
    Stock,
    ExchangeTradedFund,
    Bond,
    Cash
}

public enum GicsSector
{
    Energy,
    Materials,
    Industrials,
    ConsumerDiscretionary,
    ConsumerStaples,
    HealthCare,
    Financials,
    InformationTechnology,
    CommunicationServices,
    Utilities,
    RealEstate
}
