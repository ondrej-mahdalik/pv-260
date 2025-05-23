using ErrorOr;
using PV260.Client.BL;
using PV260.Common.Models;

namespace PV260.Client.ConsoleApp.Components.Content.Reports.ReportDetail;

internal class LatestGeneratedReportComponent(IApiClient apiClient)
    : ReportDetailComponentBase(apiClient)
{
    private ReportDetailModel? _latestReport;

    protected override async Task<ErrorOr<ReportDetailModel?>> GetReportAsync()
    {
        var report = await ApiClient.GetLatestReportAsync();
        _latestReport = report.IsError ? null : report.Value;
        return report;
    }

    protected override string GetHeader()
    {
        return "Latest Generated Report";
    }
}