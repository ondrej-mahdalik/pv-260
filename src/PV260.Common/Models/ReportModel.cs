using System.ComponentModel.DataAnnotations;

namespace PV260.Common.Models;

public class ReportModel
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }


    // Navigation property for the records
    public virtual ICollection<ReportRecordModel> Records { get; set; }

    public ReportModel()
    {
        Records = new List<ReportRecordModel>();
        CreatedAt = DateTime.UtcNow;
    }
}