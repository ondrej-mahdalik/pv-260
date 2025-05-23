using ErrorOr;
using PV260.Client.BL;
using PV260.Common.Models;

namespace PV260.Client.Mock;

public class ApiClientMock : IApiClient
{
    private readonly List<ReportDetailModel> _reports = [];
    private readonly List<EmailRecipientModel> _emailRecipients = [];
    private readonly Random _random = new();
    private const int DelayTime = 50;

    public async Task<ErrorOr<List<ReportListModel>>> GetAllReportsAsync()
    {
        // Simulate network delay
        await Task.Delay(DelayTime);

        return _reports
            .Select(r => new ReportListModel
            {
                Id = r.Id,
                CreatedAt = r.CreatedAt,
                Name = r.Name
            })
            .ToList();
    }

    public async Task<ErrorOr<ReportDetailModel?>> GetReportByIdAsync(Guid id)
    {
        var report = _reports.FirstOrDefault(r => r.Id == id);
        
        // Simulate network delay
        await Task.Delay(DelayTime);
        
        return report;
    }

    public async Task<ErrorOr<ReportDetailModel?>> GetLatestReportAsync()
    {
        var report = _reports.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
        
        // Simulate network delay
        await Task.Delay(DelayTime);
        
        return report;
    }

    public async Task<ErrorOr<ReportDetailModel>> GenerateNewReportAsync()
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
        
        // Simulate network delay
        await Task.Delay(DelayTime);
        
        return newReport;
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

    public async Task<ErrorOr<Deleted>> DeleteReportAsync(Guid id)
    {
        var report = _reports.FirstOrDefault(r => r.Id == id);
        if (report != null)
        {
            _reports.Remove(report);
        }
        
        // Simulate network delay
        await Task.Delay(DelayTime);

        return new Deleted();
    }

    public async Task<ErrorOr<Deleted>> DeleteAllReportsAsync()
    {
        _reports.Clear();
        
        // Simulate network delay
        await Task.Delay(DelayTime);
        
        return new Deleted();
    }

    public async Task<ErrorOr<Success>> SendReportAsync(Guid id)
    {
        // Simulate network delay
        await Task.Delay(DelayTime);
        
        return new Success();
    }

    public async Task<ErrorOr<List<EmailRecipientModel>>> GetAllEmailsAsync()
    {
        // Simulate network delay
        await Task.Delay(DelayTime);

        return _emailRecipients;
    }

    public async Task<ErrorOr<Created>> AddEmailAsync(EmailRecipientModel emailRecipient)
    {
        _emailRecipients.Add(emailRecipient);
        
        // Simulate network delay
        await Task.Delay(DelayTime);
        
        return new Created();
    }

    public async Task<ErrorOr<Deleted>> DeleteEmailAsync(string email)
    {
        var recipient = _emailRecipients.FirstOrDefault(r => r.EmailAddress == email);
        if (recipient != null)
        {
            _emailRecipients.Remove(recipient);
        }
        
        // Simulate network delay
        await Task.Delay(DelayTime);
        
        return new Deleted();
    }

    public async Task<ErrorOr<Deleted>> DeleteAllEmailsAsync()
    {
        _emailRecipients.Clear();
        
        // Simulate network delay
        await Task.Delay(DelayTime);
        
        return new Deleted();
    }
}