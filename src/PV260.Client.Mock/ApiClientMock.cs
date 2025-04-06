using PV260.Common.Models;
using PV260.Client.BL;

namespace PV260.Client.Mock;

public class ApiClientMock : IApiClient
{
    private readonly List<ReportModel> _reports = new();
    private SettingsModel _settings = new ("59 22 * * 0", 7, true);

    public Task<IEnumerable<ReportModel>> GetAllReportsAsync()
    {
        return Task.FromResult<IEnumerable<ReportModel>>(_reports);
    }

    public Task<ReportModel> GetReportByIdAsync(Guid id)
    {
        var report = _reports.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(report ?? throw new InvalidOperationException("Report not found"));
    }

    public Task<ReportModel> GetLatestReportAsync()
    {
        var report = _reports.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
        return Task.FromResult(report ?? throw new InvalidOperationException("Latest report not found"));
    }

    public Task<ReportModel> GenerateNewReportAsync()
    {
        var newReport = new ReportModel { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
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

    public Task<SettingsModel> GetSettingsAsync()
    {
        return Task.FromResult(_settings);
    }

    public Task<SettingsModel> UpdateSettingsAsync(SettingsModel settings)
    {
        _settings = settings;
        return Task.FromResult(_settings);
    }
}
