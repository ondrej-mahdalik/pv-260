namespace PV260.Common.Models;

public record SettingsModel
{
    public required string ReportGenerationCron { get; set; }
    public int ReportDaysToKeep { get; set; }
    public required string OldReportCleanupCron { get; set; }
    public bool SendEmailOnReportGeneration { get; set; }
}