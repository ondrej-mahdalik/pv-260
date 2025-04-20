namespace PV260.API.DAL.Entities;

public record ReportRecordEntity : EntityBase
{
    public required string CompanyName { get; set; }
    public required string Ticker { get; set; }
    public required int NumberOfShares { get; set; }
    public required double SharesChangePercentage { get; set; }
    public required double Weight { get; set; }
    
    public Guid? ReportId { get; init; }
    public ReportEntity? Report { get; init; } // Navigation property
}