using PV260.Client.BL;
using PV260.Common.Models;

namespace PV260.Client.Mock;

public class ApiClientMock : IApiClient
{
    private readonly List<ReportDetailModel> _reports = new();
    private readonly List<EmailRecipientModel> _emailRecipients = new();
    private readonly Random _random = new();

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
        var now = DateTime.UtcNow;
        var newReport = new ReportDetailModel 
        { 
            Id = Guid.NewGuid(), 
            CreatedAt = now,
            Name = $"ARK Innovation ETF Report - {now:yyyy-MM-dd HH:mm}",
            Records = GenerateMockRecords()
        };
        _reports.Add(newReport);
        return Task.FromResult(newReport);
    }

    private List<ReportRecordModel> GenerateMockRecords()
    {
        var companies = new[]
        {
            ("Tesla, Inc.", "TSLA"),
            ("Coinbase Global, Inc.", "COIN"),
            ("Roku, Inc.", "ROKU"),
            ("Block, Inc.", "SQ"),
            ("Unity Software Inc.", "U"),
            ("Twilio Inc.", "TWLO"),
            ("Zoom Video Communications, Inc.", "ZM"),
            ("Palantir Technologies Inc.", "PLTR"),
            ("Spotify Technology S.A.", "SPOT"),
            ("Roblox Corporation", "RBLX"),
            ("UiPath Inc.", "PATH"),
            ("DraftKings Inc.", "DKNG"),
            ("Teladoc Health, Inc.", "TDOC"),
            ("Shopify Inc.", "SHOP"),
            ("Snowflake Inc.", "SNOW")
        };

        var records = new List<ReportRecordModel>();
        var totalWeight = 0.0;

        foreach (var (companyName, ticker) in companies)
        {
            var shares = _random.Next(1000, 100000);
            var weight = _random.NextDouble() * 15; // Random weight between 0 and 15%
            totalWeight += weight;

            records.Add(new ReportRecordModel
            {
                Id = Guid.NewGuid(),
                CompanyName = companyName,
                Ticker = ticker,
                NumberOfShares = shares,
                SharesChangePercentage = _random.NextDouble() * 20 - 10, // Random change between -10% and +10%
                Weight = weight
            });
        }

        // Normalize weights to sum up to 100%
        var weightMultiplier = 100.0 / totalWeight;
        foreach (var record in records)
        {
            record.Weight *= weightMultiplier;
        }

        return records;
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
        Task.Delay(2000);
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