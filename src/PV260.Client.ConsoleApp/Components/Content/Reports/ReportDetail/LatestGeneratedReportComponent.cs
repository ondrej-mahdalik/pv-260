using PV260.Client.BL;
using PV260.Common.Models;

namespace PV260.Client.ConsoleApp.Components.Content.Reports.ReportDetail;

internal class LatestGeneratedReportComponent(IApiClient apiClient)
    : ReportDetailComponentBase(apiClient)
{
    private ReportDetailModel? _latestReport;

    protected override async Task<ReportDetailModel?> GetReportAsync()
    {
        _latestReport = await ApiClient.GetLatestReportAsync();
        await Task.Delay(500); // Simulate some delay for better UX

        return _latestReport;
    }

    protected override string GetHeader()
    {
        return "Latest Generated Report";
    }
}