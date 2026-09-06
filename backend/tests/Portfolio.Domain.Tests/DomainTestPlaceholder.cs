using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;
using Portfolio.Domain.ValueObjects;
using Xunit;

namespace Portfolio.Domain.Tests;

public class DomainModelTests
{
    [Fact]
    public void ExchangeTradedFund_can_contain_weighted_stock_constituents()
    {
        var stock = new Stock("MSFT", "Microsoft Corporation", new CountryCode("US"), GicsSector.InformationTechnology, new GicsIndustryCode("45103010"));
        var exchangeTradedFund = new ExchangeTradedFund("SPY", "SPDR S&P 500 ETF Trust", new CountryCode("US"));
        var originalUpdatedAt = exchangeTradedFund.UpdatedAt;

        var constituent = exchangeTradedFund.AddConstituent(stock.Id, 0.0625m);

        Assert.Single(exchangeTradedFund.Constituents);
        Assert.Equal(stock.Id, constituent.SecurityId);
        Assert.Equal(0.0625m, constituent.Weight);
        Assert.Equal("US", exchangeTradedFund.DomicileCountry.Value);
        Assert.True(exchangeTradedFund.CreatedAt <= exchangeTradedFund.UpdatedAt);
        Assert.True(originalUpdatedAt <= exchangeTradedFund.UpdatedAt);
    }
}
