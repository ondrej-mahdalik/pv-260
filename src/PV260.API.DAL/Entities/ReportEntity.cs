namespace PV260.API.DAL.Entities;

public record ReportEntity : EntityBase
{
    public required string Name { get; set; }
    public required DateTime CreatedAt { get; set; }
    
    public ICollection<ReportRecordEntity> Records { get; init; } = new List<ReportRecordEntity>(); // Navigation property
}