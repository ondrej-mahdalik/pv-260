using PV260.Client.BL;
using PV260.Common.Models;

namespace PV260.Client.ConsoleApp.Components.Content.Reports.ReportDetail;

internal class ReportDetailComponent(IApiClient apiClient, Guid reportId)
    : ReportDetailComponentBase(apiClient)
{
    private readonly Guid _reportId = reportId;
    private ReportDetailModel? _report;

    protected override async Task<ReportDetailModel?> GetReportAsync()
    {
        _report = await ApiClient.GetReportByIdAsync(_reportId);
        await Task.Delay(500); // Simulate some delay for better UX
        return _report;
    }

    protected override string GetHeader()
    {
        return "Report Detail";
    }
}