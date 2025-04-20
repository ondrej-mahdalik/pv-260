using System.ComponentModel.DataAnnotations;

namespace PV260.Common.Models;

public record ReportRecordModel : DataModelBase
{
    [Required]
    public required string CompanyName { get; set; }
    
    [Required]
    public required string Ticker { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public required int NumberOfShares { get; set; }
    
    [Required]
    public required double SharesChangePercentage { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public required double Weight { get; set; }
}