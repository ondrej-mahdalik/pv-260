using PV260.Common.Models;
using PV260.Client.BL;

namespace PV260.Client.Mock;
public class ApiClientMock : IApiClient
{
    private readonly List<ReportDetailModel> _reports = [];

    private readonly SettingsModel? _settings = new()
    {
        ReportGenerationCron = "59 22 * * 0",
        ReportDaysToKeep = 7,
        OldReportCleanupCron = "0 2 * * *",
        SendEmailOnReportGeneration = true
    };

    public Task<IEnumerable<ReportListModel>> GetAllReportsAsync()
    {
        var listModels = _reports.Select(r => new ReportListModel
        {
            Id = r.Id,
            CreatedAt = r.CreatedAt,
            Name = r.Name
        }).ToList();
        
        return Task.FromResult<IEnumerable<ReportListModel>>(listModels);
    }

    public Task<ReportDetailModel?> GetReportByIdAsync(Guid id)
    {
        var report = _reports.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(report);
    }

    public Task<ReportDetailModel?> GetLatestReportAsync()
    {
        var report = _reports.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
        return Task.FromResult(report);
    }

    public Task<ReportDetailModel> GenerateNewReportAsync()
    {
        var newReport = new ReportDetailModel { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Name = "New Report" };
        _reports.Add(newReport);
        return Task.FromResult(newReport);
    }

    public Task DeleteReportAsync(Guid id)
    {
        var report = _reports.FirstOrDefault(r => r.Id == id);
        if (report != null)
        {
            _reports.Remove(report);
        }
        return Task.CompletedTask;
    }

    public Task DeleteAllReportsAsync()
    {
        _reports.Clear();
        return Task.CompletedTask;
    }

    public Task SendReportAsync(Guid id)
    {
        return Task.CompletedTask;
    }

    public Task<SettingsModel?> GetSettingsAsync()
    {
        return Task.FromResult(_settings);
    }
}
