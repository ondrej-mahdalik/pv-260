using PV260.Client.BL;
using PV260.Common.Models;

namespace PV260.Client.Mock;

public class ApiClientMock : IApiClient
{
    private readonly List<ReportDetailModel> _reports = new();
    private readonly List<EmailRecipientModel> _emailRecipients = new();

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

    public Task<IEnumerable<EmailRecipientModel>> GetAllEmailsAsync()
    {
        return Task.FromResult<IEnumerable<EmailRecipientModel>>(_emailRecipients);
    }

    public Task AddEmailAsync(EmailRecipientModel emailRecipient)
    {
        _emailRecipients.Add(emailRecipient);
        return Task.CompletedTask;
    }

    public Task DeleteEmailAsync(string email)
    {
        var recipient = _emailRecipients.FirstOrDefault(r => r.EmailAddress == email);
        if (recipient != null)
        {
            _emailRecipients.Remove(recipient);
        }
        return Task.CompletedTask;
    }

    public Task DeleteAllEmailsAsync()
    {
        _emailRecipients.Clear();
        return Task.CompletedTask;
    }
}
