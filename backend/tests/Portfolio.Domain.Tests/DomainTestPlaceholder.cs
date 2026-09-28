using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;
using Portfolio.Domain.ValueObjects;
using Xunit;

namespace Portfolio.Domain.Tests;

public class FigiIdentifiersTests
{
    [Fact]
    public void FigiIdentifiers_requires_valid_venue_figi()
    {
        var ex = Assert.Throws<ArgumentException>(() => new FigiIdentifiers(""));
        Assert.Contains("null or whitespace", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FigiIdentifiers_validates_figi_length()
    {
        var ex = Assert.Throws<ArgumentException>(() => new FigiIdentifiers("BBG12345678")); // 11 chars, not 12
        Assert.Contains("12 characters", ex.Message);
    }

    [Fact]
    public void FigiIdentifiers_validates_figi_starts_with_bbg()
    {
        var ex = Assert.Throws<ArgumentException>(() => new FigiIdentifiers("XYZ000000000")); // Doesn't start with BBG
        Assert.Contains("start with 'BBG'", ex.Message);
    }

    [Fact]
    public void FigiIdentifiers_validates_figi_suffix_is_alphanumeric()
    {
        var ex = Assert.Throws<ArgumentException>(() => new FigiIdentifiers("BBG00000000!")); // Invalid char
        Assert.Contains("alphanumeric", ex.Message);
    }

    [Fact]
    public void FigiIdentifiers_normalizes_to_uppercase()
    {
        var figi = new FigiIdentifiers("bbg000blnnh6", "bbg000nrmj71", "bbg001s5s399");

        Assert.Equal("BBG000BLNNH6", figi.Figi);
        Assert.Equal("BBG000NRMJ71", figi.CompositeFigi);
        Assert.Equal("BBG001S5S399", figi.ShareClassFigi);
    }

    [Fact]
    public void FigiIdentifiers_allows_null_optional_identifiers()
    {
        var figi = new FigiIdentifiers("BBG000BLNNH6");

        Assert.Equal("BBG000BLNNH6", figi.Figi);
        Assert.Null(figi.CompositeFigi);
        Assert.Null(figi.ShareClassFigi);
    }

    [Fact]
    public void FigiIdentifiers_validates_optional_composite_figi()
    {
        var ex = Assert.Throws<ArgumentException>(() => new FigiIdentifiers("BBG000BLNNH6", "invalid"));
        Assert.Contains("12 characters", ex.Message);
    }

    [Fact]
    public void FigiIdentifiers_validates_optional_share_class_figi()
    {
        var ex = Assert.Throws<ArgumentException>(() => new FigiIdentifiers("BBG000BLNNH6", null, "invalid"));
        Assert.Contains("12 characters", ex.Message);
    }

    [Fact]
    public void FigiIdentifiers_returns_venue_figi_as_string()
    {
        var figi = new FigiIdentifiers("BBG000BLNNH6");
        Assert.Equal("BBG000BLNNH6", figi.ToString());
    }
}

public class DomainModelTests
{
    [Fact]
    public void Stock_requires_figi_identifiers()
    {
        var country = new CountryCode("US");
        var sector = GicsSector.InformationTechnology;
        var industry = new GicsIndustryCode("45103010");

        var ex = Assert.Throws<ArgumentNullException>(() =>
            new Stock("MSFT", "Microsoft Corporation", country, sector, industry, null!, null));

        Assert.Equal("figiIdentifiers", ex.ParamName);
    }

    [Fact]
    public void Stock_accepts_figi_identifiers_and_optional_mic()
    {
        var country = new CountryCode("US");
        var sector = GicsSector.InformationTechnology;
        var industry = new GicsIndustryCode("45103010");
        var figi = new FigiIdentifiers("BBG000BLNNH6", "BBG000BLNNH6", "BBG001S5S399");

        var stock = new Stock("MSFT", "Microsoft Corporation", country, sector, industry, figi, "XNYS");

        Assert.Equal("MSFT", stock.Symbol);
        Assert.Equal("Microsoft Corporation", stock.Name);
        Assert.Equal("US", stock.Country.Value);
        Assert.Equal(figi.Figi, stock.FigiIdentifiers!.Figi);
        Assert.Equal("XNYS", stock.SourceMicCode);
    }

    [Fact]
    public void ExchangeTradedFund_requires_figi_identifiers()
    {
        var country = new CountryCode("US");

        var ex = Assert.Throws<ArgumentNullException>(() =>
            new ExchangeTradedFund("SPY", "SPDR S&P 500 ETF Trust", country, null!, null));

        Assert.Equal("figiIdentifiers", ex.ParamName);
    }

    [Fact]
    public void ExchangeTradedFund_accepts_figi_identifiers_and_optional_mic()
    {
        var country = new CountryCode("US");
        var figi = new FigiIdentifiers("BBG000NRMJ71", "BBG000NRMJ71", null);

        var etf = new ExchangeTradedFund("SPY", "SPDR S&P 500 ETF Trust", country, figi, "ARCX");

        Assert.Equal("SPY", etf.Symbol);
        Assert.Equal("SPDR S&P 500 ETF Trust", etf.Name);
        Assert.Equal("US", etf.DomicileCountry.Value);
        Assert.Equal(figi.Figi, etf.FigiIdentifiers!.Figi);
        Assert.Equal("ARCX", etf.SourceMicCode);
    }

    [Fact]
    public void ExchangeTradedFund_can_contain_weighted_stock_constituents()
    {
        var stock = new Stock(
            "MSFT",
            "Microsoft Corporation",
            new CountryCode("US"),
            GicsSector.InformationTechnology,
            new GicsIndustryCode("45103010"),
            new FigiIdentifiers("BBG000BLNNH6", "BBG000BLNNH6", "BBG001S5S399"),
            "XNYS");

        var etf = new ExchangeTradedFund(
            "SPY",
            "SPDR S&P 500 ETF Trust",
            new CountryCode("US"),
            new FigiIdentifiers("BBG000NRMJ71", "BBG000NRMJ71", null),
            "ARCX");

        var originalUpdatedAt = etf.UpdatedAt;

        var constituent = etf.AddConstituent(stock.Id, 0.0625m);

        Assert.Single(etf.Constituents);
        Assert.Equal(stock.Id, constituent.SecurityId);
        Assert.Equal(0.0625m, constituent.Weight);
        Assert.Equal("US", etf.DomicileCountry.Value);
        Assert.True(etf.CreatedAt <= etf.UpdatedAt);
        Assert.True(originalUpdatedAt <= etf.UpdatedAt);
    }
}

