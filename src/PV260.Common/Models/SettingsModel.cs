namespace PV260.Common.Models;

public record SettingsModel(string ReportGenerationCron, int ReportDaysToKeep, bool SendEmailOnReportGeneration);