namespace PV260.API.DAL.Entities;

public record ReportEntity : EntityBase
{
    public required string Name { get; init; }
    public required DateTime CreatedAt { get; init; }
    
    public ICollection<ReportRecordEntity> Records { get; init; } = new List<ReportRecordEntity>(); // Navigation property
}