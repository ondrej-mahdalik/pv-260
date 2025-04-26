namespace PV260.Client.ConsoleApp.Components.Content.Reports.ReportOptions;

internal enum ReportOptions
{
    GenerateNewReport,
    ListReports
}

internal static class ReportOptionsExtensions
{
    public static string ToFriendlyString(this ReportOptions option)
    {
        return option switch
        {
            ReportOptions.GenerateNewReport => "Generate a new report",
            ReportOptions.ListReports => "List all reports",
            _ => option.ToString()
        };
    }

    public static string GetDescription(this ReportOptions option)
    {
        return option switch
        {
            ReportOptions.GenerateNewReport => "Create a new report based on the latest stock market data",
            ReportOptions.ListReports => "View and manage previously generated stock reports",
            _ => option.ToString()
        };
    }
}