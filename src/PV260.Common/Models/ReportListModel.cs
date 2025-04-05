namespace PV260.Common.Models;

public record ReportListModel : DataModelBase
{
    public required string Name { get; init; }
    public required DateTime CreatedAt { get; init; }
}