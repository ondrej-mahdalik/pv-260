using CsvHelper.Configuration.Attributes;

namespace PV260.API.Infrastructure.Models
{
    internal class AkrFundsModel
    {
        [Name("ticker")] public string? Ticker { get; set; }
        [Name("company")] public string? Company { get; set; }
        [Name("date")] public string? Date { get; set; }
        
        [Name("shares")] public string? SharesText { get; set; }
        [Name("weight (%)")] public string? WeightText { get; set; }
        [Name("fund")] public string? FundText { get; set; }
        [Name("cusip")] public string? CusipText { get; set; }
        [Name("market value ($)")] public string? MarketValueText { get; set; }
        
        [Ignore]
        public int NumberOfShares => int.TryParse(SharesText?.Replace(",", string.Empty).Trim(), out var s) ? s : -1;
        
        [Ignore]
        public double Weight => double.TryParse(WeightText?.Replace("%", string.Empty).Trim(), out var w) ? w : 0d;
    }
}