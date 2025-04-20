namespace PV260.API.BL.Options;

/// <summary>
/// Configuration options for report generation and management.
/// </summary>
public record ReportOptions
{
    /// <summary>
    /// The CRON expression that defines the schedule for report generation.
    /// CRON Format:
    /// <code>
    /// ┌───────────── minute (0 - 59)
    /// │ ┌───────────── hour (0 - 23)
    /// │ │ ┌───────────── day of month (1 - 31)
    /// │ │ │ ┌───────────── month (1 - 12)
    /// │ │ │ │ ┌───────────── day of week (0 - 6) (Sunday to Saturday)
    /// │ │ │ │ │
    /// │ │ │ │ │
    /// * * * * * command to execute
    /// </code>
    /// </summary>
    public required string ReportGenerationCron { get; init; }

    /// <summary>
    /// The number of days to keep generated reports before they are deleted.
    /// </summary>
    public required int ReportDaysToKeep { get; init; }

    /// <summary>
    /// The CRON expression that defines the schedule for cleaning up old reports.
    /// See <see cref="ReportGenerationCron"/> for the format.
    /// </summary>
    public required string OldReportCleanupCron { get; init; }

    /// <summary>
    /// Indicates whether an email to all recipients should be sent upon successful report generation.
    /// </summary>
    public required bool SendEmailOnReportGeneration { get; init; }
}