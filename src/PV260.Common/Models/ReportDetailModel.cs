namespace PV260.Common.Models;

public record ReportDetailModel : DataModelBase
{
    public required string Name { get; init; }
    public required DateTime CreatedAt { get; init; }
    public IList<ReportRecordModel> Records { get; init; } = [];
}