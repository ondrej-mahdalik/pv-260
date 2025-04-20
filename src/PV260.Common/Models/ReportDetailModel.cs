using System.ComponentModel.DataAnnotations;

namespace PV260.Common.Models;

public record ReportDetailModel : DataModelBase
{
    [Required]
    public required string Name { get; set; }
    [Required]
    public required DateTime CreatedAt { get; set; }
    public IList<ReportRecordModel> Records { get; init; } = [];
}