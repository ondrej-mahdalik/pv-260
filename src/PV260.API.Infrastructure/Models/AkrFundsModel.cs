using CsvHelper.Configuration;

namespace PV260.API.Infrastructure.Models
{
    internal class AkrFundsModel
    {
        public string Date { get; set; } = string.Empty;
        public string Fund { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Ticker { get; set; } = string.Empty;
        public string Cusip { get; set; } = string.Empty;
        public string Shares { get; set; } = string.Empty;
        public string MarketValue { get; set; } = string.Empty;
        public string Weight { get; set; } = string.Empty;
    }

    internal sealed class AkrFundsModelMap : ClassMap<AkrFundsModel>
    {
        public AkrFundsModelMap()
        {
            Map(m => m.Date).Name("date");
            Map(m => m.Fund).Name("fund");
            Map(m => m.Company).Name("company");
            Map(m => m.Ticker).Name("ticker");
            Map(m => m.Cusip).Name("cusip");
            Map(m => m.Shares).Name("shares");
            Map(m => m.MarketValue).Name("market value ($)");
            Map(m => m.Weight).Name("weight (%)");
        }
    }
}