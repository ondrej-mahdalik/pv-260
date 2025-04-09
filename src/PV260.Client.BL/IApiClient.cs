using PV260.Common.Models;

namespace PV260.Client.BL;
public interface IApiClient
{
    Task<IEnumerable<ReportModel>> GetAllReportsAsync();
    Task<ReportModel?> GetReportByIdAsync(Guid id);
    Task<ReportModel?> GetLatestReportAsync();
    Task<ReportModel> GenerateNewReportAsync();
    Task DeleteReportAsync(Guid id);
    Task DeleteAllReportsAsync();
    Task SendReportAsync(Guid id);
    Task<SettingsModel?> GetSettingsAsync();
    Task<SettingsModel> UpdateSettingsAsync(SettingsModel settings);
}
