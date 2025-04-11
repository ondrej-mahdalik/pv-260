namespace PV260.API.DAL.Entities;

public record ReportRecordEntity : EntityBase
{
    public required string CompanyName { get; init; }
    public required string Ticker { get; init; }
    public required int NumberOfShares { get; init; }
    public required double SharesChangePercentage { get; init; }
    public required double Weight { get; init; }
    
    public Guid? ReportId { get; init; }
    public ReportEntity? Report { get; init; } // Navigation property
}