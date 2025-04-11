namespace PV260.Common.Models;

public record ReportRecordModel : DataModelBase
{
    public required string CompanyName { get; init; }
    public required string Ticker { get; init; }
    public required int NumberOfShares { get; init; }
    public required double SharesChangePercentage { get; init; }
    public required double Weight { get; init; }
}