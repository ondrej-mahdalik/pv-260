using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PV260.Common.Models;

public class ReportRecordModel
{
    public int Id { get; set; }

    [Required]
    public string CompanyName { get; set; }

    [Required]
    public string Ticker { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int NumberOfShares { get; set; }

    [Required]
    public decimal SharesChangePercentage { get; set; }

    [Required]
    [Range(0, 100)]
    public decimal Weight { get; set; }

    // Foreign key for Report
    public int ReportId { get; set; }

    // Navigation property
    [JsonIgnore]
    public virtual ReportModel? Report { get; set; }
}